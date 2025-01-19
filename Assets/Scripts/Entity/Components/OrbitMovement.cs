using System;
using UnityEngine;
using Terrain;
using Tools;

namespace Entity.Components
{
    public class OrbitMovement : MonoBehaviour
    {
        [Header("移动设置")] 
        public float baseOrbitSpeed = 5f;        // 基础轨道速度
        public float maxOrbitSpeed = 8f;         // 最大轨道速度
        public float speedIncreaseRate = 0.5f;   // 速度增长率
        public float startAngle = 180f;
        
        private bool isClockwise = true;
        private float currentOrbitSpeed;         // 当前轨道速度
        private float currentAngle = 0f;         // 当前角度——弧度
        private CenterCircle centerCircle;       
        [SerializeField] [ReadOnly] private float angle = 0f;

        public float CurrentAngle => currentAngle; 

        private void Start()
        {
            centerCircle = GameManager.Instance.centerCircle;
            currentOrbitSpeed = baseOrbitSpeed;
            currentAngle = startAngle / 180.0f * Mathf.PI;
        }

        /// <summary>
        /// 更新移动位置
        /// </summary>
        /// <param name="radius">轨道半径</param>
        public void UpdateMovement(float radius)
        {
            // 更新角度
            currentAngle += (isClockwise ? 1 : -1) *  currentOrbitSpeed * Time.deltaTime;
            angle = currentAngle * Mathf.Rad2Deg;
            
            Vector2 position = new Vector2(
                centerCircle.center.x + Mathf.Cos(currentAngle) * radius,
                centerCircle.center.y + Mathf.Sin(currentAngle) * radius
            );
            transform.position = new Vector3(position.x, position.y, 0);
            
            if (Mathf.Abs(currentOrbitSpeed) < maxOrbitSpeed)
            {
                float sign = Mathf.Sign(currentOrbitSpeed);
                currentOrbitSpeed += sign * speedIncreaseRate * Time.deltaTime;
                currentOrbitSpeed = Mathf.Clamp(currentOrbitSpeed, -maxOrbitSpeed, maxOrbitSpeed);
            }
        }
        
        public void ToDirection(bool clockwise)
        {
            isClockwise = clockwise;
        }

        public void Reset()
        {
            currentAngle = 0f;
            currentOrbitSpeed = baseOrbitSpeed;
            UpdateMovement(centerCircle.orbitRadius);
        }

        public void SetCurrentAngle(float value)
        {
            isClockwise = true;
            startAngle = value * Mathf.Rad2Deg;
            currentAngle = startAngle;
        }
    }
}