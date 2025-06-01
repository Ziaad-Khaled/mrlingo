// UIInteracttionTest.cs
// A minimal sample that instantiates a serialized prefab a fixed distance in front of a serialized target object
// whenever the user presses the configured controller button.
// Drop it on an empty GameObject, assign the three inspector references, and press Play.

using UnityEngine;
using UnityEngine.Events;

namespace PassthroughCameraSamples.Utility
{
    public class UIInteracttionTest : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Prefab to spawn when the button is pressed")] 
        [SerializeField] private GameObject m_prefabToSpawn;

        [Tooltip("Object that the prefab will be spawned in front of")] 
        [SerializeField] private Transform m_referenceObject;

        [Tooltip("Distance (metres) in front of the reference object to spawn the prefab")]
        [Min(0.01f)]
        [SerializeField] private float m_forwardOffset = 0.2f;

        [Header("Input Settings")]
        [Tooltip("Controller button used to trigger the spawn")] 
        [SerializeField] private OVRInput.RawButton m_spawnButton = OVRInput.RawButton.A;

        [Space(10)]
        public UnityEvent OnSpawned; // Invoked each time the prefab is instantiated

        private void Update()
        {
            // Ensure required fields are assigned before continuing
            if (m_prefabToSpawn == null || m_referenceObject == null) return;

            // Spawn the prefab when the button is released
            if (OVRInput.GetUp(m_spawnButton))
            {
                SpawnPrefab();
            }
        }

        private void SpawnPrefab()
        {
            var camTransform = Camera.main != null ? Camera.main.transform : null;
            if (camTransform == null) return;

            // Spawn position: m_forwardOffset meters in front of the camera
            var spawnPos = camTransform.position + camTransform.forward * m_forwardOffset;

            // Option A: Same rotation as the camera (faces the same direction)
            var spawnRot = camTransform.rotation;

            // Option B: Face the camera (useful for UI or interactive objects)
            // var spawnRot = Quaternion.LookRotation(spawnPos - camTransform.position);

            Instantiate(m_prefabToSpawn, spawnPos, spawnRot, null);
            OnSpawned?.Invoke();
        }
    }
}
