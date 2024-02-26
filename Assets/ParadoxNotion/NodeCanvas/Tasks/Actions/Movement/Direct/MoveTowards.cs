using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Direct")]
    [Description("Di chuyển Object tới mục tiêu nhưng không tìm đường")]
    public class MoveTowards : ActionTask<Transform>
    {
        [Tooltip("Nếu mục tiêu là Null thì sẽ lấy vị trí của targetPosition")]
        public BBParameter<GameObject> target;
        public BBParameter<Vector3> targetPosition;
        
        public BBParameter<float> speed = 2;
        public BBParameter<float> stopDistance = 0.1f;
        public bool updateFrameByFrame;
        
        [Space, Tooltip("Biến để lưu giá trị xoay được vào, nếu target bên trái sẽ trả về -1 ngược lại trả về 1"), RequiredField]
        public BBParameter<float> saveFoundParameter;

        private bool isTarget;
        
        private float CurrentDistanceToTarget => (agent.position - target.value.transform.position).magnitude;
        private float CurrentDistanceToVector => (agent.position - targetPosition.value).magnitude;

        
        protected override void OnExecute()
        {
            isTarget = target.value != null;
        }

        protected override void OnUpdate() 
        {
            if (isTarget)
            {
                if (CurrentDistanceToTarget <= stopDistance.value )
                {
                    saveFoundParameter.value = 0;
                    if(!updateFrameByFrame)
                        EndAction();
                    return;
                }
                
                saveFoundParameter.value = 1;
                agent.position = Vector3.MoveTowards(agent.position, target.value.transform.position, speed.value * Time.deltaTime);
            }
            else
            {
                if (CurrentDistanceToVector <= stopDistance.value )
                {
                    saveFoundParameter.value = 0;
                    if(!updateFrameByFrame)
                        EndAction();
                    return;
                }
                agent.position = Vector3.MoveTowards(agent.position, targetPosition.value, speed.value * Time.deltaTime);
            }
        }


    }
}