using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Name("Set Speed")]
    [Description("Set tốc độ phát của animator")]
    [Category("Animator")]
    public class MecanimSetSpeed : ActionTask<Animator>
    {
        [SliderField(0, 3f)]
        public BBParameter<float> speedSet;

        protected override string info => "Mec.Speed " + speedSet.value;

        protected override void OnExecute()
        {
            if(agent == null) EndAction(true);

            agent.speed = speedSet.value;
            EndAction(true);
        }
    }
    
}

