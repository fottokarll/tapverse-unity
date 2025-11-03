# TapVerse

TapVerse is a mobile-first idle clicker prototype built with Unity 2022 LTS. The project focuses on a short, punchy tap loop that feeds upgrades, generators, tier unlocks, and a prestige reset.

## Getting Started
- Install **Unity 2022.3.10f1** (or newer 2022 LTS patch) via Unity Hub.
- Clone the repository and open the `TapVerse` project folder in Unity Hub.
- Ensure the **Android Build Support** module is installed for mobile builds.
- Open the `Boot` scene and press **Play** to start from the bootstrapper.

### Loom-style Quick Guide
- **Play the Boot scene.** Systems bootstrap and transition to the main Game scene automatically.
- **Tap the core** in the center to generate Creation Energy (CE).
- **Buy upgrades and generators** using the bottom tabs to accelerate CE per tap and CE per second.
- **Reach tier milestones** to unlock new backdrops and music layers.
- **Open the Prestige tab** once lifetime CE is high enough, review the shard preview, and confirm to reset with permanent Creation Shards.

## Project Structure
```
Assets/
  Resources/            // ScriptableObject catalogs, runtime assets, audio, sprites
  Scenes/               // Boot, Game, DevSandbox scenes
  Scripts/
    Core/               // Core utilities (BigDouble, BootLoader, ServiceLocator, pooling)
    Gameplay/           // Game state, managers, save/prestige/economy systems
    UI/                 // Runtime UI construction and floating text logic
    Services/           // Audio, haptics, tuning helpers
  ScriptableObjects/    // Balance definitions (upgrades, generators, tiers)
  Tests/PlayMode/       // Runtime play mode tests (see below)
```

## Core Systems
- **GameManager** orchestrates bootstrapping, scene loading, and manager initialization.
- **CurrencyManager / UpgradeManager / GeneratorManager / TierManager / PrestigeManager** contain the core idle gameplay math and state.
- **SaveManager** persists encrypted JSON save data, handles offline gains (capped at 8 hours), and auto-saves every 10 seconds or on key events.
- **FeedbackManager** wires basic audio, haptic, particle, and floating-text feedback using a lightweight object pool.
- **AnalyticsRouter** logs gameplay events through a simple interface ready for SDK integrations.

## Tuning Cheatsheet
| System | Key Fields | Default Values |
|--------|------------|----------------|
| Tap Base | `CurrencyManager.baseTapValue` | 1 CE/tap |
| Crit | `CurrencyManager.CritChance`, `CritPower` | 3% / 1.5x |
| Generator 1 | `Generator_SparkHarvester` | 0.5 CE/s, cost 10 CE, growth 1.15 |
| Generator 2 | `Generator_OrbWeaver` | 5 CE/s, cost 200 CE, growth 1.15 |
| Upgrades | `ScriptableObjects/Upgrades/*` | 10 starter upgrades across tap, generator, crit, and auto-tap |
| Tiers | `TierCatalog` | Tier 0 (0 CE), Tier 1 (1K CE), Tier 2 (1M CE), Tier 3 (1B CE) |
| Prestige | `PrestigeManager` | Threshold 10,000 CE, shards ~sqrt(CE/threshold) |

Use `TapVerse.Services.TuningService.Export/Import` for quick JSON balance dumps. Exported files default to the `Tuning/` folder (ignored by Git).

## Building for Mobile
- **Android:** Switch the build target to Android (`File > Build Settings`). Ensure minimum SDK 23, target SDK 33. Configure your keystore before shipping.
- **iOS:** Switch to iOS in Build Settings and export an Xcode project. Requires macOS with Xcode 14+.
- Use the Boot scene as the build entry point.

## Tests
Play Mode tests live under `Assets/Tests/PlayMode/` and cover tap accrual, upgrade scaling, generator income, and prestige shard calculations. Run via **Test Runner > PlayMode**.

## QA Focus
- Validate tap responsiveness on both mouse click and touch simulation (Unity Remote) to ensure crit windows and feedback triggers.
- Confirm auto-save and load by exiting play mode after purchasing upgrades, then relaunching to verify state persistence and offline gains messaging.
- Exercise prestige flow from preview to confirmation to guarantee Creation Shards are granted and the run resets cleanly.
- Switch between all bottom navigation tabs to spot layout or data binding regressions, including disabled-state formatting.

## Roadmap TODOs
1. [ ] Add Limited-Time Event scaffold ("Solar Storm Week").
2. [ ] Add Cosmetics store for color themes & music packs (Creation Shards).
3. [ ] Add Daily Streak rewards and calendar UI.
4. [ ] Integrate native Haptics + Firebase Analytics behind existing interfaces.

---
Happy tapping!
