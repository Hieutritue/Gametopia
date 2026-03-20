using System;
using DefaultNamespace;
using DefaultNamespace.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDescription : Singleton<UpgradeDescription>
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private Vector2 _offset;
    private UpgradeData _currentUpgrade;
    private bool _maxed;

    private void Start()
    {
        Money.Instance.OnMoneyChanged += _ => SetDescription(_currentUpgrade,_maxed);
        gameObject.SetActive(false);
    }

    public void SetDescription(UpgradeData upgradeData, bool maxed)
    {
        _currentUpgrade = upgradeData;
        _maxed = maxed;
        if (upgradeData == null)
        {
            _icon.sprite = null;
            _name.SetText("");
            _description.SetText("");
            _cost.SetText("");
            gameObject.SetActive(false);
            return;
        }

        _icon.sprite = upgradeData.Icon;
        _name.SetText(upgradeData.UpgradeName);
        _description.SetText(upgradeData.GetDescription());
        var color = Money.Instance.Amount >= upgradeData.Cost ? "green" : "red";
        _cost.SetText($"Cost: <color={color}>{Money.Instance.Amount} / {upgradeData.Cost}</color>");
        if (maxed)
        {
            _cost.SetText("Max Level");
        }
        gameObject.SetActive(true);
    }
    void FollowMouse()
    {
        if (!gameObject.activeSelf) return;

        RectTransform rect = transform as RectTransform;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        rect.localPosition = pos + _offset;
    }

    private void Update()
    {
        FollowMouse();
    }
}