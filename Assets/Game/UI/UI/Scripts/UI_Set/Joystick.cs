using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Base.Data;
using Base.Managers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
// using System.Numerics;

namespace UI.Scripts
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] Canvas canvas;
        [SerializeField] RectTransform joystickBg;
        [SerializeField] RectTransform joystickHandle;
        private float radius;
        private Vector2 inputPos = Vector2.zero;
        enum JoystickPos { None, LeftTop, RightTop, LeftBottom, RightBottom }
        private JoystickPos jPos;
        [SerializeField] private Image[] focusImgs = new Image[4];

        EventHub hub;

        void Start()
        {
            Init();
        }
        public void Init()
        {
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            radius = joystickBg.rect.width * 0.5f;
        }

        void FocusImgSet()
        {
            JoystickPos tJPos = jPos;

            bool isLeft = inputPos.x <= 0f;
            bool isTop = 0f <= inputPos.y;

            if (isLeft)
            {
                if (isTop) jPos = JoystickPos.LeftTop;
                else jPos = JoystickPos.LeftBottom;
            }
            else
            {
                if (isTop) jPos = JoystickPos.RightTop;
                else jPos = JoystickPos.RightBottom;
            }

            if (tJPos == jPos) return;

            int jPosNum = (int)jPos - 1;
            for (int i = 0; i < 4; i++)
            {
                if (jPosNum <= -1 || jPosNum != i) focusImgs[i].gameObject.SetActive(false);
                else focusImgs[i].gameObject.SetActive(true);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBg,
                eventData.position,
                canvas.worldCamera,
                out var vec);
            // Vector2 vec = eventData.position - (Vector2)joystickBg.position;
            vec = Vector2.ClampMagnitude(vec, radius);
            joystickHandle.localPosition = vec;

            inputPos = vec.normalized;

            hub.DirectionChanged(inputPos);
            FocusImgSet();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Vector2 zVec = Vector2.zero;
            inputPos = zVec;
            joystickHandle.anchoredPosition = zVec;
            hub.DirectionChanged(zVec);
            FocusImgSet();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        // /// 조이스틱 입력 처리 클래스 (UI 캔버스 기반)
        // /// 미리 생성된 조이스틱 UI 오브젝트를 제어하여 입력을 처리합니다.
        // /// 조이스틱은 캔버스에 고정되어 카메라 움직임에 영향받지 않습니다.
        // [Header("조이스틱 UI 참조")]
        // [Tooltip("미리 생성된 조이스틱 베이스 오브젝트 (Canvas의 자식)")]
        // public RectTransform joystickBase;

        // [Tooltip("조이스틱 베이스의 자식인 스틱 오브젝트")]
        // public RectTransform joystickKnob;

        // // === 이벤트 ===
        // // public static event Action<Vector2> OnDirectionChanged; // 조이스틱 방향이 변경될 때 발생하는 이벤트 (PlayerController가 구독하여 플레이어 이동에 사용)
        // private EventHub hub;
        // // === 입력 처리 관련 변수 ===
        // private Vector2 inputStartPosition; // 터치/클릭을 시작한 스크린 좌표 위치
        // private Vector2 currentInputPosition; // 현재 터치/마우스의 스크린 좌표 위치
        // private bool isInputActive = false; // 현재 입력이 활성화되어 있는지 여부 (터치/클릭 중인지)
        // private Canvas parentCanvas; // 조이스틱이 속한 캔버스 참조
        // private float baseRadius; // 조이스틱 베이스의 실제 반지름
        // private Vector2 baseInitialPosition; // 조이스틱 베이스의 초기 위치 (Canvas 내 고정 위치)

        // /// 게임 시작 시 한 번 호출되어 필요한 컴포넌트들을 설정합니다.
        // void Start()
        // {
        //     Init();
        // }
        // public void Init()
        // {
        //     // 캔버스 참조 획득
        //     parentCanvas = joystickBase.GetComponentInParent<Canvas>();

        //     // 조이스틱 패드와 스틱의 실제 크기 계산
        //     CalculateJoystickDimensions();

        //     // 베이스의 초기 위치 저장
        //     if (joystickBase != null)
        //     {
        //         baseInitialPosition = joystickBase.localPosition;
        //         // 초기에는 조이스틱을 비활성화 상태로 설정
        //         joystickBase.gameObject.SetActive(false);
        //     }

        //     // 스틱을 베이스 중앙으로 초기화
        //     if (joystickKnob != null)
        //     {
        //         joystickKnob.localPosition = Vector2.zero;
        //     }
        //     hub = GameManager.Instance.GetGameSystem<EventHub>();
        // }

        // /// 입력 처리와 UI 업데이트를 담당합니다.
        // void Update()
        // {
        //     // 터치/마우스 입력 감지 및 처리
        //     HandleInput();

        //     // 조이스틱 UI 위치 및 표시 상태 업데이트
        //     UpdateJoystickUI();
        // }

        // /// 조이스틱 베이스의 반지름을 계산하는 함수
        // void CalculateJoystickDimensions()
        // {
        //     if (joystickBase == null) return;

        //     // 베이스의 반지름 계산 (RectTransform 크기 기반)
        //     baseRadius = Mathf.Min(joystickBase.rect.width, joystickBase.rect.height) * 0.5f;
        //     // 패드 끝의 60%만큼 이동 가능하도록 설정
        //     baseRadius = baseRadius * 0.6f;
        // }

        // /// 터치/마우스 입력을 감지하고 처리하는 함수
        // void HandleInput()
        // {
        //     // === 모바일 터치 입력 처리 ===
        //     if (Input.touchCount > 0)
        //     {
        //         // 첫 번째 터치 정보 획득
        //         Touch touch = Input.GetTouch(0);
        //         Vector2 touchScreenPos = touch.position;

        //         // 터치 상태에 따른 처리
        //         switch (touch.phase)
        //         {
        //             case TouchPhase.Began:
        //                 // 터치 시작 - 조이스틱 활성화
        //                 StartInput(touchScreenPos);
        //                 break;

        //             case TouchPhase.Moved:
        //             case TouchPhase.Stationary:
        //                 // 터치 드래그 중 - 방향 계산 및 업데이트
        //                 UpdateInput(touchScreenPos);
        //                 break;

        //             case TouchPhase.Ended:
        //             case TouchPhase.Canceled:
        //                 // 터치 종료 - 조이스틱 비활성화
        //                 EndInput();
        //                 break;
        //         }
        //     }
        //     // === 에디터에서 마우스 입력 처리 (개발/테스트용) ===
        //     else
        //     {
        //         // 마우스 위치를 스크린 좌표로 사용
        //         Vector2 mouseScreenPos = Input.mousePosition;

        //         if (Input.GetMouseButtonDown(0))
        //         {
        //             // 마우스 클릭 시작
        //             StartInput(mouseScreenPos);
        //         }
        //         else if (Input.GetMouseButton(0) && isInputActive)
        //         {
        //             // 마우스 드래그 중 (입력이 활성화된 상태에서만)
        //             UpdateInput(mouseScreenPos);
        //         }
        //         else if (Input.GetMouseButtonUp(0))
        //         {
        //             // 마우스 클릭 종료
        //             EndInput();
        //         }
        //     }
        // }

        // enum JoystickPos { None, LeftTop, RightTop, LeftBottom, RightBottom }
        // private JoystickPos jPos;
        // /// 입력 시작 처리 함수
        // void StartInput(Vector2 screenPosition)
        // {
        //     // 입력 활성화 플래그 설정
        //     isInputActive = true;
        //     jPos = JoystickPos.None;
        //     // 시작 위치 저장
        //     inputStartPosition = screenPosition;
        //     currentInputPosition = screenPosition;

        //     Debug.Log($"조이스틱 입력 시작: {screenPosition}");
        // }
        // /// 입력 업데이트 처리 함수
        // void UpdateInput(Vector2 screenPosition)
        // {
        //     // 현재 입력 위치 업데이트
        //     currentInputPosition = screenPosition;

        //     // 시작점에서 현재점으로의 벡터 계산 (스크린 좌표계)
        //     Vector2 inputVector = currentInputPosition - inputStartPosition;

        //     // 벡터를 정규화하여 방향만 추출
        //     Vector2 direction = Vector2.zero;
        //     if (inputVector.magnitude > 0.2f) // 데드존 적용
        //     {
        //         direction = inputVector.normalized;

        //         bool isLeft = direction.x <= 0f;
        //         bool isTop = direction.y >= 0f;
        //         if (isLeft)
        //         {
        //             if (isTop) jPos = JoystickPos.LeftTop;
        //             else jPos = JoystickPos.LeftBottom;
        //         }
        //         else
        //         {
        //             if (isTop) jPos = JoystickPos.RightTop;
        //             else jPos = JoystickPos.RightBottom;
        //         }
        //     }
        //     // 계산된 방향을 이벤트로 전달
        //     // OnDirectionChanged?.Invoke(direction);
        //     hub.DirectionChanged(direction);
        // }

        // /// 입력 종료 처리 함수
        // void EndInput()
        // {
        //     // 입력 비활성화
        //     isInputActive = false;
        //     jPos = JoystickPos.None;

        //     // 이동 중지를 위해 영벡터(0,0) 전달
        //     // OnDirectionChanged?.Invoke(Vector2.zero);
        //     hub.DirectionChanged(Vector2.zero);

        //     Debug.Log("조이스틱 입력 종료");
        // }

        // [SerializeField] private Image[] focusImgs = new Image[4];
        // /// 조이스틱 UI 업데이트 함수
        // void UpdateJoystickUI()
        // {
        //     // 조이스틱 UI 오브젝트들이 유효하지 않으면 처리하지 않음
        //     if (joystickBase == null || joystickKnob == null || parentCanvas == null) return;

        //     if (isInputActive)
        //     {
        //         // === 입력이 활성화된 상태 ===

        //         // 조이스틱 베이스를 화면에 표시
        //         joystickBase.gameObject.SetActive(true);

        //         // 스크린 좌표를 캔버스 로컬 좌표로 변환
        //         Vector2 canvasPosition;
        //         RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //             parentCanvas.transform as RectTransform,
        //             inputStartPosition,
        //             parentCanvas.worldCamera,
        //             out canvasPosition
        //         );

        //         // 베이스를 터치 시작 위치에 배치
        //         // joystickBase.localPosition = canvasPosition;
        //         joystickBase.localPosition = baseInitialPosition;

        //         // 스틱 위치 계산
        //         Vector2 inputVector = currentInputPosition - inputStartPosition;

        //         // 캔버스 스케일 팩터 고려
        //         float scaleFactor = parentCanvas.scaleFactor;
        //         Vector2 localInputVector = inputVector / scaleFactor;

        //         // knob이 base의 중앙으로부터 떨어진 거리가 반지름을 초과하지 않도록 제한
        //         Vector2 clampedOffset = Vector2.ClampMagnitude(localInputVector, baseRadius);

        //         // 스틱을 계산된 위치에 배치 (베이스 기준 로컬 좌표)
        //         joystickKnob.localPosition = clampedOffset;
        //     }
        //     else
        //     {
        //         // === 입력이 비활성화된 상태 ===

        //         // 조이스틱을 화면에서 숨김
        //         joystickBase.gameObject.SetActive(false);

        //         // 스틱을 중앙으로 리셋
        //         if (joystickKnob != null)
        //         {
        //             joystickKnob.localPosition = Vector3.zero;
        //         }
        //     }
        //     FocusImgSet();
        // }
        // void FocusImgSet()
        // {
        //     int jPosNum = (int)jPos - 1;
        //     for (int i = 0; i < 4; i++)
        //     {
        //         if (jPosNum <= -1 || jPosNum != i) focusImgs[i].gameObject.SetActive(false);
        //         else focusImgs[i].gameObject.SetActive(true);
        //     }
        // }
    }
}