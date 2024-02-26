using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonDelve.Project;
using NaughtyAttributes;
using Newtonsoft.Json;

namespace DungeonDelve.Project
{
    [Serializable]
    public class Task
    {
        public Task() { }
        public Task(bool _isCompleted, bool _isLocked, bool _isReceived)
        {
            isCompleted = _isCompleted;
            isLocked = _isLocked;
            isReceived = _isReceived;
        }
        
        [SerializeField, JsonProperty] private bool isCompleted;
        [SerializeField, JsonProperty] private bool isLocked;
        [SerializeField, JsonProperty] private bool isReceived;
        
        public bool IsCompleted() => isCompleted;
        public bool IsLocked() => isLocked;
        public bool IsReceived() => isReceived;
        public void SetCompleted(bool _value) => isCompleted = _value;
        public bool SetTaskLocked(bool _value) =>  isLocked = _value;
        public bool SetReceived(bool _value) =>  isReceived = _value;
    } 
    
    [Serializable]
    public class TaskRequirement
    {
        [field: SerializeField] public bool IsUseRequirement { get; private set; } = true;
        [field: SerializeField, Tooltip("Vật phẩm yêu cầu")] public ItemNameCode code { get; private set; } 
        
        [field: SerializeField, Tooltip("Số lượng vật phẩm cần")] public int value { get; private set; } 

        //public ItemNameCode GetNameCode() => code;
        public void SetNameCode(ItemNameCode _value) => code = _value;
        public void SetUseRequirement(bool _value) => IsUseRequirement = _value;
        public void SetValue(int _value) => value = _value;

        // public int GetValue() => value;
        public bool CheckCompleteTask(int _hasValue) => value <= _hasValue;
    }
}


[Serializable]
[CreateAssetMenu(menuName = "Create Quest", fileName = "Quest_")]
public class QuestSetup : ScriptableObject
{
    [SerializeField, ReadOnly] private Task task;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private TaskRequirement requirement;
    [SerializeField] private List<ItemReward> rewards;
    
    #region Getter
    public Task GetTask() => task;
    public string GetTitle() => title;
    public string GetDescription() => description;
    public TaskRequirement GetRequirement() => requirement;
    public List<ItemReward> GetRewards() => rewards;
    #endregion
    
    #region Setter
    public void SetTask(Task _value) => task = _value;
    public void SetTitle(string _value) => title = _value;
    public void SetDescription(string _value) => description = _value;
    public void SetRequirement(TaskRequirement _value) => requirement = _value;
    public void SetReward(List<ItemReward> _value) => rewards = _value;
    #endregion


}
