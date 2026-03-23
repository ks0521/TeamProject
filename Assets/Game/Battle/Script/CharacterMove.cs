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
        public bool IsInputMoving { get; private set; }
        public bool isAutoMoving;
        public void Init(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        bool IsMovingCheck(float x, float y, float speed)
        {
            if (x == 0 && y == 0)
            {
                moveVelocity = Vector2.zero;
                return false;
            }
            moveVelocity = new Vector2(x, y) * speed;
            return true;
        }
        public void UpdateMoveInput(float speed)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            IsInputMoving = IsMovingCheck(x, y, speed);
        }
        public void VFixedMove()
        {

        }
        public void VChaseMove(Transform targetTransform, float speed)
        {
            if (IsInputMoving) return;
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
            if (IsInputMoving) return;
            isAutoMoving = dis != Vector2.zero;
            if (isAutoMoving)
                rb.velocity = dis;
            else
                rb.velocity = Vector2.zero;
        }
        public void FixedMove()
        {
            if (IsInputMoving)
                rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
        }
        public void MoveStop()
        {
            IsInputMoving = false;
        }
        public void ChaseMove(Vector2 targetDir, float speed)
        {
            if (IsInputMoving) return;
            isAutoMoving = targetDir != Vector2.zero;
            rb.MovePosition(rb.position + targetDir * speed * Time.deltaTime);
        }
        public void ChaseMove(Transform targetTransform, float speed)
        {
            if (IsInputMoving) return;
            Vector2 autoMoveVelocity = Vector2.MoveTowards(rb.position, targetTransform.position, speed * Time.deltaTime);
            isAutoMoving = autoMoveVelocity != Vector2.zero;
            if (isAutoMoving)
                rb.MovePosition(autoMoveVelocity);
        }

    }
}