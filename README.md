# New_Game

## Overview

A **3D action-puzzle game core** (C#) inspired by **Sonic** with combat mechanics, **mouse-controlled camera**, and **multi-monitor damage feedback**. Engine-agnostic and designed to integrate into any 3D game engine (Unity, Godot, Unreal, custom engines, etc.).

### Features

- **3D Physics**: gravity, velocity, drag, kinematic bodies
- **Collision & Damage**: sphere-based detection, knockback, health system
- **Player Control**: movement, jumping, double jump, dashing
- **� Projectile System**: fireballs, homing shots, with automatic despawn
- **💰 Collectibles**: rings, coins, health pickups, power-ups (speed boost, invincibility)
- **🎮 Level Obstacles**: spikes, lava, moving platforms, conveyor belts, trampolines
- **🖱️ Mouse-Controlled Camera**: third-person follow with mouse look
- **🖱️ Mouse Lock**: locked to screen during gameplay, unlock with ESC
- **💥 Damage Feedback**: red screen flash on all monitors when hit (customizable)
- **🎨 Retro Graphics**: pixelated, color-reduced rendering like classic Sonic (multiple presets)
- **Enemy AI**: patrol → chase → attack state machine
- **Puzzle Interactions**: switches, levers, doors with callbacks
- **Component-based architecture**: ECS-style entity system

### Core files

| File | Purpose |
|------|---------|
| `Vector3.cs` | 3D math utilities (Lerp added) |
| `Components3D.cs` | Transform, Rigidbody, Health, Damage, Collision components |
| `CameraComponents.cs` | Camera, CameraController, ScreenFlashEffect, UILayer |
| `RetroGraphicsEffect.cs` | Pixelated/retro graphics component + presets (Genesis, Master System, Game Boy, etc.) |
| `GameplayComponents.cs` | Projectile, Collectible, PowerUp, Hazard, MovingPlatform, Trampoline, ConveyorBelt |
| `Physics3DSystem.cs` | Physics simulation + collision detection/response + damage flash + trampoline bounce |
| `CameraSystem.cs` | Camera follow logic + mouse input handling + mouse lock toggle |
| `ScreenFlashSystem.cs` | Damage feedback flash renderer |
| `RetroGraphicsSystem.cs` | Applies retro pixel effect post-processing |
| `GameplaySystem.cs` | ProjectileSystem, CollectibleSystem, HazardSystem, PlatformSystem, PowerUpSystem |
| `CharacterControllers.cs` | PlayerController (movement, jump, dash) + EnemyAI (patrol/chase/attack) |
| `GameSystems.cs` | PlayerInputSystem (now with firing), InteractionSystem, EnemyAISystem |
| `GameSetup.cs` | Complete game initialization: player, enemy, camera, collectibles, hazards, platforms |
| `Game.cs` | Main game loop & entity manager |
| `Entity.cs` | ECS entity with component storage |
| `IEngineRenderer.cs` | Engine bridge interfaces (mouse, flash, retro graphics, screen size) |
| `EngineBridgeExamples.cs` | Unity/Godot implementation examples |

## Quick start

```csharp
// 1. Bridge to your engine
var inputBridge = new UnityInputBridge();           // Implement IEngineInput
var rendererBridge = new UnityRendererBridge();     // Implement IEngineRenderer
var audioBridge = new UnityAudioBridge();           // Implement IEngineAudio

// 2. Initialize game
var game = new GameCore.Game(inputBridge);
GameSetup.SetupSonicLikeGame(game, inputBridge, rendererBridge, audioBridge);

// 3. In your engine's update loop
void EngineUpdate(float deltaTime)
{
    game.Update(deltaTime);
}
```

## Creating a player

```csharp
var player = game.CreateEntity();
player.AddComponent(new Tag("Player"));
player.AddComponent(new Transform3D { Position = Vector3.Zero });
player.AddComponent(new Rigidbody3D { UseGravity = true });
player.AddComponent(new CollisionSphere { Radius = 1.0f });
player.AddComponent(new Health(100));
player.AddComponent(new PlayerController 
{ 
    Speed = 15f, 
    JumpForce = 15f,
    MaxSpeed = 25f,
    CanDoubleJump = true 
});
player.AddComponent(new MeshRenderer { MeshId = "player", MaterialId = "mat_player" });
```

## Creating an enemy

```csharp
var enemy = game.CreateEntity();
enemy.AddComponent(new Tag("Enemy"));
enemy.AddComponent(new Transform3D { Position = new Vector3(10, 0, 0) });
enemy.AddComponent(new Rigidbody3D { UseGravity = true });
enemy.AddComponent(new CollisionSphere { Radius = 0.8f });
enemy.AddComponent(new Health(50));
enemy.AddComponent(new DamageSource { Damage = 10, SourceTag = "enemy" });
enemy.AddComponent(new EnemyAI 
{ 
    DetectionRange = 25f,
    ChaseSpeed = 12f 
});
```

## Creating a puzzle switch

```csharp
var puzzleSwitch = game.CreateEntity();
puzzleSwitch.AddComponent(new Transform3D { Position = new Vector3(0, 0, 10) });
puzzleSwitch.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
puzzleSwitch.AddComponent(new Interactable 
{ 
    OnInteract = (entity) => 
    {
        // Trigger custom behavior (open door, spawn enemy, etc.)
    }
});
```

## Camera system

The **CameraSystem** automatically:
- Follows the player entity with adjustable distance and height
- Updates rotation based on **mouse movement**
- **Locks mouse to screen** on startup (ESC to unlock for pause menu)
- Supports pitch/yaw clamping for natural camera feel

```csharp
// Create camera entity
var camera = game.CreateEntity();
camera.AddComponent(new Tag("Camera"));
camera.AddComponent(new Transform3D { Position = Vector3.Zero });
camera.AddComponent(new Camera { FieldOfView = 60f });
camera.AddComponent(new CameraController
{
    FollowTarget = player,           // Follow this entity
    Distance = 6f,                   // Distance behind player
    Height = 2f,                     // Height above player
    MouseSensitivity = 2f,           // Mouse look speed
    Pitch = -20f,                    // Initial pitch (degrees)
    MaxPitch = 60f,                  // Up limit
    MinPitch = -80f,                 // Down limit
    IsLockedToScreen = true          // Lock mouse at start
});
```

## Screen flash (damage feedback)

When the player takes damage, a **red overlay flashes across all monitors**:

```csharp
// Automatically triggered when player takes damage
// In CollisionSystem.TriggerDamageFlash():
var flash = new ScreenFlashEffect(
    duration: 0.2f,      // Fade over 0.2 seconds
    intensity: 0.7f      // 70% opacity at peak
)
{
    Color = "red"        // red, white, yellow, etc.
};
entity.AddComponent(flash);
```

**Engine integration required:**

```csharp
public void DrawScreenFlash(float intensity, string color = "red")
{
    // Render a full-screen quad or overlay
    // with color and opacity = intensity
    // This affects ALL monitors in a multi-monitor setup
    
    // Example (pseudo-code):
    // var overlay = new Quad(0 -> ScreenWidth, 0 -> ScreenHeight);
    // overlay.Color = color with alpha = intensity;
    // renderer.Draw(overlay);
}
```

## Retro Graphics System

Pixelated, color-reduced rendering like classic Sonic games. Multiple **presets** available:

### Presets

| Preset | Pixel Size | Colors | Scanlines | Style |
|--------|------------|--------|-----------|-------|
| **Sega Genesis** | 2x2 | 32K (5-bit) | Yes | Classic Sonic 3 |
| **Sega Master System** | 3x3 | 4K (4-bit) | Yes | Sonic 1 (Master System) |
| **Super Nintendo** | 2x2 | 32K (5-bit) | Yes | Similar to Genesis |
| **Arcade** | 4x4 | 4K (4-bit) | Yes | High contrast cabinet style |
| **Game Boy** | 4x4 | 4 colors | No | Monochrome handheld |
| **Minimal Pixel** | 2x2 | Full color | No | Clean pixelated look |

### Create with preset

```csharp
// Apply Sega Genesis style (default in GameSetup)
var retroEffect = game.CreateEntity();
retroEffect.AddComponent(RetroPresets.SegaGenesis);

// Or Master System style
retroEffect.AddComponent(RetroPresets.SegaMasterSystem);

// Or Game Boy style
retroEffect.AddComponent(RetroPresets.GameBoy);
```

### Custom retro effect

```csharp
var customEffect = new RetroGraphicsEffect
{
    PixelSize = 3,                  // 3x3 pixel blocks
    ColorBitsPerChannel = 4,        // Reduce to 4-bit color (4096 colors)
    EnableScanlines = true,         // CRT scanline effect
    ScanlineOpacity = 0.4f,         // 40% scanline darkness
    EnableDithering = true,         // Smooth color transitions
    EnableAspectCorrection = true   // Fix aspect ratio
};

entity.AddComponent(customEffect);
```

### Engine integration

The renderer must implement `ApplyRetroGraphics()`:

```csharp
public void ApplyRetroGraphics(RetroGraphicsEffect effect)
{
    // Pseudocode (implementation varies by engine)
    
    // 1. Create downsample render target
    var pixelated = DownsampleToPixels(
        renderTarget, 
        effect.PixelSize
    );
    
    // 2. Reduce color palette
    if (effect.ColorBitsPerChannel > 0)
        pixelated = ReduceColorPalette(
            pixelated, 
            effect.ColorBitsPerChannel
        );
    
    // 3. Apply dithering for smoother gradients
    if (effect.EnableDithering)
        pixelated = ApplyDithering(pixelated);
    
    // 4. Upscale back to screen resolution
    var upscaled = UpscaleNearest(pixelated, GetScreenWidth(), GetScreenHeight());
    
    // 5. Apply scanlines
    if (effect.EnableScanlines)
        upscaled = AddScanlines(upscaled, effect.ScanlineOpacity);
    
    // 6. Display
    RenderToScreen(upscaled);
}
```

## Input handling

Updated **IEngineInput** interface includes mouse support:

```csharp
public interface IEngineInput
{
    bool IsKeyDown(string key);           // "Space", "Shift", "E", "Escape"
    float GetAxis(string axis);           // "Horizontal", "Vertical"
    float GetMouseX();                    // Screen X position
    float GetMouseY();                    // Screen Y position
    void LockMouse(bool locked);          // Lock/unlock cursor
    bool IsMouseLocked { get; }           // Check if locked
}
```

**ESC key** toggles mouse lock for pause menu:
- **Locked** (default): Mouse hidden, camera follows, no cursor movement
- **Unlocked**: Cursor visible, can interact with UI

## Creating a camera

```csharp
var camera = game.CreateEntity();
camera.AddComponent(new Tag("Camera"));
camera.AddComponent(new Transform3D { Position = new Vector3(0, 2, -5) });
camera.AddComponent(new Camera { FieldOfView = 60f });
camera.AddComponent(new CameraController
{
    FollowTarget = player,
    Distance = 6f,
    Height = 2f,
    MouseSensitivity = 2f
});
```

## Engine bridge example (Unity)

```csharp
public class UnityInputBridge : IEngineInput
{
    private bool _mouseLocked = true;

    public bool IsKeyDown(string key) => Input.GetKey(key);
    
    public float GetAxis(string axis) => Input.GetAxis(axis);
    
    public float GetMouseX() => Input.mousePosition.x;
    
    public float GetMouseY() => Input.mousePosition.y;
    
    public void LockMouse(bool locked)
    {
        _mouseLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
    
    public bool IsMouseLocked => _mouseLocked;
}

public class UnityRendererBridge : IEngineRenderer
{
    private Color _flashColor;
    private float _flashIntensity;

    public void DrawSprite(string spriteId, float x, float y, float rot, 
        float scaleX = 1, float scaleY = 1, float opacity = 1)
    {
        // Draw 3D mesh at position
    }
    
    public void DrawScreenFlash(float intensity, string color = "red")
    {
        // Render full-screen overlay on all monitors
        _flashIntensity = intensity;
        _flashColor = color == "red" ? Color.red : Color.white;
        // Draw overlay rect covering screen space
    }
```

## Projectile system

**Rocks are thrown when dashing** — Player automatically throws rocks while sliding forward:

```csharp
// When player dashes (Shift + direction):
// - Rock spawns in front of player
// - Travels at 25 units/second for 7 seconds
// - Deals 15 damage + 20 knockback on hit
// - Inherits player's momentum

// In PlayerController.Dash():
playerController.OnDash = (playerEntity, dashDirection) =>
{
    SpawnDashRock(game, playerEntity, dashDirection);
};
```

Rock properties:
- **Speed**: 25 units/sec (inherits dash velocity)
- **Lifetime**: 7 seconds (auto-despawn)
- **Damage**: 15 HP
- **Knockback**: 20 force units
- **Size**: 0.5 radius sphere
- **Auto-spawn**: One rock per dash

## Collectibles

Automatically picked up when player is within 2m:

```csharp
// Ring (health + points)
var ring = game.CreateEntity();
ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, value: 10));
ring.AddComponent(new Transform3D { Position = new Vector3(5, 1, 0) });
ring.AddComponent(new CollisionSphere { Radius = 0.3f, IsTrigger = true });

// Speed boost power-up (10 second duration)
var speedItem = game.CreateEntity();
speedItem.AddComponent(new Collectible(Collectible.CollectibleType.SpeedBoost, value: 10f));

// Health restoration
var healthPickup = game.CreateEntity();
healthPickup.AddComponent(new Collectible(Collectible.CollectibleType.HealthPickup, value: 25));
```

Types:
- **Coin**: Points only
- **Ring**: Points + partial healing
- **HealthPickup**: Restore health
- **SpeedBoost**: 1.5x speed for duration
- **Shield**: Invincibility for duration
- **Invincibility**: Damage immunity for duration

## Level obstacles & hazards

### Hazards

Damage player on contact:

```csharp
// Spikes (20 damage)
var spikes = game.CreateEntity();
spikes.AddComponent(new Hazard(Hazard.HazardType.Spike, damage: 20));
spikes.AddComponent(new Transform3D { Position = Vector3.Zero });
spikes.AddComponent(new CollisionSphere { Radius = 1.0f, IsTrigger = true });
```

Types: Spike, Lava, Pit, Electricity, Freeze

### Moving platforms

Elevators, sliding platforms:

```csharp
var platform = game.CreateEntity();
platform.AddComponent(new MovingPlatform(
    startPosition: new Vector3(0, 0, 0),
    endPosition: new Vector3(10, 0, 0),
    speed: 5f
));
platform.AddComponent(new Transform3D { Position = Vector3.Zero });
platform.AddComponent(new Rigidbody3D { IsKinematic = true });
platform.AddComponent(new CollisionSphere { Radius = 2.0f });
```

### Conveyor belts

Push entities along direction:

```csharp
var conveyor = game.CreateEntity();
conveyor.AddComponent(new ConveyorBelt(
    direction: Vector3.Right,
    speed: 8f
));
```

### Trampolines

Bounce entities upward:

```csharp
var trampoline = game.CreateEntity();
trampoline.AddComponent(new Trampoline(bounceForce: 40f));
trampoline.AddComponent(new CollisionSphere { Radius = 1.0f });
```

## Power-ups

Temporary ability modifications:

```csharp
var speedPowerUp = new PowerUpEffect(PowerUpEffect.PowerUpType.SpeedBoost, duration: 10f);
player.AddComponent(speedPowerUp);
```

Types:
- **SpeedBoost**: 1.5x speed multiplier
- **Invincibility**: Damage immunity
- **DoubleJump**: Extra jump ability
- **Slow**: Reduced speed (debuff)

## Audio system

Bridge interface for sound playback:

```csharp
public interface IEngineAudio
{
    void PlaySound(string soundId);   // "collect_ring", "hazard_spike"
    void StopSound(string soundId);
}

// Sounds auto-triggered:
// - collect_coin, collect_ring, collect_healthpickup
// - hazard_spike, hazard_lava, hazard_pit, etc.
// - player_hit, enemy_defeat
```

## Controls summary

| Input | Action |
|-------|--------|
| **WASD** | Move |
| **Space** | Jump (hold = double jump) |
| **Shift + Direction** | **Dash forward & throw rock** |
| **Mouse** | Camera look |
| **ESC** | Toggle mouse lock (pause) |
| **E** | Interact with puzzles |

## Roadmap

- [x] 3D physics & collision
- [x] Camera system with mouse look
- [x] Mouse lock for pause menu
- [x] Screen flash on all monitors (damage feedback)
- [x] Retro pixelated graphics (classic Sonic style)
- [x] Projectile/weapon system
- [x] Collectibles & power-ups
- [x] Level obstacles (hazards, platforms, trampolines, conveyors)
- [ ] Audio system integration
- [ ] UI (health bar, score, menu)
- [ ] Save/Load game state
- [ ] Networking (multiplayer)
- [ ] Unit tests

