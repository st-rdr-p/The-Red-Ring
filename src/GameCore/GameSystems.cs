using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Input controller system for running player controller based on input.
    /// </summary>
    public class PlayerInputSystem : ISystem
    {
        private readonly IEngineInput _input;

        public PlayerInputSystem(IEngineInput input)
        {
            _input = input;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<PlayerController>(out var player) &&
                    entity.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
                {
                    HandleInput(player, entity, deltaTime);
                }
            }
        }

        private void HandleInput(PlayerController player, Entity playerEntity, float deltaTime)
        {
            var moveX = _input.GetAxis("Horizontal");
            var moveZ = _input.GetAxis("Vertical");
            var moveDir = new Vector3(moveX, 0, moveZ);

            if (moveDir.Magnitude > 0.01f)
            {
                player.Move(moveDir, deltaTime);
            }

            if (_input.IsKeyDown("Space"))
            {
                player.Jump();
            }

            if (_input.IsKeyDown("Shift"))
            {
                if (moveDir.Magnitude > 0.01f)
                    player.Dash(moveDir);
            }

            player.CheckGround();
        }
    }

    /// <summary>
    /// Puzzle interaction system for handling switches, levers, doors, etc.
    /// </summary>
    public class InteractionSystem : ISystem
    {
        private readonly IEngineInput _input;
        private float _interactionCooldown = 0;

        public InteractionSystem(IEngineInput input)
        {
            _input = input;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            _interactionCooldown -= deltaTime;

            if (_input.IsKeyDown("E") && _interactionCooldown <= 0)
            {
                CheckNearbyInteractables(entities);
                _interactionCooldown = 0.3f;
            }
        }

        private void CheckNearbyInteractables(IEnumerable<Entity> entities)
        {
            Entity playerEntity = null;
            foreach (var e in entities)
            {
                if (e.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
                {
                    playerEntity = e;
                    break;
                }
            }

            if (playerEntity == null || !playerEntity.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            const float interactionDistance = 3f;

            foreach (var entity in entities)
            {
                if (!entity.TryGetComponent<Interactable>(out var interactable))
                    continue;

                if (entity.TryGetComponent<Transform3D>(out var transform))
                {
                    var dist = Vector3.Distance(playerTransform.Position, transform.Position);
                    if (dist <= interactionDistance)
                    {
                        interactable.IsActive = true;
                        interactable.OnInteract?.Invoke(entity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Enemy AI system for updating all enemy behaviors.
    /// </summary>
    public class EnemyAISystem : ISystem
    {
        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            Entity playerEntity = null;
            var entityList = new List<Entity>(entities);

            foreach (var e in entityList)
            {
                if (e.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
                {
                    playerEntity = e;
                    break;
                }
            }

            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<EnemyAI>(out var ai) &&
                    entity.TryGetComponent<Tag>(out var tag) && tag.Value == "Enemy")
                {
                    ai.Update(deltaTime, playerEntity);
                }
            }
        }
    }
}
