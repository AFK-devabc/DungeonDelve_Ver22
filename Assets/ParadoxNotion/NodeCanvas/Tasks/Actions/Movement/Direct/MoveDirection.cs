using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace NodeCanvas.Tasks.Actions
{
    [Category("Movement/Pathfinding")]
    [Description("Di chuyển theo hướng được chỉ định")]
    public class MoveDirection : ActionTask<NavMeshAgent>
    {

        public enum Direction
        {
            Left,
            Right,
            Back,
            Forward
        }
        
        [RequiredField, Tooltip("Đối tượng nào sẽ áp dụng Rotation khi di chuyển")] 
        public BBParameter<GameObject> objectApplyRotation;
        
        [Tooltip("Hướng di chuyển")]
        public Direction moveDirection;
        
        [Tooltip("The speed to flee.")]
        public BBParameter<float> speed = 4f;
        [Tooltip("Khoảng cách đối tượng tránh xa mục tiêu.")]
        public BBParameter<float> stoppingDistance = 10f;
        [Tooltip("A distance to look away from the target for valid flee destination.")]
        public BBParameter<float> lookAhead = 2f;
        
        [Space, Tooltip("Giá trị lưu vào"), RequiredField]
        public BBParameter<float> saveFoundParameter;

        private Vector3 targetPos;
        
        protected override void OnExecute() 
        {
            agent.speed = speed.value;
            if ((agent.transform.position - targetPos).magnitude >= stoppingDistance.value ) {
                EndAction(true);
            }
        }

        protected override void OnUpdate()
        {
            targetPos = GetDirection();
            if ( ( agent.transform.position - targetPos ).magnitude >= stoppingDistance.value ) {
                EndAction(true);
                saveFoundParameter.value = 0;
                return;
            }
            
            var fleePos = targetPos + ( agent.transform.position - targetPos ).normalized * ( stoppingDistance.value + lookAhead.value + agent.stoppingDistance );
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

        
        private Vector3 GetDirection()
        {
            var moveDirectionVector = Vector3.zero;
            switch (moveDirection)
            {
                case Direction.Left:
                    moveDirectionVector = -objectApplyRotation.value.transform.right;
                    saveFoundParameter.value = -1;
                    break;
                case Direction.Right:
                    moveDirectionVector = objectApplyRotation.value.transform.right;
                    saveFoundParameter.value = 1;
                    break;
                case Direction.Back:
                    moveDirectionVector = -objectApplyRotation.value.transform.forward;
                    saveFoundParameter.value = -1;
                    break;
                case Direction.Forward:
                    moveDirectionVector = objectApplyRotation.value.transform.forward;
                    saveFoundParameter.value = 1;
                    break;
            }

            return agent.transform.position + moveDirectionVector * stoppingDistance.value;
        }
        
        
        
        
    }
    
}

