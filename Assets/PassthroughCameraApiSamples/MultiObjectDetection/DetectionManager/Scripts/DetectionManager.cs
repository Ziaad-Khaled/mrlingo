// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using SceneLogic;
using UnityEngine;
using UnityEngine.Events;

namespace PassthroughCameraSamples.MultiObjectDetection
{
    [MetaCodeSample("PassthroughCameraApiSamples-MultiObjectDetection")]
    public class DetectionManager : MonoBehaviour
    {
        [SerializeField] private WebCamTextureManager m_webCamTextureManager;

        [Header("Controls configuration")]
        [SerializeField] private OVRInput.RawButton m_actionButton = OVRInput.RawButton.A;

        [Header("Ui references")]
        [SerializeField] private DetectionUiMenuManager m_uiMenuManager;

        [Header("Placement configureation")]
        [SerializeField] private GameObject m_spwanMarker;
        [SerializeField] private EnvironmentRayCastSampleManager m_environmentRaycast;
        [SerializeField] private float m_spawnDistance = 0.25f;
        [SerializeField] private AudioSource m_placeSound;
        [SerializeField] private float m_markerOffsetRight = 0.2f;

        [Header("Sentis inference ref")]
        [SerializeField] private SentisInferenceRunManager m_runInference;
        [SerializeField] private SentisInferenceUiManager m_uiInference;
        
        [Header("Scaling configuration")]
        [Tooltip("World‑space distance (in metres) at which the marker keeps its *original prefab* scale. Farther away ⇒ proportionally larger. Nearer ⇒ never smaller than the prefab scale.")]
        [SerializeField] private float m_referenceScaleDistance = 0.5f;
        
        [Space(10)]
        public UnityEvent<int> OnObjectsIdentified;

        private bool m_isPaused = true;
        private List<GameObject> m_spwanedEntities = new();
        private bool m_isStarted = false;
        private bool m_isSentisReady = false;
        private float m_delayPauseBackTime = 0;
        private SceneModeManager m_sceneModeManager;

        #region Unity Functions
        private void Awake() => OVRManager.display.RecenteredPose += CleanMarkersCallBack;

        private IEnumerator Start()
        {
            // Wait until Sentis model is loaded
            var sentisInference = FindAnyObjectByType<SentisInferenceRunManager>();
            while (!sentisInference.IsModelLoaded)
            {
                yield return null;
            }
            m_isSentisReady = true;
            m_sceneModeManager = FindAnyObjectByType<SceneModeManager>();
            if(m_sceneModeManager.CurrentMode == SceneMode.Educational)
            {
                m_markerOffsetRight = 0.0f;
            }
        }

        private void Update()
        {
            // Get the WebCamTexture CPU image
            var hasWebCamTextureData = m_webCamTextureManager.WebCamTexture != null;

            if (!m_isStarted)
            {
                // Manage the Initial Ui Menu
                if (hasWebCamTextureData && m_isSentisReady)
                {
                    m_uiMenuManager.OnInitialMenu(m_environmentRaycast.HasScenePermission());
                    m_isStarted = true;
                }
            }
            else
            {
                // Press A button to spawn 3d markers
                if (OVRInput.GetUp(m_actionButton) && m_delayPauseBackTime <= 0)
                {
                    SpwanCurrentDetectedObjects();
                }
                // Cooldown for the A button after return from the pause menu
                m_delayPauseBackTime -= Time.deltaTime;
                if (m_delayPauseBackTime <= 0)
                {
                    m_delayPauseBackTime = 0;
                }
            }

            // Not start a sentis inference if the app is paused or we don't have a valid WebCamTexture
            if (m_isPaused || !hasWebCamTextureData)
            {
                if (m_isPaused)
                {
                    // Set the delay time for the A button to return from the pause menu
                    m_delayPauseBackTime = 0.1f;
                }
                return;
            }

            // Run a new inference when the current inference finishes
            if (!m_runInference.IsRunning())
            {
                m_runInference.RunInference(m_webCamTextureManager.WebCamTexture);
            }
        }
        #endregion

        #region Marker Functions
        /// <summary>
        /// Clean 3d markers when the tracking space is re-centered.
        /// </summary>
        private void CleanMarkersCallBack()
        {
            foreach (var e in m_spwanedEntities)
            {
                Destroy(e, 0.1f);
            }
            m_spwanedEntities.Clear();
            OnObjectsIdentified?.Invoke(-1);
        }
        /// <summary>
        /// Spwan 3d markers for the detected objects
        /// </summary>
        private void SpwanCurrentDetectedObjects()
        {
            /* ---------- QUIZ MODE ---------- */
            if (m_sceneModeManager.CurrentMode == SceneMode.Quiz)
            {
                // 1. Wipe existing markers so we don’t overlap
                foreach (var m in m_spwanedEntities)
                {
                    if (m) Destroy(m);
                }
                m_spwanedEntities.Clear();

                // 2. Early-out if nothing detected
                var boxes = m_uiInference.BoxDrawn;
                if (boxes.Count == 0)
                {
                    OnObjectsIdentified?.Invoke(0);
                    return;
                }

                // 3. Pick ONE detection at random
                int randomIdx = Random.Range(0, boxes.Count);
                var box       = boxes[randomIdx];

                // 4. Spawn that single marker
                int spawned = PlaceMarkerUsingEnvironmentRaycast(box.WorldPos, box.ClassName) ? 1 : 0;
                if (spawned > 0) m_placeSound.Play();

                OnObjectsIdentified?.Invoke(spawned);
                return;
            }

            /* ---------- EDUCATIONAL MODE---------- */
            int count = 0;
            
            // 1. Wipe existing markers so we don’t overlap
            foreach (var m in m_spwanedEntities)
            {
                if (m) Destroy(m);
            }
            m_spwanedEntities.Clear();
            
            foreach (var box in m_uiInference.BoxDrawn)
            {
                if (PlaceMarkerUsingEnvironmentRaycast(box.WorldPos, box.ClassName))
                    count++;
            }

            if (count > 0) m_placeSound.Play();
            OnObjectsIdentified?.Invoke(count);
        }

        /// <summary>
        /// Place a marker using the environment raycast
        /// </summary>
        private bool PlaceMarkerUsingEnvironmentRaycast(Vector3? position, string className)
        {
            if (!position.HasValue) return false;

            /* ---------- duplicate-check unchanged ---------- */
            foreach (var e in m_spwanedEntities)
            {
                var anim = e.GetComponent<DetectionSpawnMarkerAnim>();
                if (anim == null) continue;

                if (Vector3.Distance(e.transform.position, position.Value) < m_spawnDistance &&
                    anim.GetYoloClassName() == className)
                {
                    return false;                     // already have one
                }
            }

            /* ---------- spawn & offset ---------- */
            var marker = Instantiate(m_spwanMarker);
            marker.SetActive(true);
            m_spwanedEntities.Add(marker);

            // Push the marker to the right of the camera so the object stays visible.
            Vector3 flatRight = new Vector3(Camera.main.transform.right.x,
                0f,
                Camera.main.transform.right.z).normalized;
            Vector3 shiftedPos = position.Value + flatRight * m_markerOffsetRight;
            marker.transform.position = shiftedPos;

            // Face the camera
            Vector3 toCam = Camera.main.transform.position - marker.transform.position;
            toCam.y = 0f;                                 // keep upright
            if (toCam != Vector3.zero)
                marker.transform.rotation = Quaternion.LookRotation(toCam);
            
            // ---------------- SCALE RELATIVE TO CAMERA ----------------
            Vector3 originalScale = marker.transform.localScale;               // the prefab’s scale (e.g., 0.001)
            float   distance      = Vector3.Distance(Camera.main.transform.position, marker.transform.position);
            float   scaleFactor   = distance / Mathf.Max(0.0001f, m_referenceScaleDistance);
            if (scaleFactor < 1f) scaleFactor = 1f;                            // never shrink below original
            marker.transform.localScale = originalScale * scaleFactor;
            // ----------------------------------------------------------------

            marker.GetComponent<DetectionSpawnMarkerAnim>()
                .SetYoloClassName(className);

            return true;
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// Pause the detection logic when the pause menu is active
        /// </summary>
        public void OnPause(bool pause)
        {
            m_isPaused = pause;
        }
        #endregion
    }
}
