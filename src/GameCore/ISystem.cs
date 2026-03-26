using System.Collections.Generic;

namespace GameCore
{
    public interface ISystem
    {
        void Update(float deltaTime, IEnumerable<Entity> entities);
    }

    public class PhysicsSystem : ISystem
    {
        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<RigidbodyComponent>(out var rb) &&
                    entity.TryGetComponent<TransformComponent>(out var transform))
                {
                    transform.X += rb.VelX * deltaTime;
                    transform.Y += rb.VelY * deltaTime;

                    // Gravity example
                    rb.VelY += 9.81f * deltaTime;
                }
            }
        }
    }

    public class RenderSystem : ISystem
    {
        private readonly IEngineRenderer _renderer;

        public RenderSystem(IEngineRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<TransformComponent>(out var transform) &&
                    entity.TryGetComponent<SpriteComponent>(out var sprite))
                {
                    _renderer.DrawSprite(sprite.SpriteId, transform.X, transform.Y, transform.Rotation, transform.ScaleX, transform.ScaleY, sprite.Opacity);
                }
            }
        }
    }
}
