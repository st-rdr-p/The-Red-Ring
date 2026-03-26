namespace GameCore
{
    /// <summary>
    /// Camera component for 3D camera positioning and rotation.
    /// </summary>
    public class Camera : Component
    {
        public float FieldOfView { get; set; } = 60f;
        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = 1000f;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Camera controller that follows a target entity with mouse look support.
    /// </summary>
    public class CameraController : Component
    {
        public Entity FollowTarget { get; set; }
        public float Distance { get; set; } = 5f;
        public float Height { get; set; } = 2f;
        public float MouseSensitivity { get; set; } = 2f;
        public float Pitch { get; set; } = -30f;  // Degrees
        public float Yaw { get; set; } = 0f;      // Degrees
        public float MaxPitch { get; set; } = 60f;
        public float MinPitch { get; set; } = -80f;
        public bool IsLockedToScreen { get; set; } = true;
    }

    /// <summary>
    /// Screen flash effect component for damage/hit feedback.
    /// </summary>
    public class ScreenFlashEffect : Component
    {
        public float Duration { get; set; }
        public float Intensity { get; set; }  // 0 to 1
        public string Color { get; set; } = "red";  // "red", "white", "yellow", etc.
        private float _timeRemaining;

        public ScreenFlashEffect(float duration, float intensity)
        {
            Duration = duration;
            Intensity = intensity;
            _timeRemaining = duration;
        }

        public bool IsActive => _timeRemaining > 0;

        public void Update(float deltaTime)
        {
            _timeRemaining -= deltaTime;
        }

        public float GetCurrentIntensity()
        {
            return (_timeRemaining / Duration) * Intensity;
        }
    }

    /// <summary>
    /// UI layer component for managing screen overlays and flashes.
    /// </summary>
    public class UILayer : Component
    {
        public bool ShowHealthBar { get; set; } = true;
        public bool ShowDamageFlash { get; set; } = true;
        public float HealthBarWidth { get; set; } = 0.3f;
        public float HealthBarHeight { get; set; } = 0.05f;
    }
}
