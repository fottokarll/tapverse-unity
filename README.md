# TapVerse — Unity 2022.3.62f3 LTS

TapVerse is a mobile-first idle clicker prototype focused on fast tap loops that feed upgrades, generators, tier unlocks, and prestige resets. The project targets Android first (iOS secondary) and is authored entirely in C#.

## Requirements
- Unity **2022.3.62f3** (LTS)
- IDE support via Visual Studio 2022 or JetBrains Rider
- Android Build Support modules (for device testing)

## First Open
1. Clone the repository and add the project folder to Unity Hub.
2. Launch with the 2022.3.62f3 editor and allow packages to import (UGUI is bundled).
3. Open the **Boot** scene under `Assets/Scenes/` and press **Play**.
4. The bootstrapper constructs persistent managers, loads save data, and transitions into the **Game** scene automatically.

## Smoke Test Checklist
Use this quick pass after pulling or upgrading Unity:
- [ ] Project loads without Safe Mode.
- [ ] Console compiles cleanly (warnings are acceptable).
- [ ] Game scene shows the procedural universe core and tab bar UI.
- [ ] Tapping increments Creation Energy and spawns floating text.
- [ ] Upgrades, generators, and prestige preview buttons respond.
- [ ] Procedural audio pings on taps/crit events.
- [ ] Exiting Play Mode leaves no stray `DontDestroyOnLoad` objects beyond the SystemsRoot.

## Project Structure
```
Assets/
  Resources/            // ScriptableObject catalogs for upgrades, generators, tiers
  Scenes/               // Boot, Game, DevSandbox scenes
  Scripts/
    Audio/              // Procedural sine-click clip factory
    Core/               // BigDouble math, events, pooling, service locator
    Gameplay/           // Managers (currency, upgrades, generators, tiers, prestige, save)
    Services/           // Audio/Haptics services (runtime singletons)
    UI/                 // HUD construction, floating text
    Visuals/            // Procedural sprite helpers & runtime setup
```

## Core Systems
- **BootLoader** spins up the persistent SystemsRoot, initializes managers, and feeds the Game scene.
- **GameManager** orchestrates bootstrap order, tier unlock flow, and scene transitions.
- **CurrencyManager / UpgradeManager / GeneratorManager / TierManager / PrestigeManager** maintain idle math, upgrade scaling, tier progression, and prestige rewards.
- **SaveManager** persists encrypted JSON saves, applies offline gains (capped at 8 hours), and autosaves on intervals/events.
- **FeedbackManager** centralizes audio, haptics, particles, and floating text responses via pooled instances.
- **AnalyticsRouter** fires gameplay events (`tap`, `purchase_upgrade`, `tier_unlock`, `prestige_*`, `session_*`) through an interface stub for future SDKs.

## Tuning Cheatsheet
| System | Key Fields | Default Values |
|--------|------------|----------------|
| Tap Base | `CurrencyManager.baseTapValue` | 1 CE/tap |
| Crit | `CurrencyManager.CritChance`, `CritPower` | 3% / 1.5x |
| Generator 1 | `Generator_SparkHarvester` | 0.5 CE/s, cost 10 CE, growth 1.15 |
| Generator 2 | `Generator_OrbWeaver` | 5 CE/s, cost 200 CE, growth 1.15 |
| Upgrades | `ScriptableObjects/Upgrades/*` | 10 starter upgrades across tap, generator, crit, autotap |
| Tiers | `TierCatalog` | Tier 0 (0 CE), Tier 1 (1K CE), Tier 2 (1M CE), Tier 3 (1B CE) |
| Prestige | `PrestigeManager` | Threshold 10,000 CE, shards ≈ sqrt(CE / threshold) |

Use `TapVerse.Services.TuningService.Export/Import` for quick JSON balance dumps. Exports land in the ignored `Tuning/` folder.

## Build Notes
- **Android:** Switch build target to Android (`File > Build Settings`), ensure minimum SDK 23 / target SDK 33, and configure your keystore for release builds.
- **iOS:** Switch to iOS, export an Xcode project, and build with Xcode 14+ on macOS.
- The **Boot** scene remains the first scene in Build Settings.

## Tests
Play Mode tests were removed temporarily to unblock compilation in 2022.3.62f3. Re-introduce them later behind `UNITY_INCLUDE_TESTS` once the suite is updated.

## QA Focus
- Validate tap responsiveness (mouse/touch simulation) including crit feedback and floating text pooling.
- Confirm auto-save/offline recovery by leaving Play Mode after purchases, relaunching, and verifying gains.
- Exercise prestige preview/confirmation to ensure Creation Shards grant and reset correctly.
- Navigate all bottom tabs to ensure states and disabled formatting persist.

## Roadmap TODOs
1. [ ] Add Limited-Time Event scaffold ("Solar Storm Week").
2. [ ] Add Cosmetics store for color themes & music packs (Creation Shards).
3. [ ] Add Daily Streak rewards and calendar UI.
4. [ ] Integrate native Haptics + Firebase Analytics behind existing interfaces.

Happy tapping!
