using System;

namespace GameCore
{
    /// <summary>
    /// Base controller for player character with movement, jumping, and speed mechanics.
    /// </summary>
    public class PlayerController : Component
    {
        public float Speed { get; set; } = 15f;
        public float JumpForce { get; set; } = 10f;
        public float MaxSpeed { get; set; } = 20f;
        public bool IsGrounded { get; set; } = true;
        public bool CanDoubleJump { get; set; } = true;
        private bool _hasDoubleJump = true;
        
        /// <summary>
        /// Called when player dashes to spawn projectiles.
        /// </summary>
        public System.Action<Entity, Vector3> OnDash { get; set; }

        public void Move(Vector3 direction, float deltaTime)
        {
            if (Owner.TryGetComponent<Rigidbody3D>(out var rb))
            {
                direction = direction.Normalized;
                rb.Velocity = rb.Velocity + direction * Speed * deltaTime;

                // Cap speed
                if (rb.Velocity.Magnitude > MaxSpeed)
                {
                    rb.Velocity = rb.Velocity.Normalized * MaxSpeed;
                }
            }

            // Rotate character to face direction
            if (direction.Magnitude > 0.01f)
            {
                if (Owner.TryGetComponent<Transform3D>(out var transform))
                {
                    var angle = MathF.Atan2(direction.X, direction.Z);
                    transform.Rotation = new Vector3(0, angle, 0);
                }
            }
        }

        public void Jump()
        {
            if (IsGrounded)
            {
                if (Owner.TryGetComponent<Rigidbody3D>(out var rb))
                {
                    rb.Velocity = new Vector3(rb.Velocity.X, JumpForce, rb.Velocity.Z);
                    IsGrounded = false;
                    _hasDoubleJump = CanDoubleJump;
                }
            }
            else if (_hasDoubleJump && CanDoubleJump)
            {
                if (Owner.TryGetComponent<Rigidbody3D>(out var rb))
                {
                    rb.Velocity = new Vector3(rb.Velocity.X, JumpForce, rb.Velocity.Z);
                    _hasDoubleJump = false;
                }
            }
        }

        public void CheckGround(float rayDistance = 0.5f)
        {
            if (Owner.TryGetComponent<Transform3D>(out var transform) &&
                Owner.TryGetComponent<CollisionSphere>(out var collider))
            {
                // Simple ground check: cast down from position
                IsGrounded = (transform.Position.Y - collider.Radius) <= 0;
            }
        }

        public void Dash(Vector3 direction)
        {
            if (Owner.TryGetComponent<Rigidbody3D>(out var rb))
            {
                var dashVelocity = direction.Normalized * MaxSpeed * 1.5f;
                rb.Velocity = dashVelocity;
                
                // Trigger dash event (throws rocks)
                OnDash?.Invoke(Owner, direction.Normalized);
            }
        }
    }

    /// <summary>
    /// Base AI controller for enemies with patrol, chase, and attack behaviors.
    /// </summary>
    public class EnemyAI : Component
    {
        public enum State { Idle, Patrol, Chase, Attack, Damaged }

        public State CurrentState { get; set; } = State.Patrol;
        public float DetectionRange { get; set; } = 20f;
        public float AttackRange { get; set; } = 5f;
        public float PatrolSpeed { get; set; } = 5f;
        public float ChaseSpeed { get; set; } = 12f;
        public float AttackCooldown { get; set; } = 1.5f;

        private float _attackTimer = 0;
        private Entity _target;
        private Vector3 _patrolDirection = Vector3.Forward;
        private float _directionChangeTimer = 0;

        public void Update(float deltaTime, Entity target)
        {
            _attackTimer -= deltaTime;
            _directionChangeTimer -= deltaTime;

            if (!Owner.TryGetComponent<Transform3D>(out var transform))
                return;

            var distToTarget = target != null && target.TryGetComponent<Transform3D>(out var targetTransform)
                ? Vector3.Distance(transform.Position, targetTransform.Position)
                : float.MaxValue;

            // State transitions
            if (target != null && distToTarget < DetectionRange && target.TryGetComponent<Health>(out var health) && health.IsAlive)
            {
                if (distToTarget < AttackRange)
                    CurrentState = State.Attack;
                else
                    CurrentState = State.Chase;
                _target = target;
            }
            else
            {
                CurrentState = State.Patrol;
                _target = null;
            }

            // State behavior
            switch (CurrentState)
            {
                case State.Patrol:
                    Patrol(deltaTime);
                    break;
                case State.Chase:
                    Chase(deltaTime, _target);
                    break;
                case State.Attack:
                    Attack(deltaTime, _target);
                    break;
            }
        }

        private void Patrol(float deltaTime)
        {
            if (_directionChangeTimer <= 0)
            {
                _patrolDirection = new Vector3(
                    MathF.Cos((float)DateTime.UtcNow.Ticks * 0.001f),
                    0,
                    MathF.Sin((float)DateTime.UtcNow.Ticks * 0.001f)
                ).Normalized;
                _directionChangeTimer = 5f;
            }

            if (Owner.TryGetComponent<Rigidbody3D>(out var rb))
            {
                rb.Velocity = new Vector3(
                    _patrolDirection.X * PatrolSpeed,
                    rb.Velocity.Y,
                    _patrolDirection.Z * PatrolSpeed
                );
            }
        }

        private void Chase(float deltaTime, Entity target)
        {
            if (target == null || !target.TryGetComponent<Transform3D>(out var targetTransform))
                return;

            if (Owner.TryGetComponent<Transform3D>(out var transform) &&
                Owner.TryGetComponent<Rigidbody3D>(out var rb))
            {
                var direction = (targetTransform.Position - transform.Position).Normalized;
                rb.Velocity = new Vector3(
                    direction.X * ChaseSpeed,
                    rb.Velocity.Y,
                    direction.Z * ChaseSpeed
                );

                // Face target
                var angle = MathF.Atan2(direction.X, direction.Z);
                transform.Rotation = new Vector3(0, angle, 0);
            }
        }

        private void Attack(float deltaTime, Entity target)
        {
            if (_attackTimer <= 0 && target != null && Owner.TryGetComponent<DamageSource>(out var dmg))
            {
                if (target.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(dmg.Damage);
                }
                _attackTimer = AttackCooldown;
            }
        }
    }
}
