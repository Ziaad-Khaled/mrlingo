using UnityEngine;

namespace SceneLogic
{
    /// <summary>
    /// The two high-level interaction modes a scene can be in.
    /// </summary>
    [System.Serializable]
    public enum SceneMode
    {
        Educational,
        Quiz
    }

    /// <summary>
    /// Attach this to an empty GameObject (e.g., “SceneController”).
    /// Other scripts can query or subscribe to <see cref="CurrentMode"/>
    /// to branch their behaviour.
    /// </summary>
    public class SceneModeManager : MonoBehaviour
    {
        [Header("Current scene mode")]
        [SerializeField] private SceneMode startMode = SceneMode.Educational;

        /// <summary>
        /// Globally visible but set only by this component
        /// (change via <see cref="SetMode"/>).
        /// </summary>
        public SceneMode CurrentMode { get; private set; }

        private void Awake()
        {
            CurrentMode = startMode;
        }

        /// <summary>
        /// Call this from UI buttons, triggers, etc. to switch modes
        /// and fire any mode-specific events.
        /// </summary>
        public void SetMode(SceneMode newMode)
        {
            if (CurrentMode == newMode) return;

            CurrentMode = newMode;
            Debug.Log($"Scene mode switched to {CurrentMode}");

            // TODO: invoke mode-specific actions here
            // e.g., EventBus.Raise(new ModeChangedEvent(CurrentMode));
        }
    }
}