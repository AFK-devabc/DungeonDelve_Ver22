using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Animation Clip")]
    [Category("Animator")]
    [Description("Set Clip mới vào Animator Override Controller.")]
    public class MecanimSetClipAOC : ActionTask
    {
        public BBParameter<AnimatorOverrideController> AnimatorOC;
        public BBParameter<AnimationClip> clipSet;

        public BBParameter<string> slotSet;

        //protected override string info => $"Set {clipSet.value.name} to slot {parameter.value}";
        protected override string info
        {
            get { return string.Format("Mec.Set {0} Clip to Slot [{1}]",clipSet.value ? clipSet.value.name : "NULL", slotSet.value); }
        }
        
        protected override void OnExecute()
        {
            if (string.IsNullOrEmpty(slotSet.value))
            {
                EndAction();
                return;
            }
            
            AnimatorOC.value[slotSet.value] = clipSet.value;
            EndAction();
        }
        
    }
}

