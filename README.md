# Enemy Wave Survival

A small top-down 2D survival prototype built in Unity. Survive against continuously spawning enemies that get harder over time, and try to beat your best score.

Built as a technical assignment, with the focus on code structure, Unity best practices, and basic optimization rather than visual polish. All visuals are placeholders.

**Unity version:** 6000.3.8f1 (2D URP template)

---

## How to run

1. Open the project in Unity **6000.3.8f1** via Unity Hub.
2. Open the main scene from `Assets/MAIN/Scenes/Game
3. Press **Play**.

## Controls

| Input | Action |
|---|---|
| **W A S D** | Move |
| **Mouse** | Aim (player rotates toward the cursor) |
| **Hold Left Mouse Button** | Shoot |

On death, the game freezes and a Game Over panel shows your final score. Click **Restart** to play again.

---

## Features

**Player**
- Smooth movement with acceleration, diagonal speed clamped
- Mouse aim using screen-to-world conversion
- Hold-to-fire shooting with a cooldown

**Enemies** (two types, defined as `EnemyData` ScriptableObject assets)
- **Fast and weak:** low health, high speed
- **Slow and tanky:** high health, low speed, higher contact damage
- Chase the player and deal contact damage on a cooldown

**Game loop**
- Enemies spawn just outside the camera view, in a ring around the player
- Difficulty ramps over time: spawn interval shrinks and the share of tanky enemies grows
- Score from kills, survival timer, Game Over screen, restart

**UI**
- Player HP bar and numeric HP
- Score
- Survival time

**Camera and background**
- Camera follows the player
- A tiled grid background is generated in code and snapped to the camera in whole-tile steps, so the world looks endless without importing any assets

---

## Project structure

```
Assets/
  MAIN/
    _Scripts/
      Core/       GameManager, GameState, HealthComponent, IDamageable, InputReader
      Player/     PlayerController, PlayerShooter, Projectile
      Enemy/      EnemyController, EnemySpawner, ContactDamage
      UI/         HUDController, GameOverView
      World/      CameraFollow, BackgroundSnap
    Data/
      Input/      Player InputAction assest
      ScriptableObjects/   EnemyData assets (Fast, Tank)     
    Prefabs/      Player, Projectile, Enemy
    Scenes/
```
---

## Architecture notes

**Composition over inheritance.** Behavior is split into small, single-purpose components. There are no deep class hierarchies. The same `HealthComponent` is used by both the player and the enemies.

**Event-driven communication.** Systems announce what happened, and others react, so scripts stay decoupled:

| Event | Raised by | Used by |
|---|---|---|
| `OnDamageTaken(current, max)` | `HealthComponent` | HUD health bar |
| `OnDeath` | `HealthComponent` | `GameManager` (player), `EnemyController` (enemies) |
| `OnShot` | `PlayerShooter` | Ready for SFX hooks |
| `OnEnemyDied(EnemyData)` | `EnemyController` | `EnemySpawner` |
| `OnEnemyKilled(EnemyData)` | `EnemySpawner` | `GameManager` (score) |
| `OnScoreChanged(int)` | `GameManager` | HUD |
| `OnGameOver` | `GameManager` | Game Over panel |

Subscriptions are always paired in `OnEnable` / `OnDisable`.

**Data-driven enemies (ScriptableObjects).** `EnemyData` holds health, speed, contact damage, score value, and color. Both enemy types share one prefab and one script, and differ only by their data asset. Fields are `[SerializeField] private` with read-only properties, so runtime code cannot accidentally overwrite the shared asset.

**Interface for damage.** Anything implementing `IDamageable` can be hurt. Projectiles and contact damage never need to know whether they hit the player or an enemy, so new damage sources or targets (a lava wall, a boss) can be added without touching existing code.

**Shared input.** `InputReader` is the only script that touches the generated Input System class. Gameplay scripts read values from it, and disabling it on death stops both movement and shooting.

**Physics layers instead of code checks.** `Player`, `Enemy`, and `PlayerProjectile` layers with a collision matrix mean projectiles never collide with the player or each other, and enemies' contact damage is filtered by a `LayerMask`.

**Time-stamp cooldowns.** Fire rate, contact damage, and projectile lifetime all compare `Time.time` against a stored target time. This avoids coroutines, can be restarted cleanly, and works with pooling.

---

## Optimization

- **Object pooling** with Unity's built-in `UnityEngine.Pool.ObjectPool<T>` for both projectiles and enemies, so there is no per-shot or per-spawn allocation during play.
  - Objects announce when they are finished (`OnDespawn` / `OnEnemyDied`), and the pool owner releases them. Objects never destroy themselves.
  - Subscriptions to pooled objects happen once, at creation, to avoid duplicate event handlers on reuse.
  - `Despawn` is idempotent, so a hit and a lifetime timeout in the same frame cannot release an object twice.
  - Every `Initialize` method fully resets reused state.
- References are cached in `Awake`. There are no `Find` or `GetComponent` calls inside `Update` loops.
- The HUD timer only rebuilds its string when the displayed second changes.
- Physics-based movement uses `Rigidbody2D` with interpolation. Bullets set their velocity once instead of being moved each frame.
- The background is a single tiled sprite that follows the camera, instead of a large oversized image. Its texture is created at runtime and destroyed on cleanup.
- Pooled objects are parented under container objects to keep the hierarchy readable.

---

## Assignment checklist

| Requirement | Status |
|---|---|
| Keyboard movement, smooth, rotation toward mouse | Done |
| Projectile shooting with cooldown | Done |
| Two enemy types (fast/weak, slow/tanky) | Done |
| Enemies spawn over time, chase, damage on contact | Done |
| Player and enemy health, death handling | Done |
| Increasing difficulty, one level | Done |
| Score, Game Over screen, restart | Done |
| UI: HP, score, survival time | Done |
| Health bar UI (optional) | Done |
| Object pooling (bonus) | Done |
| ScriptableObjects (bonus) | Done |
| Event-driven architecture (bonus) | Done |
| Basic VFX/SFX (bonus) | Not included |
| State machine (bonus) | Not included (simple `GameState` enum only) |
| Mobile-friendly architecture (bonus) | Not included |

## Constraints followed

- Placeholder assets only. Sprites are basic shapes, and the background grid is generated in code.
- No third-party frameworks. Only Unity packages (Input System, TextMeshPro, 2D URP).

---

## Possible improvements

- SFX and VFX hooked to `OnShot` and enemy death events
- Wave-based spawning with a visible wave counter
- A cap on simultaneous enemies
- Pickups and weapon upgrades
- A full game state machine (menu, playing, paused, game over)
- Gamepad support, which `InputReader` is already structured for
