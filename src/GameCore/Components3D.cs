using System;

namespace GameCore
{
    /// <summary>
    /// 3D transform component for position, rotation, and scale in world space.
    /// </summary>
    public class Transform3D : Component
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }  // Euler angles in radians
        public Vector3 Scale { get; set; } = Vector3.One;
    }

    /// <summary>
    /// 3D physics body component with velocity and acceleration.
    /// </summary>
    public class Rigidbody3D : Component
    {
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Mass { get; set; } = 1.0f;
        public bool UseGravity { get; set; } = true;
        public float Drag { get; set; } = 0.1f;
        public bool IsKinematic { get; set; } = false;
    }

    /// <summary>
    /// 3D mesh renderer component.
    /// </summary>
    public class MeshRenderer : Component
    {
        public string MeshId { get; set; }
        public string MaterialId { get; set; }
    }

    /// <summary>
    /// Health and damage system component.
    /// </summary>
    public class Health : Component
    {
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public bool IsAlive => CurrentHealth > 0;

        public Health(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        public void Heal(float amount)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
        }
    }

    /// <summary>
    /// Damage source component (used by weapons, projectiles, etc.)
    /// </summary>
    public class DamageSource : Component
    {
        public float Damage { get; set; }
        public string SourceTag { get; set; }  // "player", "enemy", "trap"
        public float KnockbackForce { get; set; } = 10f;
    }

    /// <summary>
    /// Collision sphere for simple hit detection.
    /// </summary>
    public class CollisionSphere : Component
    {
        public float Radius { get; set; }
        public bool IsTrigger { get; set; } = false;
    }

    /// <summary>
    /// Puzzle interaction component (switches, levers, doors, etc.)
    /// </summary>
    public class Interactable : Component
    {
        public string InteractionId { get; set; }
        public bool IsActive { get; set; } = false;
        public Action<Entity> OnInteract { get; set; }
    }

    /// <summary>
    /// Tag component for quick categorization.
    /// </summary>
    public class Tag : Component
    {
        public string Value { get; set; }

        public Tag(string value)
        {
            Value = value;
        }
    }
}
