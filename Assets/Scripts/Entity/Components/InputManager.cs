using UnityEngine;
using System;
using Entity.Components.Data;

namespace Entity.Components
{
    public class InputManager : MonoBehaviour
    {
        // 战斗动作事件
        public event Action<CombatAction> OnCombatAction;
        
        [Header("按键设置")]
        public KeyCode attackLeft = KeyCode.A;    // 左
        public KeyCode attackRight = KeyCode.D;   // 右
        public KeyCode attackDown = KeyCode.S;    // 下
        public KeyCode attackUp = KeyCode.W;      // 上
        public KeyCode runeConfirm = KeyCode.K;   // 符文确认
        
        private bool isEnabled = true;            // 输入是否启用
        private float lastInputTime = 0f;         // 上次输入时间
        public float inputCooldown = 0.1f;        // 输入冷却时间
        
        private InputState currentState = new InputState();

        private void Update()
        {
            if (!isEnabled) return;
            
            // 检查输入冷却
            float currentTime = Time.time;
            if (currentTime - lastInputTime < inputCooldown) return;

            // 重置当前帧的触发状态
            currentState.IsActionTriggered = false;

            // 检测并处理输入
            CombatAction newAction = CheckInput();
            if (newAction != CombatAction.None)
            {
                currentState.CurrentAction = newAction;
                currentState.IsActionTriggered = true;
                OnCombatAction?.Invoke(newAction);
                lastInputTime = currentTime;
            }
        }

        /// <summary>
        /// 检查当前帧的输入
        /// </summary>
        private CombatAction CheckInput()
        {
            if (Input.GetKeyDown(attackLeft))
                return CombatAction.AttackLeft;
            if (Input.GetKeyDown(attackRight))
                return CombatAction.AttackRight;
            if (Input.GetKeyDown(attackDown))
                return CombatAction.AttackDown;
            if (Input.GetKeyDown(attackUp))
                return CombatAction.AttackUp;
            if (Input.GetKeyDown(runeConfirm))
                return CombatAction.RuneConfirm;
            
            return CombatAction.None;
        }

        public void EnableInput()
        {
            isEnabled = true;
        }

        public void DisableInput()
        {
            isEnabled = false;
            ResetState();
        }

        public InputState GetCurrentState()
        {
            return currentState;
        }

        private void ResetState()
        {
            currentState.CurrentAction = CombatAction.None;
            currentState.IsActionTriggered = false;
            lastInputTime = 0f;
        }

        private void OnDestroy()
        {
            // 清除所有事件监听
            OnCombatAction = null;
        }
    }
}