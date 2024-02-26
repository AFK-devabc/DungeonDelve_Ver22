using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviourID behaviourID;
    [SerializeField] private bool isStartSpawn = true;
    [SerializeField] private bool isSaveEnemyData = true;
   
    [Space] 
    [SerializeField] private List<EnemyController> enemiesPrefab;
    
    [SerializeField, Tooltip("Khu vực sinh sản Enemy")]
    private Transform areaSpawn;
    
    [SerializeField, Tooltip("Số lượng tối đa khu vực này Spawn được")] 
    private int maxCountSpawn;
    
    [SerializeField, Tooltip("Bán kính tối đa mỗi Waypoint để random vị trí spawn")] 
    private int maxDistance;

    [SerializeField, Tooltip("Thời gian Reset lại toàn bộ số lượng Spawn trong khu vực (s)")]
    private float waitSpawn = 900f; // 15 Phút

    [Space][Tooltip("Khi tiêu diệt toàn bộ Enemy trong khu vực, sẽ gọi Event.")] 
    public UnityEvent AreaCleanedEvent;
    
    [SerializeField, Space] private bool drawAreaSpawnGizmos;


    public List<ObjectPooler<EnemyController>> PoolEnemies { get; private set; } = new();
    private YieldInstruction _yieldInstruction;
    private Coroutine _spawnCheckCoroutine;
    private Coroutine _checkAreaCoroutine;
    private readonly YieldInstruction _yieldCheck = new WaitForSeconds(1.2f);
    private DateTime _lastTime;
    private string PP_SaveCurrentEnemy => behaviourID.GetID + "Idx";
    private int _currentEnemy;
    
    
    private void Start()
    {
        _yieldInstruction = new WaitForSeconds(waitSpawn);
        foreach (var prefab in enemiesPrefab)
        {
            PoolEnemies.Add(new ObjectPooler<EnemyController>(prefab, transform, maxCountSpawn));
        }

        if (isStartSpawn) StartedSpawn();
    }

    public void StartedSpawn()
    {
        if(_spawnCheckCoroutine != null) 
            StopCoroutine(_spawnCheckCoroutine);
        _spawnCheckCoroutine = StartCoroutine(SpawnCoroutine());
    }
    private IEnumerator SpawnCoroutine()
    {
        _currentEnemy = PlayerPrefs.GetInt(PP_SaveCurrentEnemy, maxCountSpawn);
        if (_currentEnemy > 0)
        {
            for (var i = 0; i < _currentEnemy; i++)
            {
                Spawn();
                yield return new WaitForSeconds(Random.value);
            }
        }

        if (isSaveEnemyData)
        {
            _lastTime = DateTime.Parse(PlayerPrefs.GetString(behaviourID.GetID, DateTime.MinValue.ToString()));
            var _totalSeconds = DateTime.Now.Subtract(_lastTime).TotalSeconds;
            if (_totalSeconds < waitSpawn)
            {
                yield return new WaitForSeconds((float)(waitSpawn - _totalSeconds));
            }
        }
        
        while (true)
        {
            if(_currentEnemy < maxCountSpawn)
            {
                Spawn();
                _currentEnemy++;
            }
            
            if (isSaveEnemyData)
                PlayerPrefs.SetString(behaviourID.GetID, DateTime.Now.ToString());
            yield return _yieldInstruction;
        }
    }
    private void Spawn()
    {
        var _waypointRand = Random.Range(0, areaSpawn.childCount);
        var _posRand = GetRandomPoint(areaSpawn.GetChild(_waypointRand).position);
        
        var _enemyPrefabIdx = Random.Range(0, PoolEnemies.Count);
        var _poolEnemy = PoolEnemies[_enemyPrefabIdx];

        var _enemy = _poolEnemy.Get(_posRand);
        _enemy.OnDieEvent.AddListener(HandleEnemyDie);
    }
    private void HandleEnemyDie(EnemyController _enemy)
    {
        _currentEnemy--;
        _enemy.OnDieEvent.RemoveListener(HandleEnemyDie);
        CheckReward();
        
        if (isSaveEnemyData) PlayerPrefs.SetInt(PP_SaveCurrentEnemy, _currentEnemy);
    }
    private Vector3 GetRandomPoint(Vector3 waypointPosition)
    {
        var randomAngle = Random.insideUnitCircle * maxDistance;
        return waypointPosition + new Vector3(randomAngle.x, 0, randomAngle.y);;
    }
    private void CheckReward()
    {
        if (_checkAreaCoroutine != null) StopCoroutine(_checkAreaCoroutine);
        _checkAreaCoroutine = StartCoroutine(CheckCoroutine());
    }
    private IEnumerator CheckCoroutine()
    {
        yield return _yieldCheck;
        if (_currentEnemy > 0) yield break;

        yield return new WaitForSeconds(.5f);
        AreaCleanedEvent?.Invoke();
    }
    public void ReleaseAllEnemy()
    {
        if(_spawnCheckCoroutine != null) 
            StopCoroutine(_spawnCheckCoroutine);
        PoolEnemies.ForEach(x => x.List.ForEach(e => e.Release()));
    }
    public void SetTargetAtEnemy(GameObject _target)
    {
        PoolEnemies.ForEach(x => x.List.ForEach(e => e.SetRefTarget(_target)));
    }
    
    private void OnApplicationQuit()
    {
        if(isSaveEnemyData)
            PlayerPrefs.SetInt(PP_SaveCurrentEnemy, _currentEnemy);
    }
    private void OnDrawGizmos()
    {
        if(!areaSpawn || areaSpawn.childCount == 0 || !drawAreaSpawnGizmos) return;
        
        for (var i = 0; i < areaSpawn.transform.childCount; i++)
        {
            var _currentPoint = areaSpawn.transform.GetChild(i).position + new Vector3(0, .5f, 0);
            var _nextChildIndex = (i + 1) % areaSpawn.transform.childCount;
            var _nextPoint = areaSpawn.transform.GetChild(_nextChildIndex).position + new Vector3(0, .5f, 0);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_currentPoint, .35f);
            Gizmos.color = new Color(1, .5f, .5f, 1f);
            Gizmos.DrawWireSphere(_currentPoint, maxDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_currentPoint, _nextPoint);
        }
    }

    
}
