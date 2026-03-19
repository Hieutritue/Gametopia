using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MouseDigger : Singleton<MouseDigger>
{
    public float DigCooldown => PlayerData.Instance.MouseCooldown;

    private float _lastDigTime;

    public bool CanDig =>
        Time.time >= _lastDigTime + Mathf.Max(0.0001f, DigCooldown);

    [Tooltip("UI Image must be set to Filled")]
    public Image CooldownImage;

    public bool InvertFill = true;

    // -------------------------
    // Mouse follow
    // -------------------------

    [Header("Mouse Follow")]
    public bool FollowMouse = true;

    public bool FollowOnlyWhenActive;

    public Vector2 MouseOffset = Vector2.zero;

    private RectTransform _cooldownRect;
    private Canvas _parentCanvas;

    // -------------------------
    // Accumulate damage
    // -------------------------
    
    public int MaxAccumulate => PlayerData.Instance.MaxAccumulate; 
    public float AccumulateDuration => PlayerData.Instance.AccumulateDuration;
    public float DamagePerStack => PlayerData.Instance.DamagePerStack;

    private int _accumulateCount;

    private float _lastAccumulateTime;

    // -------------------------

    protected override void Awake()
    {
        base.Awake();

        if (CooldownImage != null)
        {
            _cooldownRect = CooldownImage.rectTransform;
            _parentCanvas = CooldownImage.GetComponentInParent<Canvas>();
        }
    }

    // -------------------------
    // PUBLIC
    // -------------------------

    public void StartCooldown()
    {
        _lastDigTime = Time.time;
        _lastAccumulateTime = Time.time;

        UpdateCooldownImage();
    }

    public int ConsumeStacksAndGetBonusDamage(int baseDamage)
    {
        int stacks = Mathf.Max(1, _accumulateCount);

        float mult = 1f + stacks * DamagePerStack;

        _accumulateCount = 0;

        return Mathf.RoundToInt(baseDamage * mult);
    }

    public int CurrentStacks => _accumulateCount;

    // -------------------------
    // UPDATE
    // -------------------------

    private void Update()
    {
        UpdateCooldownImage();
        UpdateMouseFollow();
        UpdateAccumulate();
    }

    // -------------------------
    // ACCUMULATE
    // -------------------------

    private void UpdateAccumulate()
    {
        if (!CanDig)
            return;

        // add stack every cooldown time
        if (Time.time >= _lastAccumulateTime + DigCooldown)
        {
            _lastAccumulateTime = Time.time;

            if (_accumulateCount < MaxAccumulate)
            {
                _accumulateCount++;
                
            }
        }

        // reset if waited too long
        if (Time.time > _lastDigTime + AccumulateDuration)
        {
            _accumulateCount = 0;
        }
    }

    // -------------------------
    // COOLDOWN UI
    // -------------------------

    private void UpdateCooldownImage()
    {
        if (CooldownImage == null)
            return;

        if (DigCooldown <= 0f)
        {
            CooldownImage.fillAmount = InvertFill ? 0f : 1f;
            return;
        }

        float endTime = _lastDigTime + DigCooldown;

        float remaining = Mathf.Clamp(
            endTime - Time.time,
            0f,
            DigCooldown
        );

        float normalizedRemaining =
            Mathf.Clamp01(remaining / DigCooldown);

        float fill =
            InvertFill
                ? normalizedRemaining
                : 1f - normalizedRemaining;

        CooldownImage.fillAmount = fill;
    }

    // -------------------------
    // FOLLOW MOUSE
    // -------------------------

    private void UpdateMouseFollow()
    {
        if (!FollowMouse || CooldownImage == null)
            return;

        if (FollowOnlyWhenActive && CanDig)
            return;

        if (_cooldownRect == null)
            _cooldownRect = CooldownImage.rectTransform;

        if (_parentCanvas == null)
            _parentCanvas =
                CooldownImage.GetComponentInParent<Canvas>();

        Vector2 mousePos = Input.mousePosition;

        if (
            _parentCanvas != null &&
            _parentCanvas.renderMode
            != RenderMode.ScreenSpaceOverlay
        )
        {
            RectTransform canvasRect =
                _parentCanvas.transform as RectTransform;

            Camera cam =
                _parentCanvas.renderMode
                == RenderMode.ScreenSpaceCamera
                    ? _parentCanvas.worldCamera
                    : null;

            Vector2 localPoint;

            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    mousePos,
                    cam,
                    out localPoint
                );

            _cooldownRect.anchoredPosition =
                localPoint + MouseOffset;
        }
        else
        {
            Vector3 worldPos =
                mousePos +
                new Vector2(
                    MouseOffset.x,
                    MouseOffset.y
                );

            _cooldownRect.position = worldPos;
        }
    }
}