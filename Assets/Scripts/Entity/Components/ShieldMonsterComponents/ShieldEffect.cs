using System;
using Entity.Components.Data;
using Tools;
using UnityEngine;

namespace Entity.Components.ShieldMonsterComponents
{
    public class ShieldEffect: MonoBehaviour
    {
        [SerializeField] [ReadOnly] private ShieldState state;
        public float changeInterval = 1.0f;

        [SerializeField] [ReadOnly] private float nowTime = 0.0f;
        
        public ShieldState State => state;

        private void Awake()
        {
            nowTime = 0.0f;
            state = ShieldState.Idle;
        }

        private void Update()
        {
            if (nowTime < changeInterval) nowTime += Time.deltaTime;
            if (nowTime >= changeInterval)
            {
                nowTime = 0.0f;
                if (state == ShieldState.Idle) state = ShieldState.Shield;
                else if (state == ShieldState.Shield) state = ShieldState.Idle;
            }
        }

        public void Reset()
        {
            nowTime = 0.0f;
            state = ShieldState.Idle;
        }
    }
}