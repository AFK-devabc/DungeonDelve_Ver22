using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Position")]
    [Category("GameObject")]
    [Description("Đặt vị trí đối tượng.")]
    public class SetObjectPosition : ActionTask
    {
        [RequiredField, Tooltip("Đối tượng cần thao tác")]
        public BBParameter<GameObject> setObject;

        [Tooltip("Nếu object là Null thì sẽ lấy vị trí của toPosition")]
        public BBParameter<GameObject> toObject;
        public BBParameter<Vector3> toPosition;

        public BBParameter<Vector3> offset;
        
        protected override string info => "Set Position";
        
        protected override void OnExecute()
        {
            if(setObject.value == null) 
                EndAction(false);
            
            setObject.value.transform.position = toObject.value == null ?
                                                                toPosition.value + offset.value :
                                                                toObject.value.transform.position + offset.value;
            EndAction(true);
        }
    }
    
}

