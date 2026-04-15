using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct MovingStatus
    {
        public bool canMove;
        public float speed;
        public float atkRange;
        public Vector2 targetPos;
        public MovingStatus(bool canMove, Vector2 targetPos, float speed, float atkRange)
        {
            this.canMove = canMove;
            this.speed = speed;
            this.atkRange = atkRange;
            this.targetPos = targetPos;
        }
    }
    public class CharacterMove
    {
        //float speed;
        Rigidbody2D rb;
        Vector2 moveVelocity;
        [field: SerializeField] public bool IsInputMoving { get; private set; }
        public bool isAutoMoving;
        [field: SerializeField] public bool IsLeftMove { get; private set; }
        //test
        public bool canMove;
        enum InputState { None, Keyboard, Joystick, Auto }
        private InputState inputState;
        public void Init(Rigidbody2D rb)
        {
            canMove = true;
            this.rb = rb;
        }
        bool IsMovingCheck(Vector2 dir, float speed)
        {
            if (inputState == InputState.Keyboard) return true;
            else if (dir.sqrMagnitude < 0.1f * 0.1f)
            {
                moveVelocity = Vector2.zero;
                inputState = InputState.None;
                return false;
            }
            IsLeftMove = dir.x < 0 ? true : false;
            moveVelocity = dir * speed;
            inputState = InputState.Joystick;
            return true;
        }
        bool IsMovingCheck(float x, float y, float speed)
        {
            if (inputState == InputState.Joystick) return true;
            if (x == 0 && y == 0)
            {
                moveVelocity = Vector2.zero;
                inputState = InputState.None;
                return false;
            }
            IsLeftMove = x < 0 ? true : false;
            moveVelocity = new Vector2(x, y) * speed;
            inputState = InputState.Keyboard;
            return true;
        }
        public void UpdateMoveInput(float speed)
        {
            if (!canMove) return;
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            IsInputMoving = IsMovingCheck(x, y, speed);
        }
        public void UpdateMoveInput(Vector2 dir, float speed)
        {
            if (!canMove) return;
            IsInputMoving = IsMovingCheck(dir, speed);
            Debug.Log($"UpdateMoveInput {IsInputMoving}");
        }
        public void VFixedMove()
        {

        }
        public void VChaseMove(Transform targetTransform, float speed)
        {
            if (!canMove) return;
            if (IsInputMoving) return;
            inputState = InputState.Auto;
            Vector2 targetPos = targetTransform.position;
            Vector2 resultVec = targetPos - rb.position;
            float dis = resultVec.sqrMagnitude;
            isAutoMoving = dis < 0.01f;
            if (isAutoMoving)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                Vector2 dir = resultVec.normalized;
                rb.velocity = dir * speed;
            }
        }
        public void VChaseMove(Vector2 dis)
        {
            if (!canMove) return;
            if (IsInputMoving) return;
            inputState = InputState.Auto;
            isAutoMoving = dis != Vector2.zero;
            if (isAutoMoving)
                rb.velocity = dis;
            else
                rb.velocity = Vector2.zero;
        }
        public void FixedMove()
        {
            if (!canMove || !IsInputMoving) return;
            rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
            Debug.Log("fixedMove 움직임");
        }
        public void MoveStop()
        {
            IsInputMoving = false;
        }
        public void ChaseMove(Vector2 targetDir, float speed)
        {
            if (!canMove) return;
            if (IsInputMoving) return;
            inputState = InputState.Auto;
            isAutoMoving = targetDir != Vector2.zero;
            if (isAutoMoving)
            {
                rb.MovePosition(rb.position + targetDir * speed * Time.deltaTime);
                IsLeftMove = targetDir.x < 0 ? true : false;
            }
        }
        public void ChaseMove(Transform targetTransform, float speed)
        {
            if (!canMove) return;
            if (IsInputMoving) return;
            inputState = InputState.Auto;
            Vector2 autoMoveVelocity = Vector2.MoveTowards(rb.position, targetTransform.position, speed * Time.deltaTime);
            isAutoMoving = autoMoveVelocity != Vector2.zero;
            if (isAutoMoving)
                rb.MovePosition(autoMoveVelocity);
            // Vector2 targetPos = targetTransform.position;
            // Vector2 autoMoveDir = (targetPos - rb.position).normalized;
            // ChaseMove(autoMoveDir, speed);
        }

    }
}