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
| **핵심 패키지** | `com.bsheepstudio.kzlib` v1.1.4 |
| **라이선스** | MIT |

이 저장소는 KZLib 라이브러리를 **개발·검증하는 실험실(Lab)** 역할을 합니다. 라이브러리 자체는 UPM Git URL로 다른 프로젝트에도 추가할 수 있습니다.

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

### Scripts 폴더 구조

```
Assets/KZLib/Scripts/
├── Data/          Config, Proto, Lingo, Tune(Graphic/Language/Native/Sound), Facet, Cluster
├── Framework/     Actor, Stanza, Pool, RosterMapper, Presentation, Graphic
├── Global/        상수, 공통 타입, Enum
├── Inspector/     KZ* Odin Inspector Attribute 정의
├── Main/          BaseMain, ResolutionMonitor
├── OnlyEditor/    AttributeDrawer, KZMenu, EditorWindow, PathCreator, Drawing
├── Platform/      Network, PlayFab, Webhook, InAppPurchase
├── Runtime/       Wrapper UGUI 컴포넌트, TimeFlow, Enchanted
├── Shared/        C# init-only 등 공용 shim
├── System/        Input, Sound, Scene, Resource, Context, CutScene, ...
├── UI/            UIManager, Window, CommonWindow, UI 모듈(Carousel/Accordion/FocusScroller)
└── Utility/       Extension, Kit, Log
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

#### Git (UPM)

Package Manager → **Add package from git URL**:

| 패키지 | GitHub |
|--------|--------|
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

> NuGetForUnity 설치 후 `Assets/KZLib/packages.config` 에 정의된 NuGet 패키지(MessagePipe, MemoryPack 등)가 자동으로 복원됩니다.

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
- **버전:** 1.1.4
- **작성자:** Ko KoZae

### Scripts 주요 모듈

| 모듈 | 설명 |
|------|------|
| **Main** | `BaseMain` — 앱 진입점, Test/Normal 플레이 모드, 언어·해상도 |
| **Framework/Actor** | `Actor`, `Unit`, `Structure` — 상태/스탯/버프/팀 관계 프레임워크 |
| **Framework** | Stanza(애니메이션), Pool(`GameObjectPawnPool`), `RosterMapper`, Presentation, Graphic |
| **UI** | `UIManager`, Window(2D/3D), CommonWindow(Loading/Download/Video/Transition 등), Carousel·CarouselNavigator·Accordion·FocusScroller |
| **System** | Input, Sound, Effect, Camera, Scene, Resource, Context, CutScene, DebugOverlay 등 |
| **Data** | Config(YAML), Proto(MemoryPack), Lingo, Tune, Facet, Cluster |
| **Platform** | PlayFab, Network, Webhook(Discord/Trello/Google), InAppPurchase |
| **Runtime** | Wrapper UGUI 컴포넌트, `TimeFlow`(오브젝트별 시간 스케일), Enchanted |
| **Inspector** | `KZ*` Odin Attribute (경로, HexColor, Clamp, List 등) |
| **OnlyEditor** | AttributeDrawer, KZMenu, EditorWindow, PathCreator, Drawing |
| **Utility** | Extension, Kit, Log |

### Runtime 컴포넌트 (일부)

- **UI:** Button, Toggle, ScrollRect(Reuse/Pager), ReuseGridLayoutGroup, Dropdown, Image(Shader/Graph), TMP, RawImage
- **Camera:** ObserveCamera
- **Particle:** ForceMoveParticleSystem
- **기타:** TimeFlow, EnchantedContentSizeFitter, FollowTargetWatcher

### OnlyEditor Drawing (일부)

- LineDrawing, ShapeDrawing, GraphicDrawing — UGUI 기반 드로잉 컴포넌트 및 에디터

---

## 아키텍처

### 앱 부트스트랩

```
BaseMain (서브클래스 구현)
  → TuneManager (언어·사운드·그래픽·네이티브 설정)
  → ConfigManager (YAML 로드)
  → RouteManager (경로 해석)
  → ResourceManager / AddressablesManager
  → UIManager, InputManager, SoundManager ...
```

에디터 **Test 모드**에서는 `TestModeConfig`의 씬별 transient 데이터로 TitleScene을 건너뛰고 지정 씬에서 바로 시작할 수 있습니다.

### 이벤트 버스 (MessagePipe)

`LogChannel` 초기화 시 MessagePipe가 구성됩니다. 해상도 변경, 입력, 로그 표시, Badge/Unlock 알림 등 시스템 간 이벤트를 pub/sub으로 전달합니다.

### Facet (서버 동기화 상태)

```
NetworkManager → FacetManager.Apply<TFacet>() → 게임 코드 Get/TryGet
```

`FacetManager`는 서버에서 내려온 `IFacet` payload를 세션 캐시로 보관합니다. 디스크 영속은 Tune(PlayerPrefs)을 사용합니다.

### Pool & RosterMapper

```
데이터 roster  →  RosterMapper<TItem,UData>  →  GameObjectPawnPool<T>  →  활성 뷰
```

- `GameObjectPawnPool<T>` — `GameObjectPawn` 기반 컴포넌트 풀 (Get/Put, storage parent)
- `RosterMapper<TItem,UData>` — roster 길이·순서와 풀 뷰 바인딩 (`TrySetDataList`)
- **비가상화 UI:** `Accordion`, `Carousel`, `CarouselNavigator` (`CarouselDot` 페이지 인디케이터)
- **가상화 UI:** `ReuseScrollRect`, `ReuseGridLayoutGroup`

### Context (Badge / Unlock)

`ContextManager`가 게임 프로젝트의 `IContentProvider` 구현체를 reflection으로 수집해, 콘텐츠 해금(Unlock)과 레드닷(Badge) 카운트를 관리합니다.

### 매니저 생명주기

`KZGameKit.ReleaseManager()` 가 싱글톤 매니저를 일괄 해제합니다.  
SceneState, UI, Input, Effect, Sound, Proto, Config, Cluster, Facet, Lingo, Addressables, Network, Webhook, Tune 등이 포함됩니다.

### 데이터 흐름

```
Excel (ClosedXML)  →  YAML Config / Lingo ScriptableObject
MemoryPack           →  Proto 바이너리 (.bytes)
Route.yaml           →  Resources / Addressables 경로 해석
Game.yaml            →  로컬/서버 리소스, 저장 모드, HUD 플래그
TestMode.yaml        →  에디터 Test 모드 씬별 transient 데이터
```

---

## 샘플

Package Manager → KoZaeLibrary → Samples 에서 Import:

| 샘플 | 설명 |
|------|------|
| **MainSample** | `BaseMain` 서브클래스, TitleScene 흐름 |
| **DebugOverlaySample** | FPS·디버그 오버레이 패널 |
| **CarouselSample** | Carousel·CarouselNavigator·CarouselDot UI 데모 |
| **FocusScrollerSample** | 포커스 기반 스크롤 리스트 |
| **AccordionSample** | 접기/펼치기 아코디언 UI (`RosterMapper` 기반) |
| **ToggleSample** | 커스텀 Toggle Mount |
| **ReuseScrollRectSample** | 가상화(재활용) ScrollRect |
| **StanzaSample** | Stanza Lerp·애니메이션 유틸 |
| **VibrationSample** | 네이티브 진동 API |

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
| `TestMode.yaml` | Test 모드 씬별 `ISceneTransient` payload (`SceneTransientDict`) |

### 경로 (`Assets/Resources/Text/Setting/`)

| 파일 | 용도 |
|------|------|
| `Route.yaml` | named route → 폴더 매핑 |

### Config 시스템

- `ConfigManager` 가 YamlDotNet으로 `*Config` 클래스를 역직렬화 (`TestModeConfig` 등)
- **KZMenu → Config → Generate All Config** 로 Excel에서 YAML 생성 (ClosedXML)
- Game / Webhook / TestMode 별 YAML 생성 메뉴 및 Custom 옵션 제공

---

## 에디터 도구 (KZMenu)

Unity 메뉴 **KZMenu** 에서 제공:

| 섹션 | 기능 |
|------|------|
| **Option** | 매니저 삭제, 빈 폴더 정리, 미사용 에셋 언로드, Missing Component/MeshFilter 검색, PlayFab/IAP 모듈 추가, 인터넷 확인 |
| **Explorer** | Route 파일, Documents, Tools, DataPath, PersistentDataPath 열기 |
| **Config** | Excel → YAML 생성 (Game/Webhook/TestMode, Custom), 설정 폴더 열기 |
| **Lingo** | Excel → Localization(String/Asset) + Addressables 등록 |
| **Proto** | Proto 배치 생성, Proto 윈도우 |
| **Window** | Manual, Editor Custom, PlayerPrefs, Graphic Quality, Network Test (Webhook) |
| **Scene** | Main Scene 열기 |

### Unity 기본 컨텍스트 메뉴 확장 (DefaultMenuItem)

| 영역 | 기능 |
|------|------|
| **Assets/KZSubMenu** | Script → ScriptableObject 생성, Texture/ScriptableObject 열기, Prefab Mesh Name 조회 |
| **Hierarchy** | Copy Hierarchy, Create Category Line |
| **GameObject** | Path Creator, UI (Empty Panel, Shape, Carousel) |

### 에디터 윈도우

ManualWindow, EasingGraphWindow, PixelEditorWindow, MeshFinderWindow, ProtoWindow, ResourceWindow, WebhookTestWindow (Network Test 메뉴, Discord/Trello/Google), PlayerPrefsWindow, EditorCustom 등

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

### 필수 Git (UPM)

| 패키지 | GitHub |
|--------|--------|
| UniTask | [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package) |
| UIEffect | [GitHub](https://github.com/mob-sakai/UIEffect?tab=readme-ov-file#-installation) |
| SoftMaskForUGUI | [GitHub](https://github.com/mob-sakai/SoftMaskForUGUI?tab=readme-ov-file#-installation) |
| ParticleEffectForUGUI | [GitHub](https://github.com/mob-sakai/ParticleEffectForUGUI?tab=readme-ov-file#-installation) |
| NuGetForUnity | [GitHub](https://github.com/GlitchEnzo/NuGetForUnity) |

### NuGet (`Assets/KZLib/packages.config`)

| 패키지 | 용도 |
|--------|------|
| MessagePipe | pub/sub 이벤트 버스 |
| MemoryPack | Proto 바이너리 직렬화 |
| YamlDotNet | YAML Config 역직렬화 |
| ClosedXML | Excel → Config/Lingo 변환 |
| MoonSharp | Lua 스크립트 (선택적) |
| R3 | Reactive Extensions |
| ZXing.Net | QR/바코드 |

### KZLib UPM 의존성 (`package.json`)

Newtonsoft JSON, Addressables, Localization, Input System, URP

---

## 관련 저장소

- [KoZaeLibrary](https://github.com/Sunnymoon724/KoZaeLibrary)

---

## PlayFab (선택)

`KZLIB_PLAY_FAB` 정의 시:

- `PlayFabManager` — Guest/Android Device 로그인, 프로필, CloudScript
- `NetworkManager` — 서버 요청을 CloudScript 경유로 라우팅, Facet payload 적용
- Android 빌드 타겟에 기본 심볼 설정됨

SDK 전체(Admin, Economy, Multiplayer 등)는 포함되어 있으나, KZLib 래퍼는 **Client API** 위주입니다.

---

## 라이선스

MIT License — [LICENSE](LICENSE) 참고
