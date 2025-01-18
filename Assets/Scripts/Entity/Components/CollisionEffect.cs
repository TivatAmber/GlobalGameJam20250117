using System;
using UnityEngine;

namespace Entity.Components
{
    public class CollisionEffect : MonoBehaviour
    {
        public float incrementInterval = 1.0f;
        public float chaosIncrement = 20f;
        private bool canTrigger = true;
        private float nowIncrementInterval = 0.0f;

        private void Update()
        {
            if (nowIncrementInterval < incrementInterval)
                nowIncrementInterval += Time.deltaTime;
        }

        public void OnCollisionWithPlayer(Player player)
        {
            if (!canTrigger) return;
            if (nowIncrementInterval < incrementInterval) return;
            nowIncrementInterval = 0.0f;
            
            var displacementHandler = player.GetComponent<DisplacementHandler>();
            if (!displacementHandler) return;

            displacementHandler.TakeDamage(chaosIncrement);
        }

        public void SetCanTrigger(bool value)
        {
            canTrigger = value;
        }

        public void Reset()
        {
            canTrigger = true;
            nowIncrementInterval = incrementInterval;
        }
    }
}