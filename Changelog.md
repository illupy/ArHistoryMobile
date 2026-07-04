# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
- Created `docs/Game.md` to document the core game design, pillars, flow, and folder structure.
- Created `Changelog.md` to keep track of development history.
- Added support for parsing and storing `noteModelCode` fields in `Match3Set` class inside `DynamicSpriteManager.cs`.
- Created a `PreviewModelRegistry` prefab inside `Assets/Resources/prefabs/` referencing the 3D model prefabs to allow loading them dynamically by code in any scene.
- Created `UIModelRotator` class inside `Assets/Scripts/UI/` to handle interactive drag-to-rotate and scroll-to-zoom controls (aligned with the clamping and euler-angle logic from `ARModelInteraction` to ensure a consistent experience).
- Updated `Match3NotePopup` to support instantiating and rendering 3D historical models via `RenderTexture` and temporary `Camera` with dynamic bounding box calculation (`FocusCameraOnModel`) to automatically fit the camera view and adjust clipping planes regardless of the model's native scale. Removed the 3-second auto-close countdown and added support for closing the popup by clicking anywhere on the dark overlay background.
- Updated `SlotBarController` to pass `noteModelCode` when displaying the match-3 completion popup.
- Added support for parsing annotations inside lesson details in `LessonModels.cs` (`LessonAnnotation` and properties on `LessonDetailResponse`).
- Created `TextMeshProLinkHandler` script to capture pointer click events on TMPro links dynamically.
- Implemented automatic parsing of annotation tags `[ann:id]` into clickable golden hyperlinks in `LessonStepUIController.cs`. When clicked, it displays the 3D model popup (`Match3NotePopup`) with the annotated historical model.
