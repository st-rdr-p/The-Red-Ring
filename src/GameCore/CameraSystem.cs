using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Camera system that updates camera position/rotation based on mouse input.
    /// </summary>
    public class CameraSystem : ISystem
    {
        private readonly IEngineInput _input;
        private float _lastMouseX = 0;
        private float _lastMouseY = 0;

        public CameraSystem(IEngineInput input)
        {
            _input = input;
            _input.LockMouse(true);  // Start with mouse locked
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<CameraController>(out var camera) &&
                    entity.TryGetComponent<Transform3D>(out var transform) &&
                    camera.FollowTarget != null &&
                    camera.FollowTarget.TryGetComponent<Transform3D>(out var targetTransform))
                {
                    UpdateCamera(camera, transform, targetTransform, deltaTime);
                }
            }

            // Toggle mouse lock with pause menu (ESC key)
            if (_input.IsKeyDown("Escape"))
            {
                _input.LockMouse(!_input.IsMouseLocked);
            }
        }

        private void UpdateCamera(CameraController camera, Transform3D cameraTransform, Transform3D targetTransform, float deltaTime)
        {
            // Get mouse delta
            var mouseX = _input.GetMouseX();
            var mouseY = _input.GetMouseY();

            var mouseDeltaX = (mouseX - _lastMouseX) * camera.MouseSensitivity;
            var mouseDeltaY = (mouseY - _lastMouseY) * camera.MouseSensitivity;

            _lastMouseX = mouseX;
            _lastMouseY = mouseY;

            // Update yaw and pitch based on mouse
            camera.Yaw += mouseDeltaX;
            camera.Pitch -= mouseDeltaY;  // Invert Y

            // Clamp pitch
            camera.Pitch = Math.Max(camera.MinPitch, Math.Min(camera.MaxPitch, camera.Pitch));

            // Convert angles to radians
            var pitchRad = camera.Pitch * MathF.PI / 180f;
            var yawRad = camera.Yaw * MathF.PI / 180f;

            // Calculate camera offset from target
            var offsetX = camera.Distance * MathF.Sin(yawRad) * MathF.Cos(pitchRad);
            var offsetY = camera.Distance * MathF.Sin(pitchRad) + camera.Height;
            var offsetZ = camera.Distance * MathF.Cos(yawRad) * MathF.Cos(pitchRad);

            // Position camera behind and above target
            cameraTransform.Position = new Vector3(
                targetTransform.Position.X + offsetX,
                targetTransform.Position.Y + offsetY,
                targetTransform.Position.Z + offsetZ
            );

            // Look at target (with slight offset up for better view)
            var lookTarget = new Vector3(
                targetTransform.Position.X,
                targetTransform.Position.Y + camera.Height * 0.5f,
                targetTransform.Position.Z
            );

            var direction = (lookTarget - cameraTransform.Position).Normalized;
            cameraTransform.Rotation = new Vector3(
                MathF.Asin(-direction.Y),
                MathF.Atan2(direction.X, direction.Z),
                0
            );
        }
    }

    /// <summary>
    /// Screen flash system that renders damage feedback flash on all monitors.
    /// </summary>
    public class ScreenFlashSystem : ISystem
    {
        private readonly IEngineRenderer _renderer;
        private List<ScreenFlashEffect> _activeFlashes = new();

        public ScreenFlashSystem(IEngineRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            _activeFlashes.Clear();

            // Collect all active screen flash effects
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<ScreenFlashEffect>(out var flash))
                {
                    flash.Update(deltaTime);
                    if (flash.IsActive)
                    {
                        _activeFlashes.Add(flash);
                    }
                }
            }

            // Render combined intensity from all flashes
            float maxIntensity = 0;
            string flashColor = "red";

            foreach (var flash in _activeFlashes)
            {
                var intensity = flash.GetCurrentIntensity();
                if (intensity > maxIntensity)
                {
                    maxIntensity = intensity;
                    flashColor = flash.Color;
                }
            }

            if (maxIntensity > 0)
            {
                _renderer.DrawScreenFlash(maxIntensity, flashColor);
            }
        }
    }
}
