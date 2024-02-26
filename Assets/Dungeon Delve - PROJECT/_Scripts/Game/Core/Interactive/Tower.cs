using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Tower : Damageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private CanvasGroup hud;
    
    public UnityEvent OnTakeDMGEvent;
    public UnityEvent OnDieEvent;

    private StatusHandle _health;
    private bool _isDie;
    private Tween _hudTween;
    private readonly float _hudDuration = .3f;
    
    private void Awake()
    {
        _health = new StatusHandle();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        RegisterEvent();
    }
    private void Start()
    {
        Init();
        hud.alpha = 0;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UnRegisterEvent();
    }
    
    private void RegisterEvent()
    {
        _health.OnInitValueEvent += healthBar.Init;
        _health.OnCurrentValueChangeEvent += healthBar.OnCurrentValueChange;
    }
    private void UnRegisterEvent()
    {
        _health.OnInitValueEvent -= healthBar.Init;
        _health.OnCurrentValueChangeEvent -= healthBar.OnCurrentValueChange;
    }
    public override void TakeDMG(int _damage, bool _isCRIT)
    {
        if (_isDie) return;
        
        _health.Decreases(_damage);
        DMGPopUpGenerator.Instance.Create(transform.position, _damage, _isCRIT, false);

        if (_health.CurrentValue <= 0)
        {
            DieHandle();
        }
        else
        {
            TakeDMGHandle();
        }
    }

    private void TakeDMGHandle()
    {
        OnTakeDMGEvent?.Invoke();
    }
    private void DieHandle()
    {
        OnDieEvent?.Invoke();
        CloseHUD();
        _isDie = true;
    }

    public void Init()
    {
        _isDie = false;
        _health.InitValue(maxHealth, maxHealth);
    }
    public void OpenHUD()
    {
        _hudTween?.Kill();
        _hudTween = hud.DOFade(1, _hudDuration);
    }
    public void CloseHUD()
    {
        _hudTween?.Kill();
        _hudTween = hud.DOFade(0, _hudDuration);
    }
    
}
