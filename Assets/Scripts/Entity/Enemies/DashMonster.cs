using System;
using UnityEngine;
using Entity.Components.BombMonsterComponents;
using Entity.Components.DashMonsterComponents;
using Terrain;

namespace Entity.Enemies
{
    [RequireComponent(typeof(BombExecutionEffect))]
    [RequireComponent(typeof(DashEffect))]
    public class DashMonster : BaseMonster
    {
        private CenterCircle centerCircle;
        private DashEffect dashEffect;

        protected override void Awake()
        {
            base.Awake();
            executionEffect = GetComponent<BombExecutionEffect>();
            centerCircle = GameManager.Instance.centerCircle;
            dashEffect = GetComponent<DashEffect>();
            Reset();
        }

        protected override void Update()
        {
            base.Update();
        }

        public void SetOrigin(Vector3 value)
        {
            dashEffect.SetOrigin(value);
        }

        public override void Reset()
        {
            base.Reset();
            collisionEffect.Reset();
            executionEffect.Reset();
            dashEffect.Reset();
        }
    }
}