using System;
using System.Collections;
using Terrain;
using Tools;
using UnityEngine;

namespace Entity.Components
{
    public class DisplacementHandler : MonoBehaviour
    {
        public float maxChaosDisplacement = 2f;  // 最大位置偏移距离
        public float chaosValue = 0f;            // 混乱值(0-100)
        public float recoveryTime = 1f;          // 位置回归时间
        public float toMaxChaosHeightTIme = 0.3f;

        private bool isDisplaced = false;
        private float displacementDistance = 0f;
        private Vector2 originalPosition;
        private Vector2 targetPosition;
        private CenterCircle centerCircle;
        private Coroutine recoveryCoroutine;

        [SerializeField] private AudioClip soundEffect;
        [SerializeField] private AudioClip jumpSoundEffect;

        [Header("Jump Settings")] 
        public float maxJumpTime = 0.2f;
        public float fallDuration = 1.0f;
        public float maxJumpHeight = 5f;

        [SerializeField] [ReadOnly] private bool isJumping = false;
        [SerializeField] [ReadOnly] private bool canJump = true;
        private float currentJumpHeight = 0f;
        private Coroutine jumpCoroutine;
        private Coroutine additionJumpCoroutine;

        [Header("Dive Setting")] 
        public float maxDiveTime = 0.5f;
        public float floatDuration = 0.5f;
        public float maxDiveHeight = 5f;
        public float divingTime = 1.0f;
        
        [SerializeField] [ReadOnly] private bool isDiving = false;
        [SerializeField] [ReadOnly] private bool canDiving = true;
        private float currentDiveHeight = 0f;
        private Coroutine diveCoroutine;

        public bool IsJumping => isJumping;
        public bool IsDiving => isDiving;
        public bool CanJump => canJump;
        public bool CanDiving => canDiving;

        public float ChaosValue => chaosValue;

        private void Start()
        {
            centerCircle = GameManager.Instance.centerCircle;
            originalPosition = transform.position;
        }

        private void Update()
        {
            HandleDisplacement();
        }

        public float GetCurrentRadius()
        {
            float radius = centerCircle.orbitRadius;
            if (isDisplaced)
            {
                radius += displacementDistance;
            }
            return radius;
        }

        public void HandleDisplacement()
        {
            if (!isDisplaced)
                return;
            CheckDeath();
        }

        // 受到伤害时调用
        public void TakeDamage(float damageAmount)
        {
            float previousChaos = chaosValue;
            ModifyChaos(damageAmount);

            // 根据当前混乱值计算新的偏移距离
            float displacementMultiplier = chaosValue / 100f;
            displacementDistance = maxChaosDisplacement * displacementMultiplier;
            isDisplaced = true;
            
            // 如果是增加混乱值，更新位置偏移
            if (damageAmount > 0 && chaosValue >= previousChaos)
            {
                displacementDistance = maxChaosDisplacement * displacementMultiplier;
                
                if (jumpCoroutine != null)
                {
                    StopCoroutine(jumpCoroutine);
                    jumpCoroutine = null;
                    isJumping = false;
                }

                if (recoveryCoroutine != null)
                {
                    StopCoroutine(recoveryCoroutine);
                    recoveryCoroutine = null;
                }

                isDisplaced = true;
                canJump = false;
                canDiving = false;
                recoveryCoroutine = StartCoroutine(RecoverFromDisplacement());
            }
        }

        // 修改混乱值
        public void ModifyChaos(float amount)
        {
            chaosValue = Mathf.Clamp(chaosValue + amount, 0, 100);
        }
        
        // 触发快速上升然后缓慢下落
        public void TriggerJump()
        {
            if (canJump == false) return;
            isDisplaced = true;
            isJumping = true;
            canDiving = false;
            jumpCoroutine = StartCoroutine(JumpRoutine());
        }

        public void TriggerDive()
        {
            if (isDiving) return;
            if (canDiving == false)
            {
                if (jumpCoroutine != null)
                {
                    StopCoroutine(jumpCoroutine);
                    jumpCoroutine = null;
                }
                if (recoveryCoroutine != null)
                {
                    StopCoroutine(recoveryCoroutine);
                    recoveryCoroutine = null;
                }
                
                recoveryCoroutine = StartCoroutine(RecoverFromDisplacement());
                return;
            }
            isDisplaced = true;
            isDiving = true;
            canJump = false;
            diveCoroutine = StartCoroutine(DiveRoutine());
        }

        public void TriggerAdditionJump()
        {
            if (isJumping)
            {
                if (jumpCoroutine != null)
                {
                    StopCoroutine(jumpCoroutine);
                    jumpCoroutine = null;
                }
                if (additionJumpCoroutine != null)
                {
                    StopCoroutine(additionJumpCoroutine);
                    additionJumpCoroutine = null;
                }
                additionJumpCoroutine = StartCoroutine(additionJumpRoutine());
            }
        }

        private IEnumerator RecoverFromDisplacement()
        {
            float elapsedTime = 0f;
            float initialDisplacement = displacementDistance;
            float nowDisplacement = GameManager.Instance.player.transform.position.magnitude - GameManager.Instance.centerCircle.orbitRadius;

            while (elapsedTime < toMaxChaosHeightTIme)
            {
                elapsedTime += Time.deltaTime;

                float t = elapsedTime / toMaxChaosHeightTIme;
                displacementDistance = Mathf.Lerp(nowDisplacement, initialDisplacement, t);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < recoveryTime)
            {
                elapsedTime += Time.deltaTime;
                
                // 使用平滑的插值回到原始位置
                float t = elapsedTime / recoveryTime;
                displacementDistance = Mathf.Lerp(initialDisplacement, 0, t);
                yield return null;
            }

            isDisplaced = false;
            isJumping = false;
            isDiving = false;
            canJump = true;
            canDiving = true;
            displacementDistance = 0f;
            recoveryCoroutine = null;
        }
        
        private IEnumerator JumpRoutine()
        {
            float initialDistance = displacementDistance;
            float targetJumpHeight = maxJumpHeight;
            AudioSource.PlayClipAtPoint(jumpSoundEffect, GameManager.Instance.transform.position);
    
            // 快速上升阶段
            float jumpTime = 0f;
            while (jumpTime < maxJumpTime)
            {
                jumpTime += Time.deltaTime;
                float t = jumpTime / maxJumpTime;
                displacementDistance = Mathf.Lerp(initialDistance, targetJumpHeight, t);
                yield return null;
            }

            // 缓慢下落阶段
            float fallTime = 0f;
            while (fallTime < fallDuration)
            {
                fallTime += Time.deltaTime;
                float t = fallTime / fallDuration;
                displacementDistance = Mathf.Lerp(targetJumpHeight, 0f, t);
                yield return null;
            }

            isDisplaced = false;
            isJumping = false;
            canDiving = true;
            displacementDistance = 0f;
            jumpCoroutine = null;
        }
        
        private IEnumerator additionJumpRoutine()
        {
            float initialDistance = displacementDistance;
            float targetJumpHeight = maxJumpHeight;
            AudioSource.PlayClipAtPoint(jumpSoundEffect, GameManager.Instance.transform.position);
    
            // 快速上升阶段
            float jumpTime = 0f;
            while (jumpTime < maxJumpTime)
            {
                jumpTime += Time.deltaTime;
                float t = jumpTime / maxJumpTime;
                displacementDistance = Mathf.Lerp(initialDistance, targetJumpHeight, t);
                yield return null;
            }

            // 缓慢下落阶段
            float fallTime = 0f;
            while (fallTime < fallDuration)
            {
                fallTime += Time.deltaTime;
                float t = fallTime / fallDuration;
                displacementDistance = Mathf.Lerp(targetJumpHeight, 0f, t);
                yield return null;
            }

            isDisplaced = false;
            isJumping = false;
            canDiving = true;
            displacementDistance = 0f;
            additionJumpCoroutine = null;
        }
        
        private IEnumerator DiveRoutine()
        {
            float initialDistance = displacementDistance;
            float targetDiveHeight = -maxDiveHeight;
    
            // 快速下潜阶段
            float diveTime = 0f;
            while (diveTime < maxDiveTime)
            {
                diveTime += Time.deltaTime;
                float t = diveTime / maxDiveTime;
                displacementDistance = Mathf.Lerp(initialDistance, targetDiveHeight, t);
                yield return null;
            }
                
            // 潜水阶段
            yield return new WaitForSeconds(diveTime);
            
            // 快速回升阶段
            float floatTime = 0f;
            while (floatTime < floatDuration)
            {
                floatTime += Time.deltaTime;
                float t = floatTime / floatDuration;
                displacementDistance = Mathf.Lerp(targetDiveHeight, 0f, t);
                yield return null;
            }

            isDisplaced = false;
            isDiving = false;
            canJump = true;
            displacementDistance = 0f;
            diveCoroutine = null;
        }
        
        private void CheckDeath()
        {
            if (Vector3.Distance(centerCircle.center, transform.position) > centerCircle.dieRadius && ChaosValue >= 100)
            {
                Die();
                GameManager.Instance.PauseGame();
            }
        }

        private void Die()
        {
            Debug.Log("Player died!");
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            chaosValue = 0f;
            isDisplaced = false;
            isJumping = false;
            isDiving = false;
            canJump = true;
            canDiving = true;
            displacementDistance = 0f;
            StopAllCoroutines();
            recoveryCoroutine = null;
            transform.position = originalPosition;
        }
        
        public void PlaySound()
        {
            // 最简单的播放方式，不需要AudioSource组件
            AudioSource.PlayClipAtPoint(soundEffect, GameManager.Instance.transform.position);
        }
    }
}