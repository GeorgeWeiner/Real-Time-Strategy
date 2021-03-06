using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Targeter targeter;
        [SerializeField] private float chaseRange = 10f;

        [ServerCallback]
        private void Update()
        {
            var target = targeter.GetTarget();
            
            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude > Mathf.Pow(chaseRange, 2))
                {
                    agent.SetDestination(target.transform.position);
                }
                
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
                return;
            }
            
            if (!agent.hasPath) return;
            if (agent.remainingDistance > agent.stoppingDistance) return;
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {   
            targeter.ClearTarget();
            
            if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas)) 
                return;

            agent.SetDestination(hit.position);
        }
    }                                                             
}