using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private bool isPlayOnStart = true;
    [SerializeField] private float time;
    [SerializeField] private TextMeshProUGUI timeText;

    public UnityEvent OnEndTimeEvent;
    private readonly YieldInstruction _yieldTime = new WaitForSeconds(1f);
    private Coroutine _timeCoroutine;


    private void Start()
    {
        SetFrameFill(false);
        if (isPlayOnStart) 
            StartCountdown();
    }

    public void SetTimeColldown(float _newTime) => time = _newTime;
    public void StartCountdown()
    {
        if (_timeCoroutine != null) 
            StopCoroutine(_timeCoroutine);
        _timeCoroutine = StartCoroutine(TimeCoroutine());
    }
    private IEnumerator TimeCoroutine()
    {
        SetFrameFill(true);
        var _timeTemp = time;
        while (_timeTemp >= 0)
        {
            SetTimeText(_timeTemp);
            _timeTemp--;
            yield return _yieldTime;
        }
        
        SetFrameFill(false);
        OnEndTimeEvent?.Invoke();
    }

    public void SetFrameFill(bool _activeValue)
    {
        if (frame)
            frame.gameObject.SetActive(_activeValue);
    }
    private void SetTimeText(float _currentTime)
    {
        var _hour = Mathf.Floor(_currentTime / 60.0f);
        var _minu = _currentTime % 60.0f;
        timeText.text = $"{_hour:00}:{_minu:00}";
    }

}
