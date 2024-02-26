using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Name("Wait AnimationClip")]
    [Description("Chờ khoảng thời gian bằng thời gian animation hiện tại đang phát trong animator. Nếu có truyền clip vào thì sẽ chờ thời gian của clip")]
    [Category("Animator")]
    public class MecanimWaitAnimationClip : ActionTask<Animator>
    {
        public BBParameter<AnimationClip> clipWait;
        public BBParameter<int> layerIndex = 0;
        
        private float animationLength;
        
        protected override string info => "Wait AnimationClip";

        protected override void OnExecute()
        {
            if(agent == null)
                EndAction(true);
            
            animationLength = clipWait != null && clipWait.value != null
                ? clipWait.value.length
                : agent.GetCurrentAnimatorStateInfo(layerIndex.value).length;
            
            Debug.Log("Wait: " + animationLength);
        }
        protected override void OnUpdate()
        {
            if(elapsedTime >= animationLength)
                EndAction(true);
        }
        
    }
}
