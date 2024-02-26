using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DungeonDelve.Project;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Quest : MonoBehaviour, IGUI
{
    [SerializeField] private GameObject questPanel;
    [SerializeField, BoxGroup("INFORMATION")] private GameObject requirementTaskPanel;
    [SerializeField, BoxGroup("INFORMATION")] private Animator noticeQuestPanel;
    [SerializeField, BoxGroup("INFORMATION")] private TextMeshProUGUI noticeQuestText;
    [SerializeField, BoxGroup("INFORMATION")] private Animator completedPanel;
    [SerializeField, BoxGroup("INFORMATION")] private TextMeshProUGUI titleQuest;
    [SerializeField, BoxGroup("INFORMATION")] private TextMeshProUGUI descriptionQuest;
    [SerializeField, BoxGroup("INFORMATION")] private TextMeshProUGUI questProgressText;
    [SerializeField, BoxGroup("INFORMATION")] private TextMeshProUGUI taskNoticeText;
    [SerializeField, BoxGroup("BUTTON")] private Button cancelBtt;
    [SerializeField, BoxGroup("BUTTON")] private Button acceptBtt;
    [SerializeField, BoxGroup("BUTTON")] private Button reportBtt;
    [SerializeField, BoxGroup("BUTTON")] private Button navigateBtt ;
    [Space]
    [SerializeField] private QuestBox barPrefab;
    [SerializeField] private Transform contentQuest;
    [Space]
    [SerializeField] private UI_Item itemPrefab;
    [SerializeField] private Transform contentItem;
    [SerializeField] private UI_Item itemRequired;

    private static List<QuestSetup> _quests => QuestManager.QuestLists;
    private static ObjectPooler<QuestBox> _poolQuestBoxText;
    private static ObjectPooler<UI_Item> _poolUIItem;
    private SO_GameItemData _itemData;
    private UserData _userData;
    private QuestBox _currentQuestBox;
    private Coroutine _completedPanelCoroutine;
    
    private bool _canPanelOpen;
    private bool _isAccept;
    private bool _isReportQuest;


    private void OnEnable()
    {
        Init();
        RegisterEvent();
    }
    private void Start()
    {
        _poolUIItem = new ObjectPooler<UI_Item>(itemPrefab, contentItem, 15);
        _poolQuestBoxText = new ObjectPooler<QuestBox>(barPrefab, contentQuest, 10);
        _poolQuestBoxText.List.ForEach(box => box.OnQuestSelectedEvent += OnSelectedQuest);
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }
    private void OnDestroy()
    {
        _poolQuestBoxText.List.ForEach(box => box.OnQuestSelectedEvent -= OnSelectedQuest);
    }
    
    private void Init()
    {
        titleQuest.text = "";
        descriptionQuest.text = "";
        _canPanelOpen = true;
        questPanel.gameObject.SetActive(false);
        itemRequired.gameObject.SetActive(false);

        InitButtonState();
    }
    private void InitButtonState()
    {
        cancelBtt.interactable = false;
        acceptBtt.interactable = false;
        cancelBtt.gameObject.SetActive(true);
        acceptBtt.gameObject.SetActive(true);
        reportBtt.gameObject.SetActive(false);
        navigateBtt.gameObject.SetActive(false);
    }   
    private void RegisterEvent()
    {
        GUI_Manager.Add(this);
        QuestManager.Instance.interactiveUI.OnPanelOpenEvent += OnOpenPanelEvent;
        QuestManager.Instance.interactiveUI.OnPanelCloseEvent += OnClosePanelEvent;
        acceptBtt.onClick.AddListener(OnClickNoticeAcceptQuest);
        cancelBtt.onClick.AddListener(OnClickNoticeCancelQuest);
        reportBtt.onClick.AddListener(OnClickNoticeReportQuest);
        navigateBtt.onClick.AddListener(NavigateToQuestSSS);
    }
    private void UnRegisterEvent()
    {
        GUI_Manager.Remove(this);
        acceptBtt.onClick.RemoveListener(OnClickNoticeAcceptQuest);
        cancelBtt.onClick.RemoveListener(OnClickNoticeCancelQuest);
        reportBtt.onClick.RemoveListener(OnClickNoticeReportQuest);
        navigateBtt.onClick.RemoveListener(NavigateToQuestSSS);

        if (!QuestManager.Instance) return;
        QuestManager.Instance.interactiveUI.OnPanelOpenEvent -= OnOpenPanelEvent;
        QuestManager.Instance.interactiveUI.OnPanelCloseEvent -= OnClosePanelEvent;
    }
    
    
    public void GetRef(GameManager _gameManager)
    {
        _itemData = _gameManager.GameItemData;
        _userData = _gameManager.UserData;
    }
    public void UpdateData()
    {
        ShowQuest();
    }
    
    
    public void OnOpenPanelEvent()
    {
        if (!_canPanelOpen) return;
        _canPanelOpen = false;

        InitButtonState();
        questPanel.SetActive(true);
        noticeQuestPanel.Play("Panel_Disable");
        ShowQuest();
        MenuController.Instance.HandleMenuOpen();
    }
    private void OnClosePanelEvent()
    {
        if (_canPanelOpen) return;
        _canPanelOpen = true;
        
        questPanel.SetActive(false);
        GUI_Inputs.InputAction.UI.OpenBag.Enable();
        MenuController.Instance.HandleMenuClose();
    }
    public void OnClickClosePanelButton() => QuestManager.Instance.interactiveUI.ClosePanel(default);
    
    private void ShowQuest()
    {
        titleQuest.text = "???";
        descriptionQuest.text = "???";
        taskNoticeText.text = "";
        itemRequired.gameObject.SetActive(false);
        
        _poolUIItem.List.ForEach(item => item.Release());
        _poolQuestBoxText.List.ForEach(box => box.Release());

        for (var i = 0; i < _quests.Count; i++)
        {
            _poolQuestBoxText.Get();
        }
        var _count = 0;
        var _boxQuestActives = _poolQuestBoxText.List.Where(x => x.gameObject.activeSelf);
        foreach (var _questBox in _boxQuestActives)
        {
            _questBox.SetQuestBox(_quests[_count]);
            CheckQuestReport(_questBox); 
            _count++;
        }
        SetQuestProgressText();
    }
    public void OnSelectedQuest(QuestBox _questBox)
    {
        _currentQuestBox = _questBox;
        var _questSetup = _questBox.questSetup;
        titleQuest.text = _questSetup.GetTitle();
        descriptionQuest.text = _questSetup.GetDescription();
        
        SetNoticeText(_questSetup.GetTask());
        SpawnItemReward(_questSetup);
        SetItemReuired(_questSetup);
        SetButton(_questBox);
        SetAnimSelectedQuestBox(_questBox);
        CheckQuestReport(_questBox);
    }
    private void SetItemReuired(QuestSetup _questSetup)
    {
        var _taskRequirement = _questSetup.GetRequirement();
        if (!_taskRequirement.IsUseRequirement)
        {
            requirementTaskPanel.SetActive(false);
            return;
        }
        
        requirementTaskPanel.SetActive(true);
        itemRequired.gameObject.SetActive(true);
        var _itemCustom = _itemData.GetItemCustom(_taskRequirement.code);
        var _hasItem = _userData.HasItemValue(_taskRequirement.code);
        
        itemRequired.SetItem(_itemCustom, _taskRequirement.value);
        itemRequired.SetValueText($"{ _taskRequirement.value}/{_hasItem}");
    }
    private void SpawnItemReward(QuestSetup _questSetup)
    {
        var _rewards = _questSetup.GetRewards();
        
        _poolUIItem.List.ForEach(item => item.Release());
        for (var i = 0; i < _rewards.Count; i++)
        {
            _poolUIItem.Get();
        }

        var _count = 0;
        var _items = _poolUIItem.List.Where(item => item.gameObject.activeSelf).ToList();
        foreach (var uiItem in _items)
        {
            var _itemCustom = _itemData.GetItemCustom(_rewards[_count].GetNameCode());
            var _itemValue = _rewards[_count].GetValue();
            
            uiItem.SetItem(_itemCustom, _itemValue);
            uiItem.SetValueText($"{_itemValue}");
            _count++;
        }
    }
    private void SetAnimSelectedQuestBox(QuestBox _questSelected)
    {
        var _currentList = _poolQuestBoxText.List.Where(box => box.gameObject.activeSelf);
        foreach (var _quest in _currentList)
        {
            var _animator = _quest.animator;
            if (_animator.IsTag("Selected") && _quest != _currentQuestBox)
                _quest.animator.Play(SwitchPanelControl.NameHashID_NonTrigger);
            else if (!_animator.IsTag("Selected") && _animator == _questSelected.animator)
                _quest.animator.Play(SwitchPanelControl.NameHashID_Selected);
        }
    }
    public static void IsTriggerQuestBox(QuestBox _questBox, bool _hasTrigger)
    {
        var _questList = _poolQuestBoxText.List.Where(box => box.gameObject.activeSelf).ToList()
            .Where(box => !box.animator.IsTag("Selected", 0) && !box.animator.IsTag("SelectedQuest", 0));
        
        int _hashID;
        if (_hasTrigger)
        {
            foreach (var _quest in _questList)
            {
                _hashID = _quest == _questBox ? SwitchPanelControl.NameHashID_Trigger : SwitchPanelControl.NameHashID_NonTrigger;
                _quest.animator.Play(_hashID);
            }   
        }
        else
        {
            foreach (var _quest in _questList)
            {
                _hashID = SwitchPanelControl.NameHashID_NonTrigger;
                _quest.animator.Play(_hashID);
            }
        }
    }
    
    
    // OnClick Button
    private void OnClickNoticeAcceptQuest()
    {
        _isAccept = true;
        OpenNoticeQuestPanel("Would you like to accept this quest?\nContinue?");
    }   
    private void OnClickNoticeCancelQuest()
    {
        _isAccept = false;
        OpenNoticeQuestPanel("Would you like to abandon this task? If cancelled, this task will no longer be selectable for the remaining time of today." +
                             " And you will also forfeit one task claim.\nContinue?");
    }
    private void OnClickNoticeReportQuest()
    {
        _isReportQuest = true;
        OpenNoticeQuestPanel("Would you like to report this mission? Upon completion, you will lose the items required for this mission.\nContinue?");
    }
    public void OnClickConfirmButton()
    {
        if (_isReportQuest)
        {
            _isReportQuest = false;
            var _taskRequirement = _currentQuestBox.questSetup.GetRequirement();
            if (_taskRequirement.IsUseRequirement)
            {
                _userData.IncreaseItemValue(_taskRequirement.code, -_taskRequirement.value);
                QuestManager.Instance.OnCompletedQuest(_currentQuestBox.questSetup);
            }
            completedPanel.Play("QuestCompleted_IN");
        }
        else if (_isAccept)
        {
            _isAccept = false;
            _currentQuestBox.SetReceivedQuestBox(true);
            QuestManager.OnStartedQuest(_currentQuestBox.questSetup);
        }
        else
        {
            _currentQuestBox.SetReceivedQuestBox(false);
            _currentQuestBox.LockTask();
            QuestManager.OnCancelQuest(_currentQuestBox.questSetup);
        }

        OnClickCancelButton();
        SetQuestProgressText();
        SetButton(_currentQuestBox);
        CheckQuestReport(_currentQuestBox);
        SetNoticeText(_currentQuestBox.questSetup.GetTask()); 
    }
    public void OnClickCancelButton()
    {
        noticeQuestPanel.Play("Panel_OUT");
        _isReportQuest = false;
    }
    
    private void OpenNoticeQuestPanel(string _noticeText)
    {
        noticeQuestPanel.Play("Panel_IN");
        noticeQuestText.text = _noticeText;
    }
    private void SetQuestProgressText() => questProgressText.text = $"In Progress: {QuestManager.currentQuest}/{QuestManager.maxQuest}";
    private void SetButton(QuestBox _questBox)
    {
        acceptBtt.gameObject.SetActive(!_questBox.IsReceived);
        navigateBtt.gameObject.SetActive(!_questBox.questSetup.GetRequirement().IsUseRequirement && !acceptBtt.gameObject.activeSelf && !_questBox.IsCompleted);
        reportBtt.gameObject.SetActive(_questBox.IsReceived && !navigateBtt.gameObject.activeSelf);
        
        var _checkCommon = !_questBox.IsLocked && !_questBox.IsCompleted;
        acceptBtt.interactable = _checkCommon && !_questBox.IsReceived && QuestManager.currentQuest < QuestManager.maxQuest;
        cancelBtt.interactable = _checkCommon && _questBox.IsReceived;     
    }
    private void CheckQuestReport(QuestBox _questBox)
    {
        var _taskRequirement = _questBox.questSetup.GetRequirement();
        if (!_taskRequirement.IsUseRequirement) 
            return;
        var _hasCurrentItemValue = _userData.HasItemValue(_taskRequirement.code);
        var _checkComplete = _taskRequirement.CheckCompleteTask(_hasCurrentItemValue);
        reportBtt.interactable = _checkComplete && !_questBox.questSetup.GetTask().IsCompleted();
        _questBox.SetReportQuest(reportBtt.interactable);
    }
    private void SetNoticeText(Task _task) => taskNoticeText.text = _task.IsCompleted() ? "You have completed this task." : _task.IsLocked() ? "Can't handle this task today." : "";
    private void NavigateToQuestSSS()
    {
        OnClickClosePanelButton();
        QuestManager.Instance.interactiveUI.OnExitPlayer();
        QuestSSSManager.Instance.HandleEnableQuest(_currentQuestBox.questSetup);
    }
    
}
