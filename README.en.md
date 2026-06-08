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
| **Core Package** | `com.bsheepstudio.kzlib` v1.1.1 |
| **License** | MIT |

This repository serves as a **lab** for developing and validating the KZLib library. The library itself can be added to other projects via a UPM Git URL.

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
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

#### Git

Package Manager → **Add package from git URL**:

| Package | GitHub |
|---------|--------|
| UniTask | [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) |
| UIEffect | [GitHub](https://github.com/mob-sakai/UIEffect?tab=readme-ov-file#-installation) |
| SoftMaskForUGUI | [GitHub](https://github.com/mob-sakai/SoftMaskForUGUI?tab=readme-ov-file#-installation) |
| ParticleEffectForUGUI | [GitHub](https://github.com/mob-sakai/ParticleEffectForUGUI?tab=readme-ov-file#-installation) |

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
https://github.com/mob-sakai/UIEffect.git?path=Packages/src
https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src
https://github.com/mob-sakai/ParticleEffectForUGUI.git
```

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
- **Version:** 1.1.1
- **Author:** Ko KoZae

### Main Script Modules

| Module | Description |
|--------|-------------|
| **Main** | `BaseMain` — app entry point, language/resolution/init modes |
| **Actor** | `Actor`, `Unit`, `Structure` — state/stat/buff/team relation framework |
| **UI** | `UIManager`, Window(2D/3D), common panels, Carousel/Accordion/FocusScroller |
| **System** | Input, Sound, Effect, Camera, Scene, Resource, Addressables, Debug managers |
| **Data** | Config(YAML), Proto(MemoryPack), Lingo, Tune, Affix, Cluster |
| **Network** | `NetworkManager` — PlayFab-integrated server requests |
| **Platform** | PlayFab, Webhook(Discord/Trello/Google), InAppPurchase |
| **GameTime** | Game time and time scaling |
| **Utility** | Extension, Kit, Stanza(animation), ObjectPool, Log |
| **InEditor** | Custom UGUI components, Odin attributes, editor windows/menus |
| **Develop** | PathCreator, GraphicQualityOption |

### InEditor Components (partial)

- **UI:** Button, Toggle, ScrollRect(Reuse/Pager), Dropdown, Image(Shader/Graph), TMP
- **Camera:** ObserveCamera
- **Particle:** ForceMoveParticleSystem
- **Drawing:** LineDrawing, ShapeDrawing, GraphicDrawing

---

## Architecture

### App Bootstrap

```
BaseMain (subclass implementation)
  → TuneManager (language/sound/graphics settings)
  → ConfigManager (YAML load)
  → RouteManager (path resolution)
  → ResourceManager / AddressablesManager
  → UIManager, InputManager, SoundManager ...
```

### Manager Lifecycle

`KZGameKit.ReleaseManager()` releases all singleton managers at once.  
Includes SceneState, UI, Input, Effect, Sound, Proto, Config, Lingo, Addressables, Network, Webhook, and more.

### Data Flow

```
Excel (ClosedXML)  →  YAML Config / Lingo ScriptableObject
MemoryPack           →  Proto binary (.bytes)
Route.yaml           →  Resources / Addressables path resolution
Game.yaml            →  local/server resources, save mode, HUD flags
```

---

## Samples

Import from Package Manager → KoZaeLibrary → Samples:

| Sample | Description |
|--------|-------------|
| **MainSample** | `BaseMain` subclass, TitleScene flow |
| **DebugOverlaySample** | FPS and debug overlay panel |
| **FocusScrollerSample** | Focus-based scroll list |
| **AccordionSample** | Expand/collapse accordion UI |
| **ToggleSample** | Custom Toggle Mount |
| **ReuseScrollRectSample** | Virtualized (recycled) ScrollRect |
| **StanzaSample** | Stanza Lerp and animation utilities |
| **VibrationSample** | Native vibration API |

> `Samples~/CarouselSample` also exists but is not registered in the `package.json` samples list.

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
| `Editor.yaml` | Editor-only parameter path dictionary |

### Paths (`Assets/Resources/Text/Setting/`)

| File | Purpose |
|------|---------|
| `Route.yaml` | Named route → folder mapping |

### Config System

- `ConfigManager` deserializes `*Config` classes via YamlDotNet
- **KZMenu → Config → Generate All Config** generates YAML from Excel (ClosedXML)

---

## Editor Tools (KZMenu)

Available from the Unity **KZMenu**:

| Section | Features |
|---------|----------|
| **Option** | Delete managers, clean empty folders, unload unused assets, add PlayFab/IAP modules |
| **Explorer** | Open route file, Documents, DataPath, PersistentDataPath, VS Code integration |
| **Config** | Excel → YAML generation, open config folders |
| **Lingo** | Excel → Localization + Addressables registration |
| **Proto** | Batch proto generation, Proto window |
| **Window** | Manual, PlayerPrefs, Graphic Quality, Network/Webhook Test |
| **Scene** | Open Main Scene |

### Editor Windows

ManualWindow, EasingGraphWindow, PixelEditorWindow, MeshFinderWindow, ProtoWindow, ResourceWindow, WebhookTestWindow (Discord/Trello/Google), and more.

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

### Required Git

| Package | GitHub |
|---------|--------|
| UniTask | [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) |
| UIEffect | [GitHub](https://github.com/mob-sakai/UIEffect?tab=readme-ov-file#-installation) |
| SoftMaskForUGUI | [GitHub](https://github.com/mob-sakai/SoftMaskForUGUI?tab=readme-ov-file#-installation) |
| ParticleEffectForUGUI | [GitHub](https://github.com/mob-sakai/ParticleEffectForUGUI?tab=readme-ov-file#-installation) |

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
https://github.com/mob-sakai/UIEffect.git?path=Packages/src
https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src
https://github.com/mob-sakai/ParticleEffectForUGUI.git
```

---

## Related Repositories

- [KoZaeLibrary](https://github.com/Sunnymoon724/KoZaeLibrary)

---

## PlayFab (Optional)

When `KZLIB_PLAY_FAB` is defined:

- `PlayFabManager` — Guest/Android Device login, profile, CloudScript
- `NetworkManager` — routes server requests through CloudScript
- Symbol is set by default on Android build target

The full SDK (Admin, Economy, Multiplayer, etc.) is included, but KZLib wrappers focus on the **Client API**.

---

## License

MIT License — see [LICENSE](LICENSE)
