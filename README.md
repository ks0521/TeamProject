# TeamProject
26.3.3 ~ 26.4.15
# TeamProject

Unity 기반 2D 자동 전투 RPG 팀 프로젝트입니다.  
프로젝트 기간: **2026.03.03 ~ 2026.04.15**

## 프로젝트 개요
- **핵심 컨셉**: 자동 전투 + 스테이지 돌파 + 성장(스탯/스킬/장비) + 퀘스트
- **핵심 구조**: `SO 기반 데이터 설계 + Manager 초기화 체계 + EventHub 이벤트 연동`
- **목표**: 확장 가능한 라이브 게임형 구조를 팀 단위로 구현

## 주요 기능
- **스테이지 전투**
  - 일반/도전/보스 스테이지 분기
  - 몬스터 스폰, 처치 판정, 클리어/실패 처리
- **보상 시스템**
  - 일반 스테이지 드랍 테이블(`DropTableSO`)
  - 도전/보스 클리어 보상 테이블(`RewardTableSO`)
  - 오프라인 보상 계산
- **성장 시스템**
  - 스탯 강화, 스킬 레벨업/장착, 장비 장착/강화/합성
  - `RuntimeStatus + StatusCalculator` 기반 최종 스탯 계산
- **퀘스트 시스템**
  - 일일/반복/무한 퀘스트 구조
  - 서버 시간 기반 일일 퀘스트 리셋
- **UI/오디오**
  - 팝업 관리, 메인 HUD 실시간 반영, 가챠 UI
  - 이벤트 기반 효과음/BGM 연동

## 기술 스택
- **Engine**: Unity `2022.3.62f3`
- **Rendering**: URP
- **Async**: UniTask
- **Data Loading**: Addressables
- **UI**: uGUI, TextMeshPro
- **Persistence**: JSON(`SaveData.json`, `QuestSave.json`), PlayerPrefs

## 프로젝트 구조 (핵심)
- `Assets/Game/Base`: 공통 데이터, 매니저, 저장
- `Assets/Game/Battle`: 전투/스테이지/캐릭터
- `Assets/Game/Growth`: 스탯/스킬/장비/재화
- `Assets/Game/Contents/Quests`: 퀘스트 및 일일 리셋
- `Assets/Game/UI`: 메인 UI, 팝업, 상점/가챠
- `Assets/Game/Audio`: BGM/SFX 관리
- `Documentation`: 상세 설계/분석 문서

## 실행 방법
1. Unity Hub에서 이 프로젝트 폴더를 추가
2. **Unity 2022.3.62f3**로 열기
3. `Assets/Game/Scenes/MainScene.unity` 실행

## 영상
- 플레이 영상: https://youtu.be/7LdXl2Ow0QU

