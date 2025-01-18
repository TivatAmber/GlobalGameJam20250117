using System;
using System.Numerics;
using Entity.Components.Data;
using Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Entity.Components.DashMonsterComponents
{
    public class DashEffect : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private DashState state;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float recoverSpeed;
        [SerializeField] private float searchRadius;
        [SerializeField] private Vector3 originPos;
        [SerializeField] private float approximateDistance;
        private GameManager gameManager;
        private Transform target;
        
        public DashState State => state;

        private void Awake()
        {
            gameManager = GameManager.Instance;
        }

        private void Update()
        {
            switch (state)
            {
                case DashState.Idle:
                    if (FindPlayer())
                    {
                        target = gameManager.player.transform;
                        state = DashState.Following;
                    }
                    break;
                case DashState.Following:
                    RotateToTarget();
                    transform.position += (target.transform.position - transform.position) * dashSpeed * Time.deltaTime;
                    if (!FindPlayer())
                    {
                        target = null;
                        state = DashState.Recover;
                    }
                    if (DoneAttack()) state = DashState.Recover;
                    break;
                case DashState.Recover:
                    transform.position += (originPos - transform.position) * recoverSpeed * Time.deltaTime;
                    if (AtTheOriginPos()) state = DashState.Idle;
                    break;
            }
        }

        void RotateToTarget()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, directionToTarget);
            transform.rotation = targetRotation;
        }

        bool FindPlayer()
        {
            return Vector3.Distance(gameManager.player.transform.position, originPos) <= searchRadius;
        }

        bool DoneAttack()
        {
            return Vector3.Distance(gameManager.player.transform.position, transform.position) <= approximateDistance;
        }

        bool AtTheOriginPos()
        {
            return Vector3.Distance(originPos, transform.position) <= approximateDistance;
        }

        public void Reset()
        {
            state = DashState.Idle;
            target = null;
        }

        public void SetOrigin(Vector3 value)
        {
            originPos = value;
            transform.position = value;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(originPos, searchRadius);
            if (target)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
#endif
    }
}