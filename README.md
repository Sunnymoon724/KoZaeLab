# KoZae's Code Lab

A research lab focused on developing various codes into a reusable library.

The library requires the following Unity Assets: **DotweenPro** and **Odin Inspector** to function.

[![License: MIT](https://img.shields.io/badge/License-MIT-blueviolet.svg)](https://github.com/Sunnymoon724/KoZaeLab/blob/master/LICENSE)
[![Unity Version](https://img.shields.io/badge/Unity-6.3%20or%20later-important)](https://unity3d.com/)

## 🧭Table of Contents
* [Getting Started](#getting-started)
* [Dependencies](#dependencies)
* [Error Solution](#error-solution)
* [Third Party Attributions](#third-party-attributions)

***

<a id="getting-started"></a>
## 🚀 Getting Started

### 1. Unity Version
This project is developed based on **Unity 6.3.0f1**.

### 2. Git URL
You can add the project via the Unity Package Manager (UPM) using the following Git URL:

```
https://github.com/Sunnymoon724/KoZaeLab.git?path=Assets/KZLib
```

***

<a id="dependencies"></a>
## 🛠️Dependencies

### Required Unity Assets
* **DotweenPro**: [Asset Store Link](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)
* **Odin Inspector**: [Asset Store Link](https://assetstore.unity3d.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

### Included Third-Party Libraries
> This library internally includes the following code and does not require separate installation.
* **UniTask**
* **ParticleEffectForUGUI**, **SoftMaskForUGUI**, **UIEffect**

***

<a id="error-solution"></a>
## ⚠️ Error Solution

**Issue:** Compilation errors may occur when the library is initially downloaded due to missing DLL files in WorkResources. (This specific DLL is generated via automated Excel parsing and is planned for public release soon.)

**Solution:** You must manually add the missing DLL by extracting the compressed file located at the following path:
`Packages\KoZaeLibrary\WorkResources\CommonLibs\KZLib`

***

<a id="third-party-attributions"></a>
## 🤝 Third Party Attributions
Please refer to the following file for the source and licensing information of all third-party libraries and code used in this library.
[**ATTRIBUTIONS.md**](ATTRIBUTIONS.md)

***
