using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Pathfinding")]
    [Description("Di chuyển đối tượng tránh xa mục tiêu và trả về 2 giá trị: -1 đang di chuyển khỏi mục tiêu, 0 đã di chuyển xong")]
    public class Flee : ActionTask<NavMeshAgent>    
    {
        [RequiredField, Tooltip("Đối tượng nào sẽ áp dụng Rotation khi di chuyển")] 
        public BBParameter<GameObject> objectApplyRotation;
        
        [RequiredField, Tooltip("The target to flee from.")]
        public BBParameter<GameObject> target;
        [Tooltip("The speed to flee.")]
        public BBParameter<float> speed = 4f;
        [Tooltip("Khoảng cách đối tượng tránh xa mục tiêu.")]
        public BBParameter<float> fledDistance = 10f;
        [Tooltip("A distance to look away from the target for valid flee destination.")]
        public BBParameter<float> lookAhead = 2f;
        
        [Space, Tooltip("Giá trị lưu vào"), RequiredField]
        public BBParameter<float> saveFoundParameter;
        
        protected override string info {
            get { return string.Format("Flee from {0}", target); }
        }

        protected override void OnExecute() {
            if ( target.value == null ) { EndAction(false); return; }
            agent.speed = speed.value;
            if ( ( agent.transform.position - target.value.transform.position ).magnitude >= fledDistance.value ) {
                EndAction(true);
            }
        }

        protected override void OnUpdate() {
            if ( target.value == null ) { EndAction(false); return; }
            var targetPos = target.value.transform.position;
            if ( ( agent.transform.position - targetPos ).magnitude >= fledDistance.value ) {
                
                saveFoundParameter.value = 0;
                EndAction(true);
                return;
            }

            var lookRot = Quaternion.LookRotation(target.value.transform.position - agent.transform.position);
            objectApplyRotation.value.transform.rotation = Quaternion.RotateTowards(objectApplyRotation.value.transform.rotation,
                Quaternion.Euler(0, lookRot.eulerAngles.y, 0), 15f);
            saveFoundParameter.value = -1;
            
            var fleePos = targetPos + ( agent.transform.position - targetPos ).normalized * ( fledDistance.value + lookAhead.value + agent.stoppingDistance );
            if ( !agent.SetDestination(fleePos) ) {
                EndAction(false);
            } 
        }


        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
        }
    }
}