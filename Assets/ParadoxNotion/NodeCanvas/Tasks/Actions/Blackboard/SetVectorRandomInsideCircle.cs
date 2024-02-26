using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ Blackboard")]
    [Description("Đặt biến Vector3 trên board bằng (vị trí mục tiêu + vị trí ngẫu nhiên) trong vòng tròn có bán kính được chỉ định bằng Random.inside. Trục Y sẽ giữ nguyên theo vị trí bắt đầu Random ban đầu")]
    public class SetVectorRandomInsideCircle : ActionTask<Transform>
    {
        [Tooltip("Vị trí mục tiêu Random, nếu giá trị là Null sẽ lấy object hiện tại (this).")] 
        public BBParameter<Transform> targetPosition;
        
        [Tooltip("Bán kính vòng tròn")]
        public BBParameter<float> radius;
        
        public BBParameter<Vector3> offset;
        
        public BBParameter<Vector3> saveFoundParameter;
        
        private Vector2 _randPoint;
        private Vector3 _newPos;

        protected override string info => $"Random inside circle = {radius.value}";
        
        protected override void OnExecute()
        {
            _randPoint = Random.insideUnitCircle * radius.value;
            _newPos = targetPosition.value != null ? targetPosition.value.position + new Vector3(_randPoint.x + offset.value.x, 0f +  + offset.value.y, _randPoint.y + offset.value.z)
                                                   : agent.transform.position + new Vector3(_randPoint.x + offset.value.x, 0f +  + offset.value.y, _randPoint.y + offset.value.z);
            
            saveFoundParameter.value = _newPos;
            EndAction(true);
        }


        public override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            if (targetPosition.value != null)
            {
                Gizmos.DrawWireSphere(targetPosition.value.position, radius.value);
            }
            else if (agent != null)
            {
                Gizmos.DrawWireSphere(agent.transform.position, radius.value);
            }
        }
    }
}

