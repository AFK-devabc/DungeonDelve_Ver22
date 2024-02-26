using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Active")]
    [Category("GameObject")]
    [Description("Set the gameobject active state.")]
    public class SetObjectActive : ActionTask
    {
        [RequiredField, Tooltip("Đối tượng cần thao tác")]
        public BBParameter<GameObject> setObject;
        
        public enum SetActiveMode
        {
            Deactivate = 0,
            Activate = 1,
            Toggle = 2
        }

        public SetActiveMode setTo = SetActiveMode.Toggle;

        protected override string info {
            get { return string.Format("{0} {1}", setTo, setObject.value != null ? setObject.value.name : "NULL"); }
        }

        protected override void OnExecute() {

            bool value;
            if ( setTo == SetActiveMode.Toggle ) 
            {
                value = !setObject.value.activeSelf;
            } 
            else
            {
                value = (int)setTo == 1;
            }
            setObject.value.SetActive(value);
            EndAction();
        }
    }
}