using System;
using UnityEngine;
using Entity.Components;
using Entity.Components.BombMonsterComponents;
using Terrain;

namespace Entity.Enemies
{
    [RequireComponent(typeof(OrbitMovement))]
    [RequireComponent(typeof(BombExecutionEffect))]    
    [RequireComponent(typeof(BombExecutionEffect))]
    public class BombMonster : BaseMonster
    {
        private OrbitMovement orbitMovement;
        private CenterCircle centerCircle;

        protected override void Awake()
        {
            base.Awake();
            orbitMovement = GetComponent<OrbitMovement>();
            executionEffect = GetComponent<BombExecutionEffect>();
            centerCircle = GameManager.Instance.centerCircle;
            Reset();
        }

        protected override void Update()
        {
            base.Update();
            if (!health.IsDead)
                orbitMovement.UpdateMovement(centerCircle.orbitRadius);
        }

        public override void Reset()
        {
            base.Reset();
            collisionEffect.Reset();
            executionEffect.Reset();
        }

        public void SetNowAngle(float value)
        {
            orbitMovement.SetCurrentAngle(value);
        }
    }
}