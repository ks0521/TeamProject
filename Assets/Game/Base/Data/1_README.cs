
//데이터구조 및 기타 규약을 적어놓는 부분입니다. 

/* 각 SO별 고유 키 구조
 * 몬스터 및 플레이어 : 1 ~ 999
 * 스킬 : 1000 ~ 1999
 * 아이템 : 2000 ~ 2999
 * 장비 - 무기 : 3000 ~ 3399
 * 장비 - 방어구 : 3400 ~ 3799
 * 장비 - 장신구 : 3800 ~ 3999
 * 스테이지 : 4000 ~ 4999
*/

/* SO
 *  - PlayerBaseStatusSO : 플레이어 초기 스펙(기본 체력, 공격력, 공격 속도, 이동속도, 공격 범위 등)저장
    - MonsterSO : 몬스터 제작 간 필요한 필수 스텟 지정
    - ItemSO : 아이템 총체의 정보를 다루는 SO(장비에 필요한 세부스텟은 X, 재화 / 이벤트 아이템 등)
    - EquipmentSO : ItemSO를 상속받아 장비류(무기, 방어구, 장신구)의 정보를 저장하는 SO(장비의 스탯 존재)
    - DropTableSO : 몬스터가 드랍할 수 있는 아이템과 드랍 확률, 드랍 개수를 저장하고 
                    확률에 맞게 드랍테이블 내 장비를 뽑는 메서드 존재
    - SkillSO : 스킬의 정보(공격력, 시전시간, 발동 위치(추적 / 시전자 위치 / 지점)와 
                공격 타입(단일, 범위 .. ) 등)를 저장
    - StatusSO : 스탯강화 항목별 정보
                (강화별 증가수치, 해금 레벨, 최대 레벨, 증가 타입 - 합적용, 곱적용...)
    - StageSO : 스테이지 명, 스테이지 타입(일반 / 돌파),최대 스폰가능 몬스터, 
                등장 몬스터 프리셋, 제한시간, 목표 처치 수, 보스전 여부, 스테이지 돌파 보상
    - ItemDatabaseSO : 모든 아이템 리스트를 저장해서 키를 입력받으면 해당하는 ItemSO 반환
*/

/* RuntimeData
    - PlayerStatRuntime : 영구 성장 합산값
                        (기본스탯 + 스탯 강화치 + 보유장비효과 + 장착 장비 효과 + 패시브 스킬 효과)
    - PlayerCombatRuntime : 실시간 전투 상태
                        (현재 HP, 현재 타겟, 현재 상태 - 이동 / 공격 / 스킬시전 /버프 / 디버프)
    - SkillRuntimeState : 스킬의 장착정보와 각 스킬별 쿨타임
    - ChallengeStageRuntime : 돌파 스테이지 진행중일때만 활성화되며, 
        스테이지 돌파 정보(웨이브 몬스터 처치 수 / 보스 HP / 남은 시간 / 현재 도전중인 StageSO ) 저장
*/

/* SaveData
- GameSaveData : 전역 SaveData
        - StageProgressData : 최근 진행중인 일반 스테이지와 최대 돌파 스테이지 정보
        - PlayerCurrencyData : 플레이어 재화 정보(경험치-레벨 / 골드 / 강화석)
        - PlayerItemInventoryData : 플레이어 보유 아이템(이벤트아이템이나 재화상자 등...) 정보 저장
        - PlayerEquipmentInventoryData : 플레이어 보유 장비 정보(여기서 보유 장비 효과 계산) 
                                        + 강화 수치
        - PlayerEquipmentData : 플레이어 장착 아이템 정보(여기서 장착 효과 계산)
        - PlayerStatUpgradeData : 플레이어 스탯 강화 정보
                                (여기의 강화 단계 * StatusSO의 강화별 증가수치 = 스탯 증가 수치)
        - PlayerSkillData : 플레이어 스킬 정보(스킬 습득정보 / 스킬 슬롯 정보)
        - PlayerAccessTimeData : 플레이어 마지막 접속 시간
*/