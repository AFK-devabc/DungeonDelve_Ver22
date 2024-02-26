using System.Collections;
using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

public class QuestSSSManager : Singleton<QuestSSSManager>
{
   [SerializeField] private GameObject evironment;
   [SerializeField] private Animator completedQuestPanel;
   
   [SerializeField,BoxGroup("PLAYER CONFIG")] private Vector3 startPosition;
   [SerializeField,BoxGroup("PLAYER CONFIG")] private Vector3 startRotation;

   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Transform doorTurn1;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Vector3 doorTurn1OpenPosition;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private StudioEventEmitter doorTurn1Audio;
   private Vector3 doorTurn1ClosePosition;
   [Space]
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Transform doorTurn2;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Vector3 doorTurn2OpenPosition;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private StudioEventEmitter doorTurn2Audio;
   private Vector3 doorTurn2ClosePosition;
   [Space]
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Transform doorTurn3;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private Vector3 doorTurn3OpenPosition;
   [SerializeField, BoxGroup("ENVIRONMENT CONFIG")] private StudioEventEmitter doorTurn3Audio;
   private Vector3 doorTurn3ClosePosition;
   
   [SerializeField, BoxGroup("INTERACTIVE")] private InteractiveUI interactiveTurn1;
   [SerializeField, BoxGroup("INTERACTIVE")] private InteractiveUI interactiveTurn2;
   [SerializeField, BoxGroup("INTERACTIVE")] private InteractiveUI interactiveTurn3;

   [SerializeField, BoxGroup("ENEMY SPAWNER")] private EnemySpawner pointSpawnerTurn1;
   [SerializeField, BoxGroup("ENEMY SPAWNER")] private EnemySpawner pointSpawnerTurn2;
   [SerializeField, BoxGroup("ENEMY SPAWNER")] private EnemySpawner pointSpawnerTurn3;

   [SerializeField, BoxGroup("CORE CONFIG")] private Tower towerTurn2;
   [SerializeField, BoxGroup("CORE CONFIG")] private CountdownTimer countdownTimerTurn2;
   [SerializeField, BoxGroup("CORE CONFIG")] private CooldownTime cooldownJoinAgainQuest;
   [SerializeField, BoxGroup("CORE CONFIG")] private CooldownTime cooldownExitQuest;
   [SerializeField, BoxGroup("CORE CONFIG")] private PlayableDirector timelineTurn3;
   [SerializeField, BoxGroup("CORE CONFIG")] private EnemyController bossTurn3;
   
   [BoxGroup("VOLUME CHANGE"), SerializeField] private DungeonAmbienceVolumeChangeTrigger dungeonAmbienceVolumeChange;
   [BoxGroup("VOLUME CHANGE"), SerializeField] private BackgroundAudio reaperBattleAudio;
   
   //Var
   private Tween _doorTween;
   private readonly float _doorDurationTween = 4f;
   private PlayerController _player;
   private Coroutine _handleCoroutine;
   private Coroutine _waitAgainCoroutine;
   private QuestSetup _questSetup;
   
   private void Start()
   {
      _player = GameManager.Instance.Player;
      
      evironment.SetActive(false);
      doorTurn1ClosePosition = doorTurn1.localPosition;
      doorTurn2ClosePosition = doorTurn2.localPosition;
      doorTurn3ClosePosition = doorTurn3.localPosition;
      
      RegisterEvent();
   }
   private void OnDestroy()
   {
      UnRegisterEvent();
   }
   private void RegisterEvent()
   {
      interactiveTurn1.OnPanelOpenEvent += HandleActiveTurn1;
      interactiveTurn2.OnPanelOpenEvent += HandleActiveTurn2;
      
      pointSpawnerTurn1.AreaCleanedEvent.AddListener(HandleClearEnemyTurn1);
      pointSpawnerTurn2.PoolEnemies.ForEach(x => x.List.ForEach(e => e.OnDieEvent.AddListener(HandleEnemyDie)));
      
      _player.OnDieEvent += HandlePlayerDie;
      bossTurn3.OnDieEvent.AddListener(HandleBossDie);
      
      towerTurn2.OnDieEvent.AddListener(HandleTowerDefenseFailed);
   }
   private void UnRegisterEvent()
   {
      interactiveTurn1.OnPanelOpenEvent -= HandleActiveTurn1;
      interactiveTurn2.OnPanelOpenEvent -= HandleActiveTurn2;
      
      pointSpawnerTurn1.AreaCleanedEvent.RemoveListener(HandleClearEnemyTurn1);
      pointSpawnerTurn2.PoolEnemies.ForEach(x => x.List.ForEach(e => e.OnDieEvent.RemoveListener(HandleEnemyDie)));

      _player.OnDieEvent -= HandlePlayerDie;
      bossTurn3.OnDieEvent.RemoveListener(HandleBossDie);
      towerTurn2.OnDieEvent.RemoveListener(HandleTowerDefenseFailed);
   }
   
   private void HandleActiveTurn1()
   {
      NoticeManager.Instance.OpenYellowTextNoticeT4("Eliminate all enemies.");
      pointSpawnerTurn1.StartedSpawn();
      interactiveTurn1.OnExitPlayer();
      interactiveTurn1.gameObject.SetActive(false);
   }
   private void HandleClearEnemyTurn1()
   {
      doorTurn1Audio.Play();
      doorTurn1.gameObject.SetActive(true);
      doorTurn1.localPosition = doorTurn1ClosePosition;
      _doorTween?.Kill();
      _doorTween = doorTurn1.transform.DOLocalMove(doorTurn1OpenPosition, _doorDurationTween).OnComplete((() =>
      {
         doorTurn1.gameObject.SetActive(false);
      }));
   }
   
   private void HandleActiveTurn2()
   {
      NoticeManager.Instance.OpenYellowTextNoticeT4("Protect the tower for 60 seconds.");
      pointSpawnerTurn2.StartedSpawn();
      interactiveTurn2.OnExitPlayer();
      interactiveTurn2.gameObject.SetActive(false);
      
      countdownTimerTurn2.gameObject.SetActive(true);
      countdownTimerTurn2.StartCountdown();
      
      towerTurn2.OpenHUD();
      towerTurn2.Init();
      pointSpawnerTurn2.SetTargetAtEnemy(towerTurn2.gameObject);
   }
   public void HandleClearEnemyTurn2()
   {
      towerTurn2.CloseHUD();
      pointSpawnerTurn2.ReleaseAllEnemy();
      doorTurn2Audio.Play();
      countdownTimerTurn2.StopAllCoroutines();
      countdownTimerTurn2.gameObject.SetActive(false);
      
      doorTurn2.gameObject.SetActive(true);
      doorTurn2.localPosition = doorTurn2ClosePosition;
      _doorTween?.Kill();
      _doorTween = doorTurn2.transform.DOLocalMove(doorTurn2OpenPosition, _doorDurationTween).OnComplete((() =>
      {
         doorTurn2.gameObject.SetActive(false);
         EnemyTracker.Clear();
      }));
   }
   
   public void HandleActiveTurn3()
   {
      timelineTurn3.Play();
      interactiveTurn3.OnExitPlayer();
      interactiveTurn3.gameObject.SetActive(false);
      
      doorTurn3.gameObject.SetActive(true);
      doorTurn3.localPosition = doorTurn3OpenPosition;
      _doorTween?.Kill();
      _doorTween = doorTurn3.transform.DOLocalMove(doorTurn3ClosePosition, _doorDurationTween);
   }

   
   public void HandleEnableQuest(QuestSetup _questSetup)
   {
      EnableCombatAudio();
      
      this._questSetup = _questSetup;
      ReleaseAllEnemy();
      LoadingPanel.Instance.Active(.7f);
      pointSpawnerTurn1.ReleaseAllEnemy();
      evironment.SetActive(true);
      
      doorTurn1.gameObject.SetActive(true);
      doorTurn1.localPosition = doorTurn1ClosePosition;
      doorTurn2.gameObject.SetActive(true);
      doorTurn2.localPosition = doorTurn2ClosePosition;
      doorTurn3.gameObject.SetActive(true);
      doorTurn3.localPosition = doorTurn3OpenPosition;
      
      interactiveTurn1.gameObject.SetActive(true);
      interactiveTurn2.gameObject.SetActive(true);
      interactiveTurn3.gameObject.SetActive(true);
      
      SetPlayerConfig(startPosition, -90f, .5f);
   }
   private void SetPlayerConfig(Vector3 _targetPos, float _camXAxisVal, float _camYAxisVal)
   {
      _player.characterController.enabled = false;
      _player.transform.position = _targetPos;
      _player.model.transform.rotation = Quaternion.Euler(startRotation);
      _player.characterController.enabled = true;

      var _cam = _player.cameraFOV.cinemachineFreeLook;
      _cam.m_XAxis.Value = _camXAxisVal;
      _cam.m_YAxis.Value = _camYAxisVal;
   }
   public void EnableControl()
   {
      _player.input.PlayerInput.Player.Enable();
      bossTurn3.SetChaseSensor(true);
      pointSpawnerTurn3.StartedSpawn();
   }
   public void DisableControl()
   {
      _player.input.PlayerInput.Player.Disable();
   }
   private void HandleBossDie(EnemyController _enemy)
   {
      if (_handleCoroutine != null) StopCoroutine(_handleCoroutine);
      _handleCoroutine = StartCoroutine(HandleBossDieCoroutine());
   }
   private void HandleTowerDefenseFailed()
   {
      if (_handleCoroutine != null) StopCoroutine(_handleCoroutine);
      _handleCoroutine = StartCoroutine(HandleTowerDefenseFailedCoroutine());
   }
   private void HandleEnemyDie(EnemyController _enemy)
   {
      pointSpawnerTurn2.SetTargetAtEnemy(towerTurn2.gameObject);  
   }
   private void HandlePlayerDie()
   {
      if (_handleCoroutine != null) StopCoroutine(_handleCoroutine);
      _handleCoroutine = StartCoroutine(HandlePlayerDieCoroutine());
   }
   private IEnumerator HandlePlayerDieCoroutine()
   {
      bossTurn3.SetChaseSensor(false);
      countdownTimerTurn2.StopAllCoroutines();
      countdownTimerTurn2.gameObject.SetActive(false);
      yield return new WaitForSeconds(7f);
      bossTurn3.gameObject.SetActive(false);
      DisableCombatAudio();
      
      yield return new WaitForSeconds(.5f);
      if (_waitAgainCoroutine != null) StopCoroutine(_waitAgainCoroutine);
      _waitAgainCoroutine = StartCoroutine(WaitAgainQuestCoroutine());
   }
   private IEnumerator HandleBossDieCoroutine()
   {
      ReleaseAllEnemy();
      NoticeManager.Instance.OpenBlueTextNoticeT4("Successful Challenge");
      
      yield return new WaitForSeconds(3f);
      completedQuestPanel.Play("QuestCompleted_IN");
      QuestManager.Instance.OnCompletedQuest(_questSetup);
      
      yield return new WaitForSeconds(3f);
      var _exitTime = 7f;
      cooldownExitQuest.StartCooldownTime(_exitTime);
      
      yield return new WaitForSeconds(_exitTime);
      LoadingPanel.Instance.Active(.7f);
      
      yield return new WaitForSeconds(.5f);
      SetPlayerConfig(Vector3.zero, 0, 0.5f);
      DisableCombatAudio();
   }
   private IEnumerator HandleTowerDefenseFailedCoroutine()
   {
      countdownTimerTurn2.StopAllCoroutines();
      countdownTimerTurn2.SetFrameFill(false);
      NoticeManager.Instance.OpenBlueTextNoticeT4("Failure Challenge");
      pointSpawnerTurn1.PoolEnemies.ForEach(x=> x.List.ForEach(e => e.SetChaseSensor(false)));
      pointSpawnerTurn2.PoolEnemies.ForEach(x=> x.List.ForEach(e => e.SetChaseSensor(false)));
      pointSpawnerTurn3.PoolEnemies.ForEach(x=> x.List.ForEach(e => e.SetChaseSensor(false)));
      ReleaseAllEnemy();
      
      yield return new WaitForSeconds(2f);
      var _exitTime = 4f;
      cooldownExitQuest.StartCooldownTime(_exitTime);
      
      yield return new WaitForSeconds(_exitTime);
      LoadingPanel.Instance.Active(1f);
      
      yield return new WaitForSeconds(.5f);
      SetPlayerConfig(Vector3.zero, 0, .5f);
      DisableCombatAudio();
      towerTurn2.CloseHUD();
      yield return new WaitForSeconds(1.3f);
      if (_waitAgainCoroutine != null) StopCoroutine(_waitAgainCoroutine);
      _waitAgainCoroutine = StartCoroutine(WaitAgainQuestCoroutine());
      
   }
   private IEnumerator WaitAgainQuestCoroutine()
   {
      var _timeTemp = 10f;
      cooldownJoinAgainQuest.StartCooldownTime(_timeTemp);
      while (_timeTemp >= 0)
      {
         if (Input.GetKeyDown(KeyCode.Y))
         {
            HandleEnableQuest(_questSetup);
            cooldownJoinAgainQuest.SetFrameFill(false);
            break;
         }
         _timeTemp -= Time.deltaTime;
         yield return null;
      }
   }
   

   private void ReleaseAllEnemy()
   {
      pointSpawnerTurn1.ReleaseAllEnemy();
      pointSpawnerTurn2.ReleaseAllEnemy();
      pointSpawnerTurn3.ReleaseAllEnemy();
   }

   private void EnableCombatAudio()
   {
      reaperBattleAudio.Play();
      dungeonAmbienceVolumeChange.SetVolume(.1f);
   }
   private void DisableCombatAudio()
   {
      reaperBattleAudio.Stop();
   }
   
}
