using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// 3D physics simulation system with gravity and collisions.
    /// </summary>
    public class Physics3DSystem : ISystem
    {
        private const float Gravity = 9.81f;

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<Rigidbody3D>(out var rb) &&
                    entity.TryGetComponent<Transform3D>(out var transform))
                {
                    if (rb.IsKinematic) continue;

                    // Apply gravity
                    if (rb.UseGravity)
                    {
                        rb.Acceleration = rb.Acceleration + Vector3.Down * Gravity;
                    }

                    // Apply acceleration
                    rb.Velocity = rb.Velocity + rb.Acceleration * deltaTime;

                    // Apply drag
                    rb.Velocity = rb.Velocity * (1 - rb.Drag * deltaTime);

                    // Update position
                    transform.Position = transform.Position + rb.Velocity * deltaTime;

                    // Reset acceleration for next frame
                    rb.Acceleration = Vector3.Zero;
                }
            }
        }
    }

    /// <summary>
    /// Simple collision detection and response system.
    /// </summary>
    public class CollisionSystem : ISystem
    {
        private List<Collider> _colliders = new();

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            _colliders.Clear();

            // Build collider list
            var entityList = new List<Entity>(entities);
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<CollisionSphere>(out var sphere) &&
                    entity.TryGetComponent<Transform3D>(out var transform))
                {
                    _colliders.Add(new Collider { Entity = entity, Sphere = sphere, Transform = transform });
                }
            }

            // Check collisions
            for (int i = 0; i < _colliders.Count; i++)
            {
                for (int j = i + 1; j < _colliders.Count; j++)
                {
                    var c1 = _colliders[i];
                    var c2 = _colliders[j];

                    if (CheckCollision(c1, c2))
                    {
                        HandleCollision(c1, c2);
                    }
                }
            }
        }

        private bool CheckCollision(Collider a, Collider b)
        {
            var dist = Vector3.Distance(a.Transform.Position, b.Transform.Position);
            return dist < (a.Sphere.Radius + b.Sphere.Radius);
        }

        private void HandleCollision(Collider a, Collider b)
        {
            // Handle trampoline bounces
            if (a.Entity.TryGetComponent<Trampoline>(out var trampA) && 
                b.Entity.TryGetComponent<Rigidbody3D>(out var rbB))
            {
                rbB.Velocity = new Vector3(rbB.Velocity.X, trampA.BounceForce, rbB.Velocity.Z);
                return;
            }

            if (b.Entity.TryGetComponent<Trampoline>(out var trampB) && 
                a.Entity.TryGetComponent<Rigidbody3D>(out var rbA))
            {
                rbA.Velocity = new Vector3(rbA.Velocity.X, trampB.BounceForce, rbA.Velocity.Z);
                return;
            }

            // Trigger collisions
            if (a.Sphere.IsTrigger || b.Sphere.IsTrigger)
            {
                // Handle trigger interactions (damage, pickups, etc.)
                if (a.Sphere.IsTrigger && a.Entity.TryGetComponent<DamageSource>(out var dmg))
                {
                    if (b.Entity.TryGetComponent<Health>(out var health))
                    {
                        health.TakeDamage(dmg.Damage);
                        ApplyKnockback(b.Entity, dmg, a.Transform);
                        TriggerDamageFlash(b.Entity);
                    }
                }

                if (b.Sphere.IsTrigger && b.Entity.TryGetComponent<DamageSource>(out var dmg2))
                {
                    if (a.Entity.TryGetComponent<Health>(out var health2))
                    {
                        health2.TakeDamage(dmg2.Damage);
                        ApplyKnockback(a.Entity, dmg2, b.Transform);
                        TriggerDamageFlash(a.Entity);
                    }
                }
            }
            else
            {
                // Solid collisions - separate entities
                var direction = (b.Transform.Position - a.Transform.Position).Normalized;
                var overlap = (a.Sphere.Radius + b.Sphere.Radius) - Vector3.Distance(a.Transform.Position, b.Transform.Position);

                a.Transform.Position = a.Transform.Position - direction * (overlap / 2);
                b.Transform.Position = b.Transform.Position + direction * (overlap / 2);
            }
        }

        private void ApplyKnockback(Entity entity, DamageSource source, Transform3D sourceTransform)
        {
            if (entity.TryGetComponent<Rigidbody3D>(out var rb) &&
                entity.TryGetComponent<Transform3D>(out var transform))
            {
                var dir = (transform.Position - sourceTransform.Position).Normalized;
                rb.Velocity = rb.Velocity + dir * source.KnockbackForce;
            }
        }

        private void TriggerDamageFlash(Entity entity)
        {
            // Add screen flash effect when player takes damage
            if (entity.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
            {
                var flash = new ScreenFlashEffect(0.2f, 0.7f)
                {
                    Color = "red"
                };
                entity.AddComponent(flash);
            }
        }

        private class Collider
        {
            public Entity Entity { get; set; }
            public CollisionSphere Sphere { get; set; }
            public Transform3D Transform { get; set; }
        }
    }
}
