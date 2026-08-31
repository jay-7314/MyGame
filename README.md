# Dream_꿈속의조작법

Unity(6000.3.8f1) 기반 2D 픽셀 아트 액션 플랫포머 게임입니다.
마을(Village)을 중심으로 NPC와 대화하며 스토리를 진행하고, 스테이지(Stage)에 진입해 적과 전투를 벌이는 구조로 이루어져 있습니다.

## 🎮 게임 소개

플레이어는 인트로에서 캐릭터를 선택한 뒤 마을에 도착합니다.
마을의 NPC들과 대화(Dialogue System)를 나누며 이야기를 진행하고, 맵(Map)을 통해 스테이지를 선택해 입장합니다.
스테이지에서는 스켈레톤 등의 적과 전투를 벌이며, 대시·점프·공격 등의 액션을 활용해 스테이지를 클리어합니다.

- **장르**: 2D 픽셀 아트 액션 플랫포머
- **주요 흐름**: 인트로 → 캐릭터 선택 → 마을(대화/설정) → 맵 선택 → 스테이지(전투)
- **렌더 파이프라인**: Universal Render Pipeline (URP)

## 🕹️ 주요 시스템 (코드 스킬)

프로젝트에서 직접 설계·구현한 주요 기능들입니다.

| 시스템 | 설명 |
| --- | --- |
| **Player Controller** | 이동, 점프(코요테 타임 포함), 대시, 착지 버퍼 등 정교한 2D 플랫포머 이동 로직 |
| **Player Attack / Knockback / Health** | 공격 히트박스 판정, 피격 넉백, 체력 관리 및 HUD 연동 |
| **Dialogue System** | `DialogueManager`를 통한 대사 출력, 화자 이미지 전환, 선택지(Choice) 분기 처리 |
| **Enemy System** | `Enemy`, `Skeleton` 등 적 AI와 히트박스, 체력바(`EnemyHealthBar`) |
| **Stage / Map / Village Manager** | 씬 전환 및 스테이지 진행 상태(Flag) 관리 |
| **Cinemachine 연동** | 구역(Zone) 기반 카메라 우선순위 전환 시스템 |
| **Intro / Character Select** | 캐릭터 선택, 세이브 로드(`LoadPage`), 신규 게임(`NewGame`) 처리 |


## 📁 폴더 구조

```
Assets/
├── Scripts/            # 핵심 게임 로직
│   ├── Player/          # 이동, 공격, 체력, 넉백 등 플레이어 관련
│   ├── Enemy/            # 적 AI 및 전투 관련
│   ├── Dialogue/         # 대화 시스템
│   ├── Stage/            # 스테이지 관리
│   ├── Village/          # 마을 NPC 및 설정 UI
│   ├── intro/            # 인트로, 캐릭터 선택, 세이브/로드
│   ├── Cinemachine/      # 카메라 연출
│   ├── Background/       # 배경(하늘 루프 등)
│   └── Common/           # 공용 스크립트(씬 전환, 게임 플래그 등)
├── Scenes/              # Intro, Village, Map, Stage, FirstVideo 등 씬 파일
├── Prefabs/             # 프리팹
├── Animations/          # 애니메이션 클립
├── Data/                # 대사·게임 데이터
├── Resources/           # 런타임 로드 리소스
└── (그 외 아트/폰트/타일 등 에셋 폴더)
```

## 🛠️ 개발 환경

- **Unity 버전**: `6000.3.8f1`
- **주요 패키지**
  - Universal Render Pipeline (`com.unity.render-pipelines.universal`)
  - Cinemachine `3.1.7`
  - Input System `1.18.0`
  - 2D Animation / Aseprite / Tilemap 등 2D 툴킷 일체
  - Timeline, Visual Scripting



## 🎨 사용 에셋 크레딧

프로젝트에 포함된 외부 아트/UI 에셋입니다. (Unity Asset Store 등에서 배포되는 리소스이며, 각 에셋의 라이선스는 원 배포처 정책을 따릅니다.)

- **Fantasy Wooden GUI - Free** — UI/버튼 등 목재 테마 GUI
- **Ethan the Hero** — 플레이어 캐릭터 스프라이트
- **Hero and Opponents** — 캐릭터/적 스프라이트
- **Evil Wizard 2** — 적 캐릭터 스프라이트
- **Gothicvania-Town** — 마을 배경/타일
- **Pixel Lost Game Scene** — 스테이지 배경
- **Pixel_HUD_UI_FreeKit** — HUD UI 키트
- **40+ Simple Icons - Free** — 아이콘 세트
- **PlatformerSet1** — 플랫포머 타일셋
- **Screw And Tape** — 오브젝트 스프라이트
- **TextMesh Pro** — 텍스트 렌더링 (Unity 기본 패키지)

## 📄 라이선스

이 저장소의 코드는 [MIT License](LICENSE)를 따릅니다.
단, 위에 명시된 외부 아트 에셋들은 각자의 원 라이선스를 따르며 별도 배포 시 주의가 필요합니다.

<img width="2399" height="1220" alt="Image" src="https://github.com/user-attachments/assets/6829543e-2018-4d6a-be2c-f643fb25566d" />
