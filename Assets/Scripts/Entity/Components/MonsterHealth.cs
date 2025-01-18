using Tools;
using UnityEngine;

namespace Entity.Components
{
    public class MonsterHealth : MonoBehaviour
    {
        public float maxHealth = 1f;
        
        private float currentHealth;
        [SerializeField] [ReadOnly] private bool isDead = false;

        public bool IsDead => isDead;
        public float CurrentHealth => currentHealth;

        private void Awake()
        {
            Reset();
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            currentHealth = Mathf.Max(0, currentHealth - damage);
            if (currentHealth <= 0)
            {
                isDead = true;
            }
        }

        public void Reset()
        {
            currentHealth = maxHealth;
            isDead = false;
        }
    }
}