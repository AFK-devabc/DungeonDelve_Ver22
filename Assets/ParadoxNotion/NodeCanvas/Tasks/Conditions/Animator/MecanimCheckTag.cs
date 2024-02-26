using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
    [Name("Check Tag Animation")] 
    [Description("Kiểm tra animator 1 lần duy nhất và trả về giá trị True - False nếu animation hiện tại có Tag = giá trị được chỉ định không ?")] 
    [Category("Animator")]
    public class MecanimCheckTag : ConditionTask<Animator>
    {
        [RequiredField]
        public BBParameter<string> tagCheck;
        public BBParameter<int> layerCheck = 0;

        protected override string info => $"CurrentTag == {tagCheck}";
        protected override bool OnCheck() => agent.GetCurrentAnimatorStateInfo(layerCheck.value).IsTag(tagCheck.value);
        
    }

}
