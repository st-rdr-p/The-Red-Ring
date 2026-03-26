using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Projectile system - updates projectiles, manages homing, and despawns.
    /// </summary>
    public class ProjectileSystem : ISystem
    {
        private List<Entity> _toRemove = new();

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            _toRemove.Clear();
            var entityList = new List<Entity>(entities);

            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<Projectile>(out var projectile) &&
                    entity.TryGetComponent<Transform3D>(out var transform) &&
                    entity.TryGetComponent<Rigidbody3D>(out var rb))
                {
                    projectile.TimeAlive += deltaTime;

                    // Update direction if homing
                    if (entity.TryGetComponent<HomingProjectile>(out var homing) && homing.Target != null)
                    {
                        if (homing.Target.TryGetComponent<Transform3D>(out var targetTransform))
                        {
                            var direction = (targetTransform.Position - transform.Position).Normalized;
                            var currentDir = rb.Velocity.Normalized;

                            // Smoothly turn toward target
                            var newDir = Vector3.Lerp(currentDir, direction, homing.TurnSpeed * deltaTime);
                            rb.Velocity = newDir * projectile.Speed;
                        }
                    }
                    else
                    {
                        // Normal projectile - maintain speed
                        var direction = rb.Velocity.Normalized;
                        rb.Velocity = direction * projectile.Speed;
                    }

                    // Despawn if lifetime exceeded
                    if (projectile.TimeAlive > projectile.Lifetime)
                    {
                        _toRemove.Add(entity);
                    }
                }
            }

            // This would need engine support to actually remove entities
            // For now, we just mark them inactive
            foreach (var entity in _toRemove)
            {
                if (entity.TryGetComponent<Projectile>(out var proj))
                {
                    proj.IsActive = false;
                }
            }
        }
    }

    /// <summary>
    /// Collectible system - handles pickup collection and effects.
    /// </summary>
    public class CollectibleSystem : ISystem
    {
        private readonly IEngineAudio _audio;

        public CollectibleSystem(IEngineAudio audio)
        {
            _audio = audio;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            Entity player = null;
            var entityList = new List<Entity>(entities);

            // Find player
            foreach (var e in entityList)
            {
                if (e.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
                {
                    player = e;
                    break;
                }
            }

            if (player == null || !player.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            const float pickupRange = 2.0f;

            // Check collectibles
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<Collectible>(out var collectible) &&
                    entity.TryGetComponent<Transform3D>(out var transform) &&
                    !collectible.IsCollected)
                {
                    // Rotate collectible
                    transform.Rotation = transform.Rotation + new Vector3(0, collectible.RotationSpeed * deltaTime, 0);

                    // Check if player is in range
                    var dist = Vector3.Distance(playerTransform.Position, transform.Position);
                    if (dist <= pickupRange)
                    {
                        collectible.IsCollected = true;
                        ApplyCollectibleEffect(player, collectible);
                        _audio.PlaySound($"collect_{collectible.Type.ToString().ToLower()}");
                    }
                }
            }
        }

        private void ApplyCollectibleEffect(Entity player, Collectible collectible)
        {
            switch (collectible.Type)
            {
                case Collectible.CollectibleType.Coin:
                    // Award points (would need a score system)
                    break;

                case Collectible.CollectibleType.Ring:
                    // Award points and health
                    if (player.TryGetComponent<Health>(out var health))
                    {
                        health.Heal((float)collectible.Value);
                    }
                    break;

                case Collectible.CollectibleType.HealthPickup:
                    if (player.TryGetComponent<Health>(out var h))
                    {
                        h.Heal((float)collectible.Value);
                    }
                    break;

                case Collectible.CollectibleType.SpeedBoost:
                    var speedPowerUp = new PowerUpEffect(PowerUpEffect.PowerUpType.SpeedBoost, (float)collectible.Value);
                    player.AddComponent(speedPowerUp);
                    break;

                case Collectible.CollectibleType.Shield:
                    var shieldPowerUp = new PowerUpEffect(PowerUpEffect.PowerUpType.Invincibility, 10f);
                    player.AddComponent(shieldPowerUp);
                    break;

                case Collectible.CollectibleType.Invincibility:
                    var invincibility = new PowerUpEffect(PowerUpEffect.PowerUpType.Invincibility, (float)collectible.Value);
                    player.AddComponent(invincibility);
                    break;
            }
        }
    }

    /// <summary>
    /// Hazard system - applies damage to entities touching hazards.
    /// </summary>
    public class HazardSystem : ISystem
    {
        private readonly IEngineAudio _audio;

        public HazardSystem(IEngineAudio audio)
        {
            _audio = audio;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Update all hazards
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<Hazard>(out var hazard))
                {
                    hazard.Update(deltaTime);
                }
            }

            // Check collisions with hazards
            foreach (var entity in entityList)
            {
                if (!entity.TryGetComponent<Health>(out var health) || 
                    !entity.TryGetComponent<Transform3D>(out var transform) ||
                    !entity.TryGetComponent<CollisionSphere>(out var collider))
                    continue;

                foreach (var hazardEntity in entityList)
                {
                    if (hazardEntity == entity || !hazardEntity.TryGetComponent<Hazard>(out var hazard))
                        continue;

                    if (hazardEntity.TryGetComponent<Transform3D>(out var hazardTransform) &&
                        hazardEntity.TryGetComponent<CollisionSphere>(out var hazardCollider))
                    {
                        var dist = Vector3.Distance(transform.Position, hazardTransform.Position);
                        if (dist < (collider.Radius + hazardCollider.Radius) && hazard.CanDamage)
                        {
                            health.TakeDamage(hazard.DamageAmount);
                            _audio.PlaySound($"hazard_{hazard.Type.ToString().ToLower()}");

                            // Knockback from hazard
                            if (entity.TryGetComponent<Rigidbody3D>(out var rb))
                            {
                                var dir = (transform.Position - hazardTransform.Position).Normalized;
                                rb.Velocity = rb.Velocity + dir * 10f;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Platform system - updates moving platforms and applies conveyor belt physics.
    /// </summary>
    public class PlatformSystem : ISystem
    {
        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Update moving platforms
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<MovingPlatform>(out var platform) &&
                    entity.TryGetComponent<Transform3D>(out var transform))
                {
                    platform.Update(deltaTime, transform);
                }
            }

            // Apply conveyor belt effects
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<ConveyorBelt>(out var belt) &&
                    entity.TryGetComponent<Transform3D>(out var beltTransform) &&
                    entity.TryGetComponent<CollisionSphere>(out var beltCollider))
                {
                    // Check for entities on belt
                    foreach (var other in entityList)
                    {
                        if (other == entity || !other.TryGetComponent<Transform3D>(out var otherTransform))
                            continue;

                        if (other.TryGetComponent<CollisionSphere>(out var otherCollider) &&
                            other.TryGetComponent<Rigidbody3D>(out var rb))
                        {
                            var dist = Vector3.Distance(otherTransform.Position, beltTransform.Position);
                            if (dist < (beltCollider.Radius + otherCollider.Radius))
                            {
                                // Push entity along belt
                                rb.Velocity = rb.Velocity + belt.Direction * belt.Speed * deltaTime;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Power-up system - manages temporary ability buffs.
    /// </summary>
    public class PowerUpSystem : ISystem
    {
        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<PowerUpEffect>(out var powerUp))
                {
                    powerUp.Update(deltaTime);

                    // Apply effects while active
                    if (powerUp.IsActive)
                    {
                        ApplyPowerUpEffect(entity, powerUp, deltaTime);
                    }
                }
            }
        }

        private void ApplyPowerUpEffect(Entity entity, PowerUpEffect powerUp, float deltaTime)
        {
            if (!entity.TryGetComponent<PlayerController>(out var controller))
                return;

            switch (powerUp.Type)
            {
                case PowerUpEffect.PowerUpType.SpeedBoost:
                    // Increase speed temporarily
                    controller.MaxSpeed *= 1.5f;
                    break;

                case PowerUpEffect.PowerUpType.Invincibility:
                    // Make invincible (skip damage in collision system)
                    break;

                case PowerUpEffect.PowerUpType.DoubleJump:
                    // Already have double jump, but could extend count
                    break;

                case PowerUpEffect.PowerUpType.Slow:
                    // Slow down movement
                    controller.Speed *= 0.5f;
                    break;
            }
        }
    }
}
