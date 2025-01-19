using System;
using UnityEngine;
using Entity.Components;
using Entity.Components.BombMonsterComponents;
using Entity.Components.Data;
using Entity.Components.ShieldMonsterComponents;
using Terrain;

namespace Entity.Enemies
{
    [RequireComponent(typeof(OrbitMovement))]
    [RequireComponent(typeof(BombExecutionEffect))]
    [RequireComponent(typeof(ShieldEffect))]
    public class ShieldMonster : BaseMonster
    {
        private OrbitMovement orbitMovement;
        private CenterCircle centerCircle;
        private ShieldEffect shieldEffect;

        protected override void Awake()
        {
            base.Awake();
            orbitMovement = GetComponent<OrbitMovement>();
            executionEffect = GetComponent<BombExecutionEffect>();
            shieldEffect = GetComponent<ShieldEffect>();
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
            shieldEffect.Reset();
        }
        
        public override void TakeHit(AttackResult hitResult, float damage = 1f)
        {
            if (health.IsDead || (hitResult != AttackResult.Perfect && hitResult != AttackResult.Normal &&
                                  hitResult != AttackResult.Puncture))
                return;
            
            AudioSource.PlayClipAtPoint(hitSound, GameManager.Instance.transform.position);
            if (shieldEffect.State == ShieldState.Shield &&
                hitResult is AttackResult.Perfect or AttackResult.Normal)
                return;

            health.TakeDamage(damage);

            if (health.IsDead)
            {
                monsterAnimator.PlayHitAnimation(hitResult);
                if (hitResult == AttackResult.Perfect) Execute();
                else NormalExecute();
                shouldPlayDeathAnimation = true;
            }
        }
        
        public void SetNowAngle(float value)
        {
            orbitMovement.SetCurrentAngle(value);
        }
    }
}