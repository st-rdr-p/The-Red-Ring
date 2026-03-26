namespace GameCore
{
    /// <summary>
    /// Example game setup for a 3D Sonic-like game.
    /// Shows how to initialize entities, systems, and components.
    /// </summary>
    public class GameSetup
    {
        /// <summary>
        /// Initialize a sample 3D game with player, enemy, and interactive puzzle elements.
        /// </summary>
        public static void SetupSonicLikeGame(Game game, IEngineInput input, IEngineRenderer renderer, IEngineAudio audio)
        {
            // Add systems in order of execution
            game.AddSystem(new Physics3DSystem());
            game.AddSystem(new CollisionSystem());
            game.AddSystem(new PlayerInputSystem(input));
            game.AddSystem(new ProjectileSystem());
            game.AddSystem(new CollectibleSystem(audio));
            game.AddSystem(new HazardSystem(audio));
            game.AddSystem(new PlatformSystem());
            game.AddSystem(new PowerUpSystem());
            game.AddSystem(new EnemyAISystem());
            game.AddSystem(new InteractionSystem(input));
            game.AddSystem(new CameraSystem(input));
            game.AddSystem(new ScreenFlashSystem(renderer));
            game.AddSystem(new RetroGraphicsSystem(renderer));

            // Create player
            var player = game.CreateEntity();
            player.AddComponent(new Tag("Player"));
            player.AddComponent(new Transform3D { Position = Vector3.Zero });
            player.AddComponent(new Rigidbody3D { Mass = 1.0f, UseGravity = true });
            player.AddComponent(new MeshRenderer { MeshId = "player_mesh", MaterialId = "player_material" });
            player.AddComponent(new CollisionSphere { Radius = 1.0f });
            player.AddComponent(new Health(100));
        var playerController = player.AddComponent(new PlayerController 
        { 
            Speed = 15f, 
            JumpForce = 15f,
            MaxSpeed = 25f,
            CanDoubleJump = true 
        });

        // Set up dash callback to spawn rocks
        playerController.OnDash = (playerEntity, dashDirection) =>
        {
            SpawnDashRock(game, playerEntity, dashDirection);
        };
                AttackRange = 3f,
                PatrolSpeed = 5f,
                ChaseSpeed = 12f
            });

            // Create sample puzzle switch
            var puzzleSwitch = game.CreateEntity();
            puzzleSwitch.AddComponent(new Tag("Interactable"));
            puzzleSwitch.AddComponent(new Transform3D { Position = new Vector3(0, 0, 10) });
            puzzleSwitch.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
            puzzleSwitch.AddComponent(new Interactable 
            { 
                InteractionId = "door_opener",
                OnInteract = (e) => 
                {
                    if (e.TryGetComponent<Interactable>(out var inter))
                    {
                        inter.IsActive = !inter.IsActive;
                    }
                }
            });

            // Apply retro graphics effect (Sonic 3 style - Sega Genesis)
            var retroEffect = game.CreateEntity();
            retroEffect.AddComponent(new Tag("GraphicsSettings"));
            retroEffect.AddComponent(RetroPresets.SegaGenesis);

            // Create sample collectibles (rings)
            for (int i = 0; i < 5; i++)
            {
                var ring = game.CreateEntity();
                ring.AddComponent(new Tag("Collectible"));
                ring.AddComponent(new Transform3D { Position = new Vector3(5 + i * 2, 1, 0) });
                ring.AddComponent(new CollisionSphere { Radius = 0.3f, IsTrigger = true });
                ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, 10));
            }

            // Create health pickup
            var healthPickup = game.CreateEntity();
            healthPickup.AddComponent(new Tag("Collectible"));
            healthPickup.AddComponent(new Transform3D { Position = new Vector3(-5, 1, 5) });
            healthPickup.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
            healthPickup.AddComponent(new Collectible(Collectible.CollectibleType.HealthPickup, 25));

            // Create hazard (spikes)
            var hazard = game.CreateEntity();
            hazard.AddComponent(new Tag("Hazard"));
            hazard.AddComponent(new Transform3D { Position = new Vector3(0, 0, -5) });
            hazard.AddComponent(new CollisionSphere { Radius = 1.0f, IsTrigger = true });
            hazard.AddComponent(new Hazard(Hazard.HazardType.Spike, 20));

            // Create moving platform
            var platform = game.CreateEntity();
            platform.AddComponent(new Tag("Platform"));
            platform.AddComponent(new Transform3D { Position = new Vector3(0, 0, 0) });
            platform.AddComponent(new CollisionSphere { Radius = 2.0f });
            platform.AddComponent(new Rigidbody3D { IsKinematic = true });
            platform.AddComponent(new MovingPlatform(
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 0),
                5f
            ));

            // Create conveyor belt
            var conveyor = game.CreateEntity();
            conveyor.AddComponent(new Tag("Conveyor"));
            conveyor.AddComponent(new Transform3D { Position = new Vector3(-10, -2, 0) });
            conveyor.AddComponent(new CollisionSphere { Radius = 3.0f });
            conveyor.AddComponent(new ConveyorBelt(Vector3.Right, 8f));

            // Create trampoline
            var trampoline = game.CreateEntity();
            trampoline.AddComponent(new Tag("Trampoline"));
            trampoline.AddComponent(new Transform3D { Position = new Vector3(5, -5, 0) });
            trampoline.AddComponent(new CollisionSphere { Radius = 1.0f });
            trampoline.AddComponent(new Trampoline(40f));
        }

        /// <summary>
        /// Spawns a rock projectile when player dashes.
        /// </summary>
        private static void SpawnDashRock(Game game, Entity player, Vector3 direction)
        {
            if (!player.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            // Create rock projectile
            var rock = game.CreateEntity();
            rock.AddComponent(new Tag("Projectile"));
            
            // Spawn slightly in front of player
            var spawnPos = playerTransform.Position + direction * 2f;
            rock.AddComponent(new Transform3D { Position = spawnPos });
            
            rock.AddComponent(new Rigidbody3D 
            { 
                IsKinematic = true,
                Velocity = direction * 25f  // Rock inherits dash velocity
            });
            
            rock.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
            rock.AddComponent(new MeshRenderer { MeshId = "rock_mesh", MaterialId = "rock_material" });
            
            // Rock projectile: 7 second lifetime, 15 damage
            rock.AddComponent(new Projectile("rock", speed: 25f, lifetime: 7f) 
            { 
                Launcher = player 
            });
            
            rock.AddComponent(new DamageSource 
            { 
                Damage = 15, 
                SourceTag = "player", 
                KnockbackForce = 20f 
            });
        }
    }
}
