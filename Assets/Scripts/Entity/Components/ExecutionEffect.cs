using UnityEngine;

namespace Entity.Components
{
    public class ExecutionEffect: MonoBehaviour
    {
        protected bool isExecuting = false;
        protected bool doneExecution;

        public bool DoneExecution => doneExecution;
        
        public virtual void Execute()
        {
            if (isExecuting) return;
            isExecuting = true;
        }

        public virtual void NormalKill()
        {
            if (isExecuting) return;
            isExecuting = true;
            doneExecution = true;
        }
        
        public void Reset()
        {
            isExecuting = false;
            doneExecution = false;
            StopAllCoroutines();
        }
    }
}