# KoZae's Code Lab

**[English](README.en.md)** | **한국어**

Unity 6 기반 연구·개발 프로젝트입니다. 다양한 게임 개발 코드를 **KoZaeLibrary (KZLib)** 로 정리하고, 실제 프로젝트 환경에서 통합 테스트합니다.

[![License: MIT](https://img.shields.io/badge/License-MIT-blueviolet.svg)](LICENSE)
[![Unity Version](https://img.shields.io/badge/Unity-6000.3.7f1-important)](https://unity3d.com/)

---

## 목차

- [개요](#개요)
- [시작하기](#시작하기)
- [KoZaeLibrary (KZLib)](#kozealibrary-kzlib)
- [아키텍처](#아키텍처)
- [샘플](#샘플)
- [설정 파일](#설정-파일)
- [에디터 도구 (KZMenu)](#에디터-도구-kzmenu)
- [선택 모듈](#선택-모듈)
- [의존성](#의존성)
- [관련 저장소](#관련-저장소)

---

## 개요

| 항목 | 내용 |
|------|------|
| **Unity 버전** | 6000.3.7f1 (Unity 6.3) |
| **렌더 파이프라인** | URP (Universal Render Pipeline) |
| **핵심 패키지** | `com.bsheepstudio.kzlib` v1.1.1 |
| **라이선스** | MIT |

이 저장소는 KZLib 라이브러리를 **개발·검증하는 실험실(Lab)** 역할을 합니다. 라이브러리 자체는 UPM Git URL로 다른 프로젝트에도 추가할 수 있습니다.

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

---

## 시작하기

### 1. 요구 사항

- **Unity 6000.3.7f1** 이상

#### Asset Store

| 패키지 | 링크 |
|--------|------|
| DOTween Pro | [Asset Store](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) |
| Odin Inspector | [Asset Store](https://assetstore.unity3d.com/packages/tools/utilities/odin-inspector-and-serializer-89041) |

#### Git

Package Manager → **Add package from git URL**:

| 패키지 | GitHub |
|--------|--------|
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

### 2. 프로젝트 열기

1. Unity Hub에서 이 저장소를 엽니다.
2. 첫 임포트 후 **Window → Package Manager** 에서 `KoZaeLibrary` 샘플을 Import 합니다.
3. **KZMenu → Scene → Open Main Scene** 또는 원하는 샘플 씬을 엽니다.

### 3. KZLib만 다른 프로젝트에 추가

Package Manager → **Add package from git URL**:

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

---

## KoZaeLibrary (KZLib)

### 패키지 정보

- **이름:** `com.bsheepstudio.kzlib`
- **표시 이름:** KoZaeLibrary
- **버전:** 1.1.1
- **작성자:** Ko KoZae

### Scripts 주요 모듈

| 모듈 | 설명 |
|------|------|
| **Main** | `BaseMain` — 앱 진입점, 언어·해상도·초기화 모드 |
| **Actor** | `Actor`, `Unit`, `Structure` — 상태/스탯/버프/팀 관계 프레임워크 |
| **UI** | `UIManager`, Window(2D/3D), 공통 패널, Carousel/Accordion/FocusScroller |
| **System** | Input, Sound, Effect, Camera, Scene, Resource, Addressables, Debug 등 매니저 |
| **Data** | Config(YAML), Proto(MemoryPack), Lingo, Tune, Affix, Cluster |
| **Network** | `NetworkManager` — PlayFab 연동 서버 요청 |
| **Platform** | PlayFab, Webhook(Discord/Trello/Google), InAppPurchase |
| **GameTime** | 게임 시간·시간 스케일 |
| **Utility** | Extension, Kit, Stanza(애니메이션), ObjectPool, Log |
| **InEditor** | 커스텀 UGUI 컴포넌트, Odin Attribute, 에디터 윈도우·메뉴 |
| **Develop** | PathCreator, GraphicQualityOption |

### InEditor 컴포넌트 (일부)

- **UI:** Button, Toggle, ScrollRect(Reuse/Pager), Dropdown, Image(Shader/Graph), TMP
- **Camera:** ObserveCamera
- **Particle:** ForceMoveParticleSystem
- **Drawing:** LineDrawing, ShapeDrawing, GraphicDrawing

---

## 아키텍처

### 앱 부트스트랩

```
BaseMain (서브클래스 구현)
  → TuneManager (언어·사운드·그래픽 설정)
  → ConfigManager (YAML 로드)
  → RouteManager (경로 해석)
  → ResourceManager / AddressablesManager
  → UIManager, InputManager, SoundManager ...
```

### 매니저 생명주기

`KZGameKit.ReleaseManager()` 가 싱글톤 매니저를 일괄 해제합니다.  
SceneState, UI, Input, Effect, Sound, Proto, Config, Lingo, Addressables, Network, Webhook 등이 포함됩니다.

### 데이터 흐름

```
Excel (ClosedXML)  →  YAML Config / Lingo ScriptableObject
MemoryPack           →  Proto 바이너리 (.bytes)
Route.yaml           →  Resources / Addressables 경로 해석
Game.yaml            →  로컬/서버 리소스, 저장 모드, HUD 플래그
```

---

## 샘플

Package Manager → KoZaeLibrary → Samples 에서 Import:

| 샘플 | 설명 |
|------|------|
| **MainSample** | `BaseMain` 서브클래스, TitleScene 흐름 |
| **DebugOverlaySample** | FPS·디버그 오버레이 패널 |
| **FocusScrollerSample** | 포커스 기반 스크롤 리스트 |
| **AccordionSample** | 접기/펼치기 아코디언 UI |
| **ToggleSample** | 커스텀 Toggle Mount |
| **ReuseScrollRectSample** | 가상화(재활용) ScrollRect |
| **StanzaSample** | Stanza Lerp·애니메이션 유틸 |
| **VibrationSample** | 네이티브 진동 API |

> `Samples~/CarouselSample` 도 존재하나 `package.json` samples 목록에는 미등록 상태입니다.

---

## 설정 파일

### 런타임 (`Assets/Resources/Text/Config/`)

| 파일 | 용도 |
|------|------|
| `Game.yaml` | Proto/Language/Config/UI/FX 경로, `IsLocalResource`, `IsLocalSave`, HUD |
| `Webhook.yaml` | Discord Webhook URL (BugReport 등) |

### 에디터 (`Assets/WorkResources/Text/Config/`)

| 파일 | 용도 |
|------|------|
| `Editor.yaml` | 에디터 전용 파라미터 경로 딕셔너리 |

### 경로 (`Assets/Resources/Text/Setting/`)

| 파일 | 용도 |
|------|------|
| `Route.yaml` | named route → 폴더 매핑 |

### Config 시스템

- `ConfigManager` 가 YamlDotNet으로 `*Config` 클래스를 역직렬화
- **KZMenu → Config → Generate All Config** 로 Excel에서 YAML 생성 (ClosedXML)

---

## 에디터 도구 (KZMenu)

Unity 메뉴 **KZMenu** 에서 제공:

| 섹션 | 기능 |
|------|------|
| **Option** | 매니저 삭제, 빈 폴더 정리, 미사용 에셋 언로드, PlayFab/IAP 모듈 추가 |
| **Explorer** | Route 파일·Documents·DataPath·PersistentDataPath 열기, VS Code 연동 |
| **Config** | Excel → YAML 생성, 설정 폴더 열기 |
| **Lingo** | Excel → Localization + Addressables 등록 |
| **Proto** | Proto 배치 생성, Proto 윈도우 |
| **Window** | Manual, PlayerPrefs, Graphic Quality, Network/Webhook Test |
| **Scene** | Main Scene 열기 |

### 에디터 윈도우

ManualWindow, EasingGraphWindow, PixelEditorWindow, MeshFinderWindow, ProtoWindow, ResourceWindow, WebhookTestWindow(Discord/Trello/Google) 등

---

## 선택 모듈

컴파일 심볼로 기능을 켜고 끕니다. **KZMenu → Option** 에서 추가 가능:

| 심볼 | 기능 |
|------|------|
| `KZLIB_PLAY_FAB` | PlayFab Client API (로그인, 프로필, CloudScript) |
| `KZLIB_IN_APP_PURCHASE` | Unity Purchasing (IAP) |

PlayFab SDK는 `Assets/PlayFabSDK/` 에 포함되어 있으며, KZLib 래퍼는 Client·CloudScript 위주로 사용합니다.

---

## 의존성

### 필수 Unity Asset Store

| 패키지 | 링크 |
|--------|------|
| DOTween Pro | [Asset Store](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) |
| Odin Inspector | [Asset Store](https://assetstore.unity3d.com/packages/tools/utilities/odin-inspector-and-serializer-89041) |

### 필수 Git

| 패키지 | GitHub |
|--------|--------|
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

## 관련 저장소

- [KoZaeLibrary](https://github.com/Sunnymoon724/KoZaeLibrary)

---

## PlayFab (선택)

`KZLIB_PLAY_FAB` 정의 시:

- `PlayFabManager` — Guest/Android Device 로그인, 프로필, CloudScript
- `NetworkManager` — 서버 요청을 CloudScript 경유로 라우팅
- Android 빌드 타겟에 기본 심볼 설정됨

SDK 전체(Admin, Economy, Multiplayer 등)는 포함되어 있으나, KZLib 래퍼는 **Client API** 위주입니다.

---

## 라이선스

MIT License — [LICENSE](LICENSE) 참고
