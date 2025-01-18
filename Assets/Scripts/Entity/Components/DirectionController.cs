using System;
using UnityEngine;
using Terrain;

namespace Entity.Components
{
    /// <summary>
    /// 方向控制器，负责处理实体的朝向
    /// </summary>
    public class DirectionController : MonoBehaviour
    {
        private Vector2 facingDirection = Vector2.right;  // 当前朝向
        private CenterCircle centerCircle;              
        private bool isClockwise = true;                // 是否逆时针旋转

        public bool IsClockwise => isClockwise;
        
        private void Start()
        {
            // 获取中心圆引用
            centerCircle = GameManager.Instance.centerCircle;
            UpdateTangentDirection();
        }
        
        private void Update()
        {
            // 每帧更新切线方向
            UpdateTangentDirection();
        }
        
        /// <summary>
        /// 更新切线方向
        /// </summary>
        private void UpdateTangentDirection()
        {
            if (centerCircle == null) return;
            
            // 计算当前位置到圆心的向量
            Vector2 currentPos = transform.position;
            Vector2 centerPos = centerCircle.center;
            Vector2 toCenter = centerPos - currentPos;
            
            // 根据旋转方向计算切线
            facingDirection = isClockwise 
                ? new Vector2(toCenter.y, -toCenter.x).normalized    // 顺时针切线
                : new Vector2(-toCenter.y, toCenter.x).normalized;   // 逆时针切线
            
            // 更新物体旋转
            float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// 设置旋转方向
        /// </summary>
        /// <param name="clockwise">是否逆时针</param>
        public void SetDirection(bool clockwise)
        {
            isClockwise = clockwise;
            UpdateTangentDirection();
        }

        public Vector2 GetFacingDirection()
        {
            return facingDirection;
        }

        public void Reset()
        {
            isClockwise = false;
            UpdateTangentDirection();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (centerCircle == null) return;
            
            Gizmos.color = Color.blue;
            Vector3 position = transform.position;
            Gizmos.DrawLine(position, position + (Vector3)(facingDirection * 2f));
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, (Vector3)centerCircle.center);
        }
        #endif
    }
}