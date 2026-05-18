# ⚔️ Auto Battle RPG — Unity 2D 팀 프로젝트

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2022.3.62f3-black?logo=unity" />
  <img src="https://img.shields.io/badge/C%23-.NET-blueviolet?logo=csharp" />
  <img src="https://img.shields.io/badge/Rendering-URP-blue" />
  <img src="https://img.shields.io/badge/Async-UniTask-green" />
  <img src="https://img.shields.io/badge/Assets-Addressables-orange" />
  <img src="https://img.shields.io/badge/UI-uGUI%20%2B%20TMP-lightgrey" />
  <img src="https://img.shields.io/badge/기간-2026.03.03%20~%2004.15-informational" />
</p>

<p align="center">
  <b>ScriptableObject 기반 데이터 설계 · Manager 초기화 체계 · EventHub 이벤트 연동</b><br/>
  <i>확장 가능한 라이브 게임 구조를 4인 팀으로 구현한 자동 전투 RPG</i>
</p>

---

## 🎮 플레이 영상

▶ [YouTube 데모 보기](https://youtu.be/7LdXl2Ow0QU)

---

## 📌 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 2D 자동 전투 RPG |
| 엔진 | Unity 2022.3.62f3 (URP) |
| 개발 기간 | 2026.03.03 ~ 2026.04.15 (약 6주) |
| 팀 규모 | 4인 |
| 핵심 컨셉 | 자동 전투 + 스테이지 돌파 + 성장(스탯 / 스킬 / 장비) + 퀘스트 |

> **구조적 목표** : SO 기반 데이터 설계, Manager 초기화 체계, EventHub 이벤트 연동을 통해 기능 간 결합도를 낮추고 라이브 서비스 확장이 가능한 아키텍처를 지향했습니다.

---

## 🛠 기술 스택

| 분류 | 기술 |
|------|------|
| 엔진 | Unity 2022.3.62f3 |
| 렌더링 | Universal Render Pipeline (URP) |
| 비동기 | UniTask |
| 에셋 로딩 | Addressables |
| UI | uGUI, TextMeshPro |
| 저장 | JSON (`SaveData.json`, `QuestSave.json`), PlayerPrefs |
| 버전 관리 | Git / GitHub (feature 브랜치 전략) |

---

## ✨ 주요 기능

### ⚔️ 스테이지 전투 시스템
- 일반 / 도전 / 보스 스테이지 분기 처리
- 몬스터 오브젝트 풀 기반 스폰 · 처치 판정 · 클리어/실패 처리
- 보스 고유 패턴 (돌진, 광역 스킬, 자동 스킬 시전)

### 💰 보상 시스템
- 일반 스테이지 드랍 테이블 (`DropTableSO`)
- 도전 / 보스 클리어 보상 테이블 (`RewardTableSO`)
- 오프라인 접속 보상 자동 계산

### 📈 성장 시스템
- **스탯 강화** : `RuntimeStatus + StatusCalculator` 기반 최종 스탯 계산
- **스킬** : 레벨업 · 장착 · 우선순위 변경 · 오토스킬 토글
- **장비** : 장착 · 강화 · 합성 · Dictionary 기반 빠른 조회

### 📋 퀘스트 시스템
- 일일 / 반복 / 무한 퀘스트 구조
- 서버 시간 기반 일일 퀘스트 자동 리셋

### 🔊 오디오 시스템
- 챕터별 BGM 자동 전환
- 이벤트 기반 효과음(SFX) 연동
- 인게임 볼륨 옵션 창

### 🖥️ UI / UX
- 팝업 매니저를 통한 계층적 팝업 관리
- 메인 HUD 실시간 스탯 반영
- 가챠(뽑기) UI
- 모바일 조이스틱 지원

---

## 🗂 프로젝트 구조 (핵심)

```
Assets/Game/
├── Base/          # 공통 데이터(SO), 매니저 초기화, 세이브/로드
├── Battle/        # 스테이지, 몬스터 스폰, 캐릭터 전투
├── Growth/        # 스탯·스킬·장비·재화 성장 시스템
├── Contents/
│   └── Quests/    # 퀘스트 & 일일 리셋 로직
├── UI/            # 메인 HUD, 팝업, 상점/가챠
└── Audio/         # AudioManager, BGM/SFX 클립
```

---

## 🚀 실행 방법

1. Unity Hub에서 프로젝트 폴더를 추가합니다.
2. **Unity 2022.3.62f3** 버전으로 엽니다.
3. `Assets/Game/Scenes/MainScene.unity` 씬을 실행합니다.

---

## 👥 팀원 소개 및 역할

> 커밋 이력 분석을 기반으로 각 팀원의 주요 기여 영역을 정리했습니다.

---

### 🏗 김규성 (GyuSeong · [ks0521](https://github.com/ks0521)) — **프로젝트 리드 / 핵심 아키텍처**
> 총 커밋 기여 1위 · 전 파트 통합 및 PR 관리 담당

| 영역 | 주요 작업 |
|------|-----------|
| **프로젝트 초기화** | Unity 폴더 구조 설계, `.gitignore` 설정, 팀원 개별 씬 분리 |
| **핵심 프레임워크** | `Manager` 초기화 체계 구축, SO 딕셔너리 설계, `EventHub` 이벤트 연동, `Addressables` 로딩 파이프라인 |
| **스테이지 시스템** | `StageManager` 구현, 일반/도전/보스 스테이지 분기, 몬스터 오브젝트 풀, 보스 HP UI 연동, 아이템 드랍 로직 |
| **세이브 / 로드** | JSON 기반 `SaveData` 구조 설계, 게임 시작 시 자동 로드, `PlayerPrefs` 연동 |
| **스킬 시스템** | `SkillManager` 설계 및 개선, 오토스킬 버튼 연결, 스킬 슬롯 저장 형식, 스킬 우선순위 변경 기능 |
| **장비 시스템** | 장비창 UI 최적화, `Dictionary` 기반 장비 빠른 조회 구현 |
| **오프라인 보상** | 비접속 시간 계산 로직 구현 및 보상 팝업 처리 (`OfflineRewards` 브랜치) |
| **가챠 시스템** | 뽑기 기능 구현, `GachaManager` 코드 개선, 상점 뽑기 UI 제작 |
| **오디오** | `AudioManager` 제작, BGM 교체 기능, 조이스틱 연결 |
| **UI / UX** | `PopupManager` 관리 방식 개선, 메인 씬 구축, UI 이미지 조정, 스테이지 클리어 UI |
| **퀘스트** | 퀘스트 잠금 상태 구현, 퀘스트 버그 수정, 서버 시간 리셋 연동 |
| **통합 / 배포** | 153개 PR 생성 및 머지, 파트 통합 씬 완성 |

---

### 🎯 김혜인 (khy13 · [khy1379](https://github.com/khy1379)) — **스킬 시스템 / 캐릭터**
> `feature-hy/player-skill-base`, `feature-hy/skill-ui-link`, `fix-hy/*` 브랜치 담당

| 영역 | 주요 작업 |
|------|-----------|
| **캐릭터 기초** | 캐릭터 이동 & 공격 기능 구현 (`Character Move And Attack feature`) |
| **스킬 기반 구조** | `EquipSkillController` 분리 설계, 스킬 프리팹 2종 제작 |
| **자동 스킬** | 우선순위 기반 자동 스킬 사용(`auto-priority-skill-use`) 구현 |
| **스킬 UI 연결** | `SkillManager ↔ SkillUI` 데이터 연동, 스킬 버튼 오류 수정 |
| **스킬 장착 팝업** | Skill Equip Change Popup UI 패널 수정 |
| **스킬 타입 변경** | Projectile Skill → Area Skill by Projectile 구조 전환 |
| **스킬 포인트 버그 수정** | 스킬포인트 0 상태에서 레벨업 버튼 잘못 활성화되는 현상 수정 |
| **조이스틱** | 조이스틱 기능 구현, `Joystick Prefab` 제작 |
| **데이터 정리** | `Node` 클래스 추가, 유틸 스크립트 위치 이동, 에셋 정리 |

---

### 🎵 이종준 (AlcheJ · [AlcheJ](https://github.com/AlcheJ)) — **오디오 / 보스 패턴 / 퀘스트**
> `Jongjun_BGM&Map`, `Jongjun_BossPattern`, `Jongjun_Quest2`, `Jongjun_QuestPolishing` 브랜치 담당

| 영역 | 주요 작업 |
|------|-----------|
| **BGM 시스템** | 맵 전환에 따른 BGM 자동 변경, 볼륨 옵션 창 기능 구현 |
| **사운드 에셋** | BGM / SFX 후보 선정 및 추가, 오디오 매니저 확장 |
| **보스 패턴** | 보스 스킬 자동화, 돌진 로직 구현 및 수정, 캐릭터/보스 모션 작업 (SPUM) |
| **캐릭터 애니메이션** | 보스 및 캐릭터 동작 구체화 (`260324 캐릭터 모션 작업 완료`) |
| **퀘스트 시스템** | 퀘스트 UI 구현부터 폴리싱까지 전담 (`Jongjun_Quest2` → `Jongjun_QuestPolishing`) |
| **상태 파일** | 초기 StatusFiles 설계 참여 (`Jongjun-StatusFiles`) |
| **드랍 테이블 검증** | 몬스터 사망 및 드랍 테이블 동작 확인 |

---

### 🎨 박관규 (GAWN GYU · rhksrb0823) — **UI 디자인 / 에셋**
> `pgg/UI` 브랜치 전담, 프로젝트 전 기간 UI 지속 작업

| 영역 | 주요 작업 |
|------|-----------|
| **메인 화면 UI** | 메인화면 및 팝업 UI 초기 제작 |
| **능력치 강화창** | 스탯 강화 UI 제작 및 버그 수정 |
| **스테이지 선택 창** | 스테이지 선택 화면 UI 구현 |
| **장비창 UI** | 장비창 UI 작업 및 연결 (`UI 연결 및 장비창 작업`) |
| **에셋 작업** | 장비 이미지, 캐릭터 이미지 추가 및 교체 |
| **지속적 UI 개선** | 전 개발 기간 동안 매일 UI 작업 및 리파인 (90 commits) |

---

## 📊 기여도 요약

```
GyuSeong  ████████████████████████████████████████  341 commits  (프로젝트 리드 / 아키텍처 / 통합)
GAWN GYU  ████████                                   90 commits  (UI 디자인 / 에셋)
AlcheJ    ████████                                   88 commits  (오디오 / 보스 / 퀘스트)
khy13     ██████                                     66 commits  (스킬 시스템 / 캐릭터)
```

---

## 📁 브랜치 전략

| 브랜치 | 담당 | 목적 |
|--------|------|------|
| `main` | 전체 | 안정화된 통합 버전 |
| `MainFramework` | GyuSeong | 핵심 프레임워크 구축 |
| `Stage` | GyuSeong | 스테이지 시스템 |
| `OfflineRewards` | GyuSeong | 오프라인 보상 |
| `feature-hy/player-skill-base` | khy13 | 스킬 기반 구조 |
| `feature-hy/skill-ui-link` | khy13 | 스킬 UI 연결 |
| `Jongjun_BGM&Map` | AlcheJ | BGM / 맵 사운드 |
| `Jongjun_BossPattern` | AlcheJ | 보스 패턴 |
| `Jongjun_Quest2` / `Jongjun_QuestPolishing` | AlcheJ | 퀘스트 구현 |
| `pgg/UI` | GAWN GYU | 전체 UI 작업 |
