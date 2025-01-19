using System;
using Entity.Components;
using Entity.Components.Data;
using UnityEngine;
using Tools;

namespace Entity.Enemies
{
    [RequireComponent(typeof(MonsterHealth))]
    [RequireComponent(typeof(MonsterAnimator))]
    [RequireComponent(typeof(CollisionEffect))]
    [RequireComponent(typeof(ExecutionEffect))]
    public class BaseMonster : MonoBehaviour
    {
        protected MonsterHealth health;
        protected MonsterAnimator monsterAnimator;
        protected CollisionEffect collisionEffect;
        protected ExecutionEffect executionEffect;
        protected bool shouldPlayDeathAnimation = false;
        protected float score = 100.0f;

        [SerializeField] protected AudioClip hitSound;

        public bool IsDead => health.IsDead;

        protected virtual void Awake()
        {
            health = GetComponent<MonsterHealth>();
            monsterAnimator = GetComponent<MonsterAnimator>();
            collisionEffect = GetComponent<CollisionEffect>();
        }

        public virtual void TakeHit(AttackResult hitResult, float damage = 1f)
        {
            if (health.IsDead || (hitResult != AttackResult.Perfect && hitResult != AttackResult.Normal && hitResult != AttackResult.Puncture))
                return;

            health.TakeDamage(damage);
            AudioSource.PlayClipAtPoint(hitSound, GameManager.Instance.transform.position);

            if (health.IsDead)
            {
                monsterAnimator.PlayHitAnimation(hitResult);
                if (hitResult == AttackResult.Perfect) Execute();
                else NormalExecute();
                shouldPlayDeathAnimation = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Player>(out var player)) OnCollisionWithPlayer(player);
        }

        public void OnCollisionWithPlayer(Player player)
        {
            if (health.IsDead) return;
            collisionEffect.OnCollisionWithPlayer(player);
        }

        protected void Execute()
        {
            GameManager.Instance.Score += score * 1.5f;
            collisionEffect.SetCanTrigger(false);
            executionEffect.Execute();
        }

        protected void NormalExecute()
        {
            GameManager.Instance.Score += score;
            collisionEffect.SetCanTrigger(false);
            executionEffect.NormalKill();
        }
        
        protected virtual void Update()
        {
            if (executionEffect.DoneExecution) 
                MonsterPool.Instance.ReturnMonster(gameObject);
            
            if (shouldPlayDeathAnimation && executionEffect.DoneExecution)
            {
                shouldPlayDeathAnimation = false;
                Die();
            }
            else if (health.IsDead && monsterAnimator.DoneDeathAnimation())
            {
                MonsterPool.Instance.ReturnMonster(gameObject);
            }
        }

        protected virtual void Die()
        {
            if (!health.IsDead) return;
            monsterAnimator.PlayDeathAnimation();
        }

        public virtual void Reset()
        {
            health.Reset();
            monsterAnimator.Reset();
            shouldPlayDeathAnimation = false;
        }
    }
}