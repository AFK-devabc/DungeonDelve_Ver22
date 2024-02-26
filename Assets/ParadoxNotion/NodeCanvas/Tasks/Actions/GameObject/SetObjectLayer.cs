using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Layer")]
    [Category("GameObject")]
    [Description("Set the gameobject layer.")]
    public class SetObjectLayer : ActionTask
    {
        [RequiredField, Tooltip("Đối tượng cần thao tác.")]
        public BBParameter<GameObject> setObject;
        
        [RequiredField, Tooltip("Layer mới cần set.")]
        public BBParameter<LayerMask> setLayer;
        
        protected override string info {
            get { return string.Format("Set Layer Object {0}.", setObject.value != null ? setObject.value.name : "NULL"); }
        }

        protected override void OnExecute() 
        {
            setObject.value.layer = setLayer.value;
            EndAction();
        }
        
    }
}

