# UFO The Hunter

UFO를 조작해 지상의 오브젝트를 빨아들이며 성장하는 **하이퍼 캐주얼 게임**입니다.
SBS게임아카데미 팀 프로젝트로 제작했습니다.

- **기간** 2025.03 ~ 2025.06
- **엔진 / 언어** Unity · C#
- **팀 구성** 4명 — **프로그래밍 1명(본인)** · 기획 1명 · 3D 아트 1명 · UI 1명
- **역할** **프로그래밍 전담.** 기획·아트·UI 담당자와 협업해 게임 로직 전반을 구현했습니다.

---

## 직접 작성한 코드

> 이 저장소에는 에셋스토어 패키지가 함께 포함되어 있습니다.
> 팀에서 **프로그래밍은 제가 전담**했으며, 제가 작성한 코드는 아래 한 폴더에 모여 있습니다.

### [`Assets/HoleGame/Script/`](Assets/HoleGame/Script) — 130개

| 폴더 | 내용 |
|---|---|
| [`UFO/`](Assets/HoleGame/Script/UFO) | UFO 이동·높이 제어, 흡수, 크기 성장, 카메라 연출 |
| [`EarthObject/`](Assets/HoleGame/Script/EarthObject) | 흡수 대상 오브젝트, 스폰, 질량·형태 판정, 보스 |
| [`Widget/`](Assets/HoleGame/Script/Widget) | UI 위젯 |
| [`Skill/`](Assets/HoleGame/Script/Skill) | 스킬 베이스와 개별 스킬 |
| [`AllManager/`](Assets/HoleGame/Script/AllManager) | 게임·스테이지·오브젝트·사운드 매니저, 세이브 |
| [`Data/`](Assets/HoleGame/Script/Data) | 스탯·스테이지·유저 데이터 정의 |
| [`Achievement/`](Assets/HoleGame/Script/Achievement) | 업적과 보상 |
| [`Editor/`](Assets/HoleGame/Script/Editor) | 에디터 확장 도구 |

*(`Assets/Plugins`, `Assets/AssetPlugin`, `Assets/JMO Assets`, `Assets/Effects` 등은 외부 에셋입니다.)*

---

## 봐주셨으면 하는 부분

### ① 기획자가 직접 데이터를 다루도록 만든 구조

기획 담당자가 엑셀로 밸런스를 잡으면 그대로 게임에 들어가도록 CSV 로더와 데이터 정의를 만들었습니다.

- [`AllManager/CsvLoader.cs`](Assets/HoleGame/Script/AllManager/CsvLoader.cs)
- [`Data/ExelUFOStatData.cs`](Assets/HoleGame/Script/Data/ExelUFOStatData.cs) · [`ExelEarthObjectData.cs`](Assets/HoleGame/Script/Data/ExelEarthObjectData.cs) · [`ExelPlayerData.cs`](Assets/HoleGame/Script/Data/ExelPlayerData.cs)

### ② 맵 제작 에디터 확장

스테이지 배치를 코드 없이 편집할 수 있도록 에디터 창을 만들었습니다.

- [`Editor/MapEditorWindow.cs`](Assets/HoleGame/Script/Editor/MapEditorWindow.cs) — 맵 편집 창
- [`Editor/SyncMapData.cs`](Assets/HoleGame/Script/Editor/SyncMapData.cs) — 맵 데이터 동기화
- [`Editor/SelectPrefabsWindow.cs`](Assets/HoleGame/Script/Editor/SelectPrefabsWindow.cs) · [`SelectStatWindow.cs`](Assets/HoleGame/Script/Editor/SelectStatWindow.cs)

### ③ 핵심 플레이 로직

- [`UFO/SwallowUp.cs`](Assets/HoleGame/Script/UFO/SwallowUp.cs) · [`UFO/SizeUp.cs`](Assets/HoleGame/Script/UFO/SizeUp.cs) — 흡수와 성장
- [`EarthObject/ShapeManager.cs`](Assets/HoleGame/Script/EarthObject/ShapeManager.cs) — 오브젝트 형태·질량에 따른 흡수 판정
- [`Skill/SkillBase.cs`](Assets/HoleGame/Script/Skill/SkillBase.cs) — 스킬 공통 베이스와 파생 구조

---

## 실행 화면

<!-- 스크린샷 / GIF를 이 아래에 추가하세요 -->
