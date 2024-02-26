using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace NodeCanvas.Tasks.Actions
{

    [Name("Seek (GameObject)")]
    [Category("Movement/Pathfinding")]
    public class MoveToGameObject : ActionTask<NavMeshAgent>
    {
        // Vì xoay object vào target cần Update mỗi frame nên objectApplyRotation chỉ áp dụng cho Node MoveToGameObject,
        // không áp dụng cho node MoveToPosition
        
        [RequiredField, Tooltip("Đối tượng nào sẽ áp dụng Rotation khi di chuyển")] 
        public BBParameter<GameObject> objectApplyRotation;
        [RequiredField, Tooltip("Vị trí mục tiêu cần di chuyển tới")]
        public BBParameter<GameObject> target;
        public BBParameter<float> speed = 4;
        public BBParameter<float> keepDistance = 0.1f;

        
        private Vector3? lastRequest;

        protected override string info {
            get { return "Seek " + target; }
        }

        protected override void OnExecute() {
            if ( target.value == null ) { EndAction(false); return; }
            agent.speed = speed.value;
            if ( Vector3.Distance(agent.transform.position, target.value.transform.position) <= agent.stoppingDistance + keepDistance.value ) {
                EndAction(true);
                return;
            }
        }

        protected override void OnUpdate()
        {
            if ( target.value == null ) { EndAction(false); return; }
            var pos = target.value.transform.position;
            if ( lastRequest != pos ) {
                if ( !agent.SetDestination(pos) ) {
                    EndAction(false);
                    return;
                }
            }

            lastRequest = pos;

            var lookRot = Quaternion.LookRotation(target.value.transform.position - agent.transform.position);
            objectApplyRotation.value.transform.rotation = Quaternion.RotateTowards(objectApplyRotation.value.transform.rotation,
                Quaternion.Euler(0, lookRot.eulerAngles.y, 0), 15f);
            
            if ( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value ) {
                EndAction(true);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
            lastRequest = null;
        }

        public override void OnDrawGizmosSelected() {
            if ( target.value != null ) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(target.value.transform.position, keepDistance.value);
            }
        }
    }
}