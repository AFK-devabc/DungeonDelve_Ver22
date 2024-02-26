using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Capsule Enabled")]
    [Category("Collider")]
    [Description("Set trạng thái của Capsule Collider.")]
    public class SetCapsuleEnabled : ActionTask
    {
        public enum SetEnableMode
        {
            Disable = 0,
            Enable = 1,
            Toggle = 2
        }

        public BBParameter<CapsuleCollider> collider;
        public SetEnableMode setTo = SetEnableMode.Toggle;
        
        protected override string info {
            get { return string.Format("{0} {1} {2}", setTo, "Capsule", agentInfo); }
        }
        
        protected override void OnExecute() { 

            bool value;

            if ( setTo == SetEnableMode.Toggle ) {

                value = !collider.value.enabled;

            } else {

                value = (int)setTo == 1;
            }

            collider.value.enabled = value;
            EndAction();
        } 
        
    }
}

