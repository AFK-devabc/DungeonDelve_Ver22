using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Direct")]
    [Description("Nếu tùy chọn updateFrameByFrame = TRUE sẽ xoay đối tượng về phía mục tiêu trên mỗi khung hình, ngược lại sẽ cập nhật đến khi góc của " +
                 "(this) và (taget) <= angleDifference")]
    public class RotateTowards : ActionTask<Transform>
    {
        [Tooltip("Nếu mục tiêu là Null thì sẽ lấy vị trí của targetPosition")]
        public BBParameter<GameObject> target;
        public BBParameter<Vector3> targetPosition;
        
        public BBParameter<float> speed = 2;
        
        public bool updateFrameByFrame;
        
        [SliderField(0, 180)]
        public BBParameter<float> angleDifference = 5;
        
        [Space, Tooltip("Biến để lưu giá trị xoay được vào, nếu target bên trái sẽ trả về -1 ngược lại trả về 1"), RequiredField]
        public BBParameter<float> saveFoundParameter;

        private Quaternion currentRotate;
        private float deltaAngle;
        private bool isTarget;
        
        
        protected override void OnExecute()
        {
            isTarget = target.value != null;
            currentRotate = agent.rotation;
        }

        protected override void OnUpdate() 
        {
            var dir = Quaternion.LookRotation( isTarget ? target.value.transform.position - agent.position : targetPosition.value - agent.position);
            agent.rotation = Quaternion.RotateTowards(agent.rotation, Quaternion.Euler(0, dir.eulerAngles.y, 0), speed.value * Time.deltaTime);

            deltaAngle = Mathf.DeltaAngle(currentRotate.eulerAngles.y, agent.eulerAngles.y);
            currentRotate = agent.rotation;
        
            if (deltaAngle == 0 || Mathf.Abs(deltaAngle) <= angleDifference.value)
            { 
                saveFoundParameter.value = 0;
                if (!updateFrameByFrame)
                {
                    EndAction();
                }
            }
            else
            {
                saveFoundParameter.value = Mathf.Sign(deltaAngle);
            }
        }
    }
}