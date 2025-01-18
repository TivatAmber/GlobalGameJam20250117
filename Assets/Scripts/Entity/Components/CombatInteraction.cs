using System;
using Entity.Components.Data;
using Entity.Enemies;
using Tools;
using UnityEngine;

namespace Entity.Components 
{
    [RequireComponent(typeof(InputManager))]
    [RequireComponent(typeof(OrbitMovement))]
    [RequireComponent(typeof(DisplacementHandler))]
    [RequireComponent(typeof(DirectionController))]
    public class CombatInteraction : MonoBehaviour 
    {
        #region 战斗参数
        [Header("战斗设置")]
        public float perfectHitWindow = 20f;    // 完美击中的角度窗口
        public float normalHitWindow = 30f;     // 普通击中的角度窗口
        public float attackDamage = 5f;         // 失败攻击受到的紊乱
        public float recovery = 10f;
        public float lessRecovery = 5f;
        #endregion
        
        #region 目标敌人
        [SerializeField] [ReadOnly] private BaseMonster currentTarget;
        public BaseMonster CurrentTarget => currentTarget;
        [SerializeField] [ReadOnly] private float angleToCurrentTarget;
        public float AngleToCurrentTarget => angleToCurrentTarget;
        #endregion

        #region 组件引用
        private InputManager inputManager;
        private OrbitMovement orbitMovement;
        private DisplacementHandler displacementHandler;
        private DirectionController directionController;
        private GameManager gameManager;
        #endregion

        private void Awake()
        {
            inputManager = GetComponent<InputManager>();
            orbitMovement = GetComponent<OrbitMovement>();
            displacementHandler = GetComponent<DisplacementHandler>();
            directionController = GetComponent<DirectionController>();
            gameManager = GameManager.Instance;
            
            inputManager.OnCombatAction += HandleCombatAction;
        }

        private void HandleCombatAction(CombatAction action)
        {
            switch (action)
            {
                case CombatAction.AttackLeft:
                case CombatAction.AttackRight:
                    HandleHorizontalAttack(action == CombatAction.AttackLeft);
                    break;
                
                case CombatAction.AttackUp:
                    HandleUpwardAttack();
                    break;
                
                case CombatAction.AttackDown:
                    HandleDownwardAttack();
                    break;
            }
        }

        private void HandleHorizontalAttack(bool isLeft)
        {
            AttackResult result = EvaluateAttack();
            switch (result)
            {
                case AttackResult.Perfect:
                    currentTarget.TakeHit(AttackResult.Perfect);
                    directionController.SetDirection(isLeft);
                    orbitMovement.ToDirection(isLeft);
                    break;
                case AttackResult.Normal:
                    currentTarget.TakeHit(AttackResult.Normal);
                    directionController.SetDirection(isLeft);
                    orbitMovement.ToDirection(isLeft);
                    break;
                case AttackResult.Fail:
                    displacementHandler.TakeDamage(attackDamage);
                    break;
            }
        }

        private void HandleUpwardAttack()
        {
            AttackResult result = EvaluateAttack();
            
            if (result == AttackResult.Perfect)
            {
                // 1. 玩家进入泡泡，减少紊乱
                // 2. 玩家如果在自发向上运动则立即开始下落，不影响被打产生的速度
                // 3. 怪死掉
                displacementHandler.ModifyChaos(-recovery);
                displacementHandler.TriggerDive();
            } else if (result == AttackResult.Normal)
            {    
                // 1. 玩家进入泡泡，减少更少紊乱
                // 2. 玩家如果在自发向上运动则立即开始下落，不影响被打产生的速
                displacementHandler.ModifyChaos(-lessRecovery);
                displacementHandler.TriggerDive();
            } else if (result == AttackResult.Fail)
            {
                // 1. 玩家紊乱
                // 2. 怪攻击
                displacementHandler.TakeDamage(attackDamage);
            }
        }

        private void HandleDownwardAttack()
        {
            if (displacementHandler.IsJumping)
            {
                AttackResult result = EvaluateAttack();

                switch (result)
                {
                    case AttackResult.Perfect:
                        //     1. 怪物爆符文
                        //     2. 玩家向上运动，不改变方向
                        currentTarget.TakeHit(AttackResult.Perfect);
                        displacementHandler.TriggerAdditionJump();
                        break;
                    case AttackResult.Normal:
                        //     1. 怪正常死亡
                        //     2. 玩家自由落体，不改变方向
                        currentTarget.TakeHit(AttackResult.Normal);
                        break;
                    case AttackResult.Fail:
                        displacementHandler.TakeDamage(attackDamage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                } 
            }
            else
            {
                displacementHandler.TriggerJump();
            }
        }

        private AttackResult EvaluateAttack()
        {
            if (!currentTarget) return AttackResult.Fail;
            float absAngle = Mathf.Abs(angleToCurrentTarget);
            
            if (absAngle <= perfectHitWindow)
                return AttackResult.Perfect;
            
            if (absAngle <= normalHitWindow)
                return AttackResult.Normal;
            
            return AttackResult.Fail;
        }
        
        private void Update()
        {
            UpdateTargetAndAngle();
        }

        private void UpdateTargetAndAngle()
        {
            if (!gameManager || gameManager.monsters == null || gameManager.monsters.Count == 0)
            {
                currentTarget = null;
                angleToCurrentTarget = float.MaxValue;
                return;
            }

            Vector3 centerPos = gameManager.centerCircle.transform.position;
            Vector3 playerPos = transform.position;
            
            // 计算玩家和怪物的角度
            float playerAngle = GetAngleFromCenter(playerPos, centerPos);
            bool isClockwise = directionController.IsClockwise;
            
            // 找到最近的怪物
            BaseMonster nearestAngleMonster = null;
            float smallestAngleDiff = float.MaxValue;
            float nearestAngle = 0f;
            
            foreach (BaseMonster monster in gameManager.monsters)
            {
                if (monster == null || !monster.gameObject.activeSelf) continue;
                
                float monsterAngle = GetAngleFromCenter(monster.transform.position, centerPos);
                
                // 计算需要前进的角度
                float angleDiff = monsterAngle - playerAngle;
                
                // 根据移动方向调整角度
                if (isClockwise)
                {
                    // 如果顺时针移动且目标在后面，需要绕一圈
                    if (angleDiff < 0)
                    {
                        angleDiff += 360f;
                    }
                }
                else
                {
                    // 如果逆时针移动且目标在前面，需要绕一圈
                    if (angleDiff > 0)
                    {
                        angleDiff -= 360f;
                    }
                }

                float absAngleDiff = Mathf.Abs(angleDiff);
                if (absAngleDiff < Mathf.Abs(smallestAngleDiff))
                {
                    smallestAngleDiff = angleDiff;
                    nearestAngleMonster = monster;
                    nearestAngle = angleDiff;
                }
            }
            
            // 更新当前目标和角度
            currentTarget = nearestAngleMonster;
            angleToCurrentTarget = nearestAngle;
        }

        private float GetAngleFromCenter(Vector3 position, Vector3 center)
        {
            Vector3 dirFromCenter = position - center;
            return Mathf.Atan2(dirFromCenter.y, dirFromCenter.x) * Mathf.Rad2Deg;
        }
    }
}