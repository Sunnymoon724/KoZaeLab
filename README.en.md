# KoZae's Code Lab

**English** | **[한국어](README.ko.md)**

A Unity 6 research and development project that organizes game development code into **KoZaeLibrary (KZLib)** and validates it in a real project environment.

[![License: MIT](https://img.shields.io/badge/License-MIT-blueviolet.svg)](LICENSE)
[![Unity Version](https://img.shields.io/badge/Unity-6000.3.7f1-important)](https://unity3d.com/)

---

## Table of Contents

- [Overview](#overview)
- [Getting Started](#getting-started)
- [KoZaeLibrary (KZLib)](#kozealibrary-kzlib)
- [Architecture](#architecture)
- [Samples](#samples)
- [Configuration Files](#configuration-files)
- [Editor Tools (KZMenu)](#editor-tools-kzmenu)
- [Optional Modules](#optional-modules)
- [Dependencies](#dependencies)
- [Related Repositories](#related-repositories)

---

## Overview

| Item | Details |
|------|---------|
| **Unity Version** | 6000.3.7f1 (Unity 6.3) |
| **Render Pipeline** | URP (Universal Render Pipeline) |
| **Core Package** | `com.bsheepstudio.kzlib` v1.1.17 |
| **License** | MIT |

This repository serves as a **lab** for developing and validating the KZLib library. The library itself can be added to other projects via a UPM Git URL.

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

### Scripts Folder Structure

```
Assets/KZLib/Scripts/
├── Data/          Config, Proto, Lingo, Facet, Cluster
├── Framework/     Actor, Stanza, Pool, RosterMapper, Presentation, ReactivePrefs
├── Global/        Constants, shared types, enums
├── Inspector/     KZ* Odin Inspector attribute definitions
├── Main/          BaseMain, ResolutionMonitor
├── OnlyEditor/    AttributeDrawer, KZMenu, EditorWindow, PathCreator, Drawing
├── Platform/      Network, PlayFab, Webhook, InAppPurchase
├── Runtime/       Wrapper UGUI components, TimeFlow, Enchanted
├── Shared/        Shared C# shims (e.g. init-only)
├── System/        Input, Sound, Graphic, Scene, Resource, Context, CutScene, ...
├── UI/            UIManager, Window, CommonWindow, UI modules (Carousel/Accordion/FocusScroller)
└── Utility/       Extension, Kit, Log
```

---

## Getting Started

### 1. Requirements

- **Unity 6000.3.7f1** or later

#### Asset Store

| Package | Link |
|---------|------|
| DOTween Pro | [Asset Store](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) |
| Odin Inspector | [Asset Store](https://assetstore.unity3d.com/packages/tools/utilities/odin-inspector-and-serializer-89041) |

#### Git (UPM)

Package Manager → **Add package from git URL**:

| Package | GitHub |
|---------|--------|
| UniTask | [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) |
| UIEffect | [GitHub](https://github.com/mob-sakai/UIEffect?tab=readme-ov-file#-installation) |
| SoftMaskForUGUI | [GitHub](https://github.com/mob-sakai/SoftMaskForUGUI?tab=readme-ov-file#-installation) |
| ParticleEffectForUGUI | [GitHub](https://github.com/mob-sakai/ParticleEffectForUGUI?tab=readme-ov-file#-installation) |
| NuGetForUnity | [GitHub](https://github.com/GlitchEnzo/NuGetForUnity) |

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
https://github.com/mob-sakai/UIEffect.git?path=Packages/src
https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src
https://github.com/mob-sakai/ParticleEffectForUGUI.git
https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity
```

> After installing NuGetForUnity, NuGet packages defined in `Assets/KZLib/packages.config` (MessagePipe, MemoryPack, etc.) are restored automatically.

### 2. Open the Project

1. Open this repository in Unity Hub.
2. After the first import, go to **Window → Package Manager** and import `KoZaeLibrary` samples.
3. Open **KZMenu → Scene → Open Main Scene** or any sample scene.

### 3. Add KZLib to Another Project

Package Manager → **Add package from git URL**:

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

---

## KoZaeLibrary (KZLib)

### Package Info

- **Name:** `com.bsheepstudio.kzlib`
- **Display Name:** KoZaeLibrary
- **Version:** 1.1.17
- **Author:** Ko KoZae

### Main Script Modules

| Module | Description |
|--------|-------------|
| **Main** | `BaseMain` — app entry point, Test/Normal play mode, language/resolution |
| **Framework/Actor** | `Actor`, `Unit`, `Structure` — state/stat/buff/team relation framework |
| **Framework** | Stanza(animation), Pool(`GameObjectPawnPool`), `RosterMapper`, Presentation, `ReactivePrefs` |
| **UI** | `UIManager`, Window(2D/3D), CommonWindow (Loading/Download/Video/Transition, etc.), Carousel·CarouselNavigator·Accordion·FocusScroller |
| **System** | Input, Sound, Graphic, Effect, Camera, Scene, Resource, Context, CutScene, DebugOverlay, etc. |
| **Data** | Config(YAML), Proto(MemoryPack), Lingo, Facet, Cluster |
| **Platform** | PlayFab, Network, Webhook(Discord/Trello/Google), InAppPurchase |
| **Runtime** | Wrapper UGUI components, `TimeFlow`(per-object time scale), Enchanted |
| **Inspector** | `KZ*` Odin attributes (paths, HexColor, Clamp, List, etc.) |
| **OnlyEditor** | AttributeDrawer, KZMenu, EditorWindow, PathCreator, Drawing |
| **Utility** | Extension, Kit, Log |

### Runtime Components (partial)

- **UI:** Button, Toggle, ScrollRect(Reuse/Pager), ReuseGridLayoutGroup, Dropdown, Image(Shader/Graph), TMP, RawImage
- **Camera:** ObserveCamera
- **Particle:** ForceMoveParticleSystem
- **Other:** TimeFlow, EnchantedContentSizeFitter, FollowTargetWatcher

### OnlyEditor Drawing (partial)

- LineDrawing, ShapeDrawing, GraphicDrawing — UGUI drawing components and editors

---

## Architecture

### App Bootstrap

```
BaseMain (subclass implementation)
  → ConfigManager (YAML load)
  → RouteManager (path resolution)
  → GraphicManager / LingoManager (ReactivePrefs user settings)
  → ResourceManager / AddressablesManager
  → UIManager, InputManager, SoundManager ...
```

In editor **Test mode**, `TestModeConfig` scene transient data lets you skip TitleScene and start directly from a configured scene.

### User Settings (ReactivePrefs)

The `Data/Tune` layer (`TuneManager`, `LanguageTune`, `SoundTune`, `GraphicTune`, `NativeTune`) was removed. Each domain manager now owns PlayerPrefs directly via `ReactivePrefs<T>`.

| Manager | Settings | Description |
|---------|----------|-------------|
| `LingoManager` | `Language` | Applies Unity Localization locale |
| `GraphicManager` | `Resolution`, `FrameRate`, `GraphicQuality` | Screen, FPS, QualitySettings, camera far clip |
| `SoundManager` | `SoundProfile` | Master, music, and effect volumes |
| `VibrationManager` | `UseVibration` | Vibration on/off |
| `PushManager` | `UseNotification`, `UseNightNotification` | Local push and night-time notification toggles |

`ReactivePrefs<T>` exposes changes through R3 `Observable` and persists a default when the key is missing or parsing fails. The `GraphicQualityOption` ScriptableObject moved to `System/Graphic`.

### Event Bus (MessagePipe)

MessagePipe is configured during `LogChannel` initialization. It delivers cross-system events such as resolution changes, input, log display, and Badge/Unlock notifications via pub/sub.

### Facet (Server-Synced State)

```
NetworkManager → FacetManager.Apply<TFacet>() → game code Get/TryGet
```

`FacetManager` caches `IFacet` payloads from the server in a session store. Client-side local settings use each manager's `ReactivePrefs`.

### Pool & RosterMapper

```
data roster  →  RosterMapper<TItem,UData>  →  GameObjectPawnPool<T>  →  active views
```

- `GameObjectPawnPool<T>` — `GameObjectPawn`-backed component pool (Get/Put, storage parent)
- `RosterMapper<TItem,UData>` — binds roster length/order to pooled views (`TrySetDataList`)
- **Non-virtualized UI:** `Accordion`, `Carousel`, `CarouselNavigator` (`CarouselDot` page indicators)
- **Virtualized UI:** `ReuseScrollRect`, `ReuseGridLayoutGroup`

### Context (Badge / Unlock)

`ContextManager` discovers `IContentProvider` implementations in the game project via reflection and manages content unlock state and badge (red-dot) counts.

### Manager Lifecycle

`KZGameKit.ReleaseManager()` releases all singleton managers at once.  
Includes SceneState, UI, Input, Effect, Sound, Graphic, Proto, Config, Cluster, Facet, Lingo, Addressables, Network, Webhook, and more.

### Data Flow

```
Excel (ClosedXML)  →  YAML Config / Lingo ScriptableObject
MemoryPack           →  Proto binary (.bytes)
Route.yaml           →  Resources / Addressables path resolution
Game.yaml            →  local/server resources, save mode, HUD flags
TestMode.yaml        →  editor Test mode per-scene transient data
```

---

## Samples

Import from Package Manager → KoZaeLibrary → Samples:

| Sample | Description |
|--------|-------------|
| **MainSample** | `BaseMain` subclass, TitleScene flow |
| **DebugOverlaySample** | FPS and debug overlay panel |
| **CarouselSample** | Carousel, CarouselNavigator, and CarouselDot UI demo |
| **FocusScrollerSample** | Focus-based scroll list |
| **AccordionSample** | Expand/collapse accordion UI (`RosterMapper`-based) |
| **ToggleSample** | Custom Toggle Mount |
| **ReuseScrollRectSample** | Virtualized (recycled) ScrollRect |
| **StanzaSample** | Stanza Lerp and animation utilities |
| **VibrationSample** | Native vibration API |

---

## Configuration Files

### Runtime (`Assets/Resources/Text/Config/`)

| File | Purpose |
|------|---------|
| `Game.yaml` | Proto/Language/Config/UI/FX paths, `IsLocalResource`, `IsLocalSave`, HUD |
| `Webhook.yaml` | Discord Webhook URLs (BugReport, etc.) |

### Editor (`Assets/WorkResources/Text/Config/`)

| File | Purpose |
|------|---------|
| `TestMode.yaml` | Test mode per-scene `ISceneTransient` payload (`SceneTransientDict`) |

### Paths (`Assets/Resources/Text/Setting/`)

| File | Purpose |
|------|---------|
| `Route.yaml` | Named route → folder mapping |

### Config System

- `ConfigManager` deserializes `*Config` classes via YamlDotNet (including `TestModeConfig`)
- **KZMenu → Config → Generate All Config** generates YAML from Excel (ClosedXML)
- Separate YAML generation menus for Game, Webhook, and TestMode, with custom options

---

## Editor Tools (KZMenu)

Available from the Unity **KZMenu**:

| Section | Features |
|---------|----------|
| **Option** | Delete managers, clean empty folders, unload unused assets, find missing components/mesh filters, add PlayFab/IAP modules, check internet |
| **Explorer** | Open route file, Documents, Tools, DataPath, PersistentDataPath |
| **Config** | Excel → YAML generation (Game/Webhook/TestMode, custom), open config folders |
| **Lingo** | Excel → Localization (String/Asset) + Addressables registration |
| **Proto** | Batch proto generation, Proto window |
| **Window** | Manual, Editor Custom, PlayerPrefs, Graphic Quality, Network Test (Webhook) |
| **Scene** | Open Main Scene |

### Unity Built-in Context Menu Extensions (DefaultMenuItem)

| Area | Features |
|------|----------|
| **Assets/KZSubMenu** | Script → create ScriptableObject, open Texture/ScriptableObject, Prefab mesh name lookup |
| **Hierarchy** | Copy Hierarchy, Create Category Line |
| **GameObject** | Path Creator, UI (Empty Panel, Shape, Carousel) |

### Editor Windows

ManualWindow, EasingGraphWindow, PixelEditorWindow, MeshFinderWindow, ProtoWindow, ResourceWindow, WebhookTestWindow (Network Test menu; Discord/Trello/Google), PlayerPrefsWindow, EditorCustom, and more.

---

## Optional Modules

Toggle features via compile symbols. Add via **KZMenu → Option**:

| Symbol | Feature |
|--------|---------|
| `KZLIB_PLAY_FAB` | PlayFab Client API (login, profile, CloudScript) |
| `KZLIB_IN_APP_PURCHASE` | Unity Purchasing (IAP) |

The PlayFab SDK is included at `Assets/PlayFabSDK/`. KZLib wrappers primarily use Client and CloudScript APIs.

---

## Dependencies

### Required Unity Asset Store

| Package | Link |
|---------|------|
| DOTween Pro | [Asset Store](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) |
| Odin Inspector | [Asset Store](https://assetstore.unity3d.com/packages/tools/utilities/odin-inspector-and-serializer-89041) |

### Required Git (UPM)

| Package | GitHub |
|---------|--------|
| UniTask | [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) |
| UIEffect | [GitHub](https://github.com/mob-sakai/UIEffect?tab=readme-ov-file#-installation) |
| SoftMaskForUGUI | [GitHub](https://github.com/mob-sakai/SoftMaskForUGUI?tab=readme-ov-file#-installation) |
| ParticleEffectForUGUI | [GitHub](https://github.com/mob-sakai/ParticleEffectForUGUI?tab=readme-ov-file#-installation) |
| NuGetForUnity | [GitHub](https://github.com/GlitchEnzo/NuGetForUnity) |

### NuGet (`Assets/KZLib/packages.config`)

| Package | Purpose |
|---------|---------|
| MessagePipe | Pub/sub event bus |
| MemoryPack | Proto binary serialization |
| YamlDotNet | YAML config deserialization |
| ClosedXML | Excel → Config/Lingo conversion |
| MoonSharp | Lua scripting (optional) |
| R3 | Reactive Extensions |
| ZXing.Net | QR/barcode |

### KZLib UPM Dependencies (`package.json`)

Newtonsoft JSON, Addressables, Localization, Input System, URP

---

## Related Repositories

- [KoZaeLibrary](https://github.com/Sunnymoon724/KoZaeLibrary)

---

## PlayFab (Optional)

When `KZLIB_PLAY_FAB` is defined:

- `PlayFabManager` — Guest/Android Device login, profile, CloudScript
- `NetworkManager` — routes server requests through CloudScript, applies Facet payloads
- Symbol is set by default on Android build target

The full SDK (Admin, Economy, Multiplayer, etc.) is included, but KZLib wrappers focus on the **Client API**.

---

## License

MIT License — see [LICENSE](LICENSE)
