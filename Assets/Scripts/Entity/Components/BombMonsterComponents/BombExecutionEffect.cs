using System;
using System.Collections;
using Entity.Components.Data;
using Entity.Enemies;
using Terrain;
using UnityEngine;

namespace Entity.Components.BombMonsterComponents
{
    public class BombExecutionEffect : ExecutionEffect
    {
        public float moveSpeed = 0.3f;
        public float executionDistance = 0.5f;
        public float clearRadius = 1f;

        private CenterCircle centerCircle;
        private bool isClockwise;

        private void Awake()
        {
            centerCircle = GameManager.Instance.centerCircle;
        }
        
        public override void Execute()
        {
            base.Execute();

            // 获取玩家位置（作为攻击方向）
            Vector2 playerPosition = GameManager.Instance.player.transform.position;
            Vector2 attackDirection = (playerPosition - (Vector2)transform.position).normalized;

            // 通过叉积判断方向
            Vector2 currentPos = transform.position - centerCircle.center;
            float crossProduct = currentPos.x * attackDirection.y - currentPos.y * attackDirection.x;
            isClockwise = crossProduct < 0;

            StartCoroutine(ExecuteMovement());
        }

        private IEnumerator ExecuteMovement()
        {
            float movedAngle = 0f;
            float startAngle = CalculateAngleFromXAxis(transform.position, centerCircle.transform.position) + Mathf.PI;

            while (Mathf.Abs(movedAngle) < executionDistance)
            {
                float deltaAngle = (isClockwise ? 1 : -1) * moveSpeed * Time.deltaTime;
                movedAngle += deltaAngle;
                Vector2 position = new Vector2(
                    centerCircle.center.x + Mathf.Cos(startAngle + movedAngle) * centerCircle.orbitRadius,
                    centerCircle.center.y + Mathf.Sin(startAngle + movedAngle) * centerCircle.orbitRadius
                );
                transform.position = new Vector3(position.x, position.y, 0);
                ClearMonstersInPath();
                
                yield return null;
            }

            doneExecution = true;
        }

        private void ClearMonstersInPath()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, clearRadius);
        
            foreach (var collider in colliders)
            {
                if (collider.gameObject == gameObject) continue;

                var monster = collider.GetComponent<BaseMonster>();
                if (monster && !monster.IsDead)
                {
                    monster.TakeHit(AttackResult.Normal);
                }
            }
        }
        
        public static float CalculateAngleFromXAxis(Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 direction = endPoint - startPoint;
            return Mathf.Atan2(direction.y, direction.x);
        }
    }
}