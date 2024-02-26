using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    public class Wait : ActionTask
    {
        [Tooltip("Có muốn Random thời gian chờ")]
        public BBParameter<bool> randomValue;
        public BBParameter<float> minValue, maxValue;
        
        [Space]
        public BBParameter<float> waitTime = 1f;
        public CompactStatus finishStatus = CompactStatus.Success;

        protected override string info => $"Wait {waitTime} sec.";


        protected override void OnExecute()
        {
            if (randomValue.value)
            {
                waitTime.value = Random.Range(minValue.value, maxValue.value);
            }
        }

        protected override void OnUpdate() 
        {
            if ( elapsedTime >= waitTime.value )
            {
                EndAction(finishStatus == CompactStatus.Success);
            }
        }
    }
}