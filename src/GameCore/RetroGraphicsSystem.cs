using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// System that handles retro graphics post-processing effects.
    /// Applies pixelation, color reduction, scanlines, etc. to the render output.
    /// </summary>
    public class RetroGraphicsSystem : ISystem
    {
        private readonly IEngineRenderer _renderer;
        private List<RetroGraphicsEffect> _activeEffects = new();

        public RetroGraphicsSystem(IEngineRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            _activeEffects.Clear();

            // Collect all active retro graphics effects
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<RetroGraphicsEffect>(out var effect) && effect.Enabled)
                {
                    _activeEffects.Add(effect);
                }
            }

            // Apply effects (typically only one should be active at a time)
            // If multiple exist, the first one takes precedence
            if (_activeEffects.Count > 0)
            {
                _renderer.ApplyRetroGraphics(_activeEffects[0]);
            }
        }
    }
}
