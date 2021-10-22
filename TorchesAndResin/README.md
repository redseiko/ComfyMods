# Torches and Resin

  * Any newly placed `Standing wood torch`, `Standing iron torch` or `Sconce` starts with `10,000` resin fuel.

## Instructions

  * Unzip `TorchesAndResin.dll` to `/Valheim/BepInEx/plugins/`.

## Changelog

### 1.2.0

  * Updated for Hearth & Home.
  * Added a Transpiler to the Fireplace.Awake() method to change the initial UpdateFireplace.Invoke() from 0s to 0.5s.

### 1.1.0

  * Added braziers to list of torches supported.
  * Use `ZNetView.m_prefabName` instead of `GameObject.name`.

### 1.0.0

  * Updated project template and references to use latest DLLs.