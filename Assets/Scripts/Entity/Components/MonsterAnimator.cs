using UnityEngine;
using Entity.Components.Data;

namespace Entity.Components
{
    [RequireComponent(typeof(Animator))]
    public class MonsterAnimator : MonoBehaviour
    {
        [Header("Animation Parameters")]
        public string perfectHitTrigger = "PerfectHit";
        public string normalHitTrigger = "NormalHit";
        public string deathTrigger = "Death";
    
        [Header("Animation States")]
        public string perfectHitState = "PerfectHit";
        public string normalHitState = "NormalHit";
        public string deathState = "Death";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void PlayHitAnimation(AttackResult hitResult)
        {
            if (!animator || IsPlayingHitAnimation() || IsPlayingDeathAnimation()) return;

            switch (hitResult)
            {
                case AttackResult.Perfect:
                    animator.SetTrigger(perfectHitTrigger);
                    break;
                case AttackResult.Normal:
                    animator.SetTrigger(normalHitTrigger);
                    break;
            }
        }

        public void PlayDeathAnimation()
        {
            if (!animator || IsPlayingDeathAnimation()) return;
            animator.SetTrigger(deathTrigger);
        }

        public bool IsPlayingHitAnimation()
        {
            if (!animator) return false;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(perfectHitState) || stateInfo.IsName(normalHitState);
        }

        public bool IsPlayingDeathAnimation()
        {
            if (!animator) return false;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(deathState);
        }

        public bool DoneDeathAnimation()
        {
            if (!animator) return false;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(deathState) && 
                stateInfo.normalizedTime >= 1.0f)
            {
                return true;
            }

            return false;
        }

        public void Reset()
        {
            if (!animator) return;
            
            animator.ResetTrigger(perfectHitTrigger);
            animator.ResetTrigger(normalHitTrigger);
            animator.ResetTrigger(deathTrigger);
            
            animator.Play("Idle", 0, 0f);
        }
    }
}