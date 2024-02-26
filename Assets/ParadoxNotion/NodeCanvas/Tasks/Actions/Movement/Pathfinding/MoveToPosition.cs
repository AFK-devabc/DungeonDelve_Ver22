using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace NodeCanvas.Tasks.Actions
{

    [Name("Seek (Vector3)")]
    [Category("Movement/Pathfinding")]
    public class MoveToPosition : ActionTask<NavMeshAgent>
    {
        // Đây là di chuyển tới vị trí được cho trước tức là không cập nhật vị trí mới trong lúc di chuyển như MoveToGameObject
        // nên Node này không áp dụng objectApplyRotation vào.
        
        [Tooltip("Vị trí mục tiêu cần di chuyển tới")]
        public BBParameter<Vector3> targetPosition;
        
        public BBParameter<float> speed = 4;
        public BBParameter<float> keepDistance = 0.1f;

        private Vector3? lastRequest;

        protected override string info {
            get { return "Seek " + targetPosition; }
        }

        protected override void OnExecute() {
            agent.speed = speed.value;
            if ( Vector3.Distance(agent.transform.position, targetPosition.value) < agent.stoppingDistance + keepDistance.value ) {
                EndAction(true);
            }
        }

        protected override void OnUpdate() {
            if ( lastRequest != targetPosition.value ) {
                if ( !agent.SetDestination(targetPosition.value) ) {
                    EndAction(false);
                    return;
                }
            }
            
            lastRequest = targetPosition.value;
            if ( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value ) {
                EndAction(true);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( lastRequest != null && agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
            lastRequest = null;
        }
    }
}