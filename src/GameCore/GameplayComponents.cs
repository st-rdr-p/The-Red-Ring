namespace GameCore
{
    /// <summary>
    /// Projectile component for bullets, fireballs, homing shots, etc.
    /// </summary>
    public class Projectile : Component
    {
        public string ProjectileType { get; set; }  // "fireball", "ring", "homing_shot"
        public float Speed { get; set; }
        public float Lifetime { get; set; }  // Seconds before despawn
        public float TimeAlive { get; set; }
        public Entity Launcher { get; set; }  // Who fired this
        public bool IsActive { get; set; } = true;

        public Projectile(string type, float speed, float lifetime)
        {
            ProjectileType = type;
            Speed = speed;
            Lifetime = lifetime;
            TimeAlive = 0;
        }
    }

    /// <summary>
    /// Homing projectile component - tracks a target.
    /// </summary>
    public class HomingProjectile : Component
    {
        public Entity Target { get; set; }
        public float TurnSpeed { get; set; } = 10f;
        public float DetectionRange { get; set; } = 50f;

        public HomingProjectile() { }

        public HomingProjectile(Entity target, float turnSpeed = 10f)
        {
            Target = target;
            TurnSpeed = turnSpeed;
        }
    }

    /// <summary>
    /// Collectible item component (coins, rings, health pickups, power-ups).
    /// </summary>
    public class Collectible : Component
    {
        public enum CollectibleType { Coin, Ring, HealthPickup, SpeedBoost, Shield, Invincibility }

        public CollectibleType Type { get; set; }
        public float Value { get; set; }  // Health restored, points awarded, etc.
        public float RotationSpeed { get; set; } = 5f;  // Radians per second
        public bool IsCollected { get; set; } = false;

        public Collectible(CollectibleType type, float value)
        {
            Type = type;
            Value = value;
        }
    }

    /// <summary>
    /// Power-up effect component (temporary ability buffs).
    /// </summary>
    public class PowerUpEffect : Component
    {
        public enum PowerUpType { Invincibility, SpeedBoost, DoubleJump, Slow }

        public PowerUpType Type { get; set; }
        public float Duration { get; set; }
        public float TimeRemaining { get; set; }

        public PowerUpEffect(PowerUpType type, float duration)
        {
            Type = type;
            Duration = duration;
            TimeRemaining = duration;
        }

        public bool IsActive => TimeRemaining > 0;

        public void Update(float deltaTime)
        {
            TimeRemaining -= deltaTime;
        }
    }

    /// <summary>
    /// Hazard component (spikes, lava, falling damage).
    /// </summary>
    public class Hazard : Component
    {
        public enum HazardType { Spike, Lava, Pit, Electricity, Freeze }

        public HazardType Type { get; set; }
        public float DamageAmount { get; set; }
        public float DamageCooldown { get; set; } = 0.5f;  // Prevent rapid damage
        private float _damageTimer = 0;

        public Hazard(HazardType type, float damage)
        {
            Type = type;
            DamageAmount = damage;
        }

        public bool CanDamage
        {
            get
            {
                if (_damageTimer <= 0)
                {
                    _damageTimer = DamageCooldown;
                    return true;
                }
                return false;
            }
        }

        public void Update(float deltaTime)
        {
            _damageTimer -= deltaTime;
        }
    }

    /// <summary>
    /// Moving platform component (elevators, conveyor belts).
    /// </summary>
    public class MovingPlatform : Component
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public float Speed { get; set; }
        public float WaitTime { get; set; } = 0f;
        private float _waitTimer = 0;
        private bool _movingToEnd = true;

        public MovingPlatform(Vector3 start, Vector3 end, float speed)
        {
            StartPosition = start;
            EndPosition = end;
            Speed = speed;
        }

        public void Update(float deltaTime, Transform3D transform)
        {
            if (_waitTimer > 0)
            {
                _waitTimer -= deltaTime;
                return;
            }

            var targetPos = _movingToEnd ? EndPosition : StartPosition;
            var direction = (targetPos - transform.Position).Normalized;
            var distance = Vector3.Distance(transform.Position, targetPos);

            if (distance < Speed * deltaTime)
            {
                transform.Position = targetPos;
                _movingToEnd = !_movingToEnd;
                _waitTimer = WaitTime;
            }
            else
            {
                transform.Position = transform.Position + direction * Speed * deltaTime;
            }
        }
    }

    /// <summary>
    /// Trampoline component (bounces entities).
    /// </summary>
    public class Trampoline : Component
    {
        public float BounceForce { get; set; } = 30f;
        public float BounceCooldown { get; set; } = 0.3f;

        public Trampoline(float bounceForce = 30f)
        {
            BounceForce = bounceForce;
        }
    }

    /// <summary>
    /// Conveyor belt component (pushes entities along).
    /// </summary>
    public class ConveyorBelt : Component
    {
        public Vector3 Direction { get; set; }
        public float Speed { get; set; }

        public ConveyorBelt(Vector3 direction, float speed)
        {
            Direction = direction.Normalized;
            Speed = speed;
        }
    }
}
