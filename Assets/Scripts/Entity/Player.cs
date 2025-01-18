using UnityEngine;
using Entity.Components;

namespace Entity
{
    [RequireComponent(typeof(OrbitMovement))]
    [RequireComponent(typeof(DirectionController))]
    [RequireComponent(typeof(DisplacementHandler))]
    [RequireComponent(typeof(CombatInteraction))]
    public class Player : MonoBehaviour
    {
        private OrbitMovement orbitMovement;
        private DirectionController directionController;
        private DisplacementHandler displacementHandler;
        private CombatInteraction combatInteraction;

        public float CurrentAngle => orbitMovement.CurrentAngle;
        
        private void Awake()
        {
            // 获取所有必需的组件
            orbitMovement ??= GetComponent<OrbitMovement>();
            directionController ??= GetComponent<DirectionController>();
            displacementHandler ??= GetComponent<DisplacementHandler>();
            combatInteraction ??= GetComponent<CombatInteraction>();
        }
        
        private void Update()
        {
            // 更新各个组件
            float currentRadius = displacementHandler.GetCurrentRadius();
            orbitMovement.UpdateMovement(currentRadius);
            displacementHandler.HandleDisplacement();
        }
        
        public void TakeDamage(float damageAmount)
        {
            displacementHandler.TakeDamage(damageAmount);
        }
        
        public void ResetPlayer()
        {
            orbitMovement.Reset();
            directionController.Reset();
            displacementHandler.Reset();
            gameObject.SetActive(true);
        }
    }
}