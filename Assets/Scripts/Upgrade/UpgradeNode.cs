using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace.Upgrade
{
    public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        [Header("Feedbacks")] [SerializeField] private MMF_Player _hoverFeedback;
        [SerializeField] private MMF_Player _buyFeedback;
        [SerializeField] private MMF_Player _cantBuyFeedback;

        [Header("Upgrade Levels")] public List<UpgradeData> upgrades = new();

        public int CurrentLevel { get; private set; }

        public bool Revealed { get; private set; }

        [Header("Events")] public UnityEvent OnRevealed;
        public UnityEvent OnFirstUpgrade;

        [Header("Visuals")] public GameObject hiddenVisual;
        public GameObject buyableVisual;
        public GameObject notEnoughMoneyVisual;
        public GameObject maxLevelVisual;
        [SerializeField] private Image[] _icons;

        public List<GameObject> levelVisuals = new();

        // ------------------------

        public bool IsMaxLevel => CurrentLevel >= upgrades.Count;

        private void Start()
        {
            Revealed = upgrades[0].InitiallyAvailable;
            _icons.ForEach(i => i.sprite = upgrades[0].Icon);
            // Initialize visuals
            UpdateVisuals();

            Money.Instance.OnMoneyChanged += _ => UpdateVisuals();
        }

        public UpgradeData GetCurrentUpgrade()
        {
            if (IsMaxLevel) return null;
            return upgrades[CurrentLevel];
        }

        // ------------------------
        // REVEAL
        // ------------------------

        public void Reveal()
        {
            if (Revealed)
                return;

            Revealed = true;

            OnRevealed?.Invoke();

            UpdateVisuals();

            foreach (var checkMark in levelVisuals)
            {
                checkMark.transform.parent.parent.gameObject.SetActive(true);
            }
        }

        // ------------------------
        // CHECK BUYABLE
        // ------------------------

        public bool CheckBuyable()
        {
            if (!Revealed)
                return false;

            if (IsMaxLevel)
                return false;

            var upgrade = GetCurrentUpgrade();

            return Money.Instance.Amount >= upgrade.Cost;
        }

        // ------------------------
        // BUY
        // ------------------------

        public void Buy()
        {
            if (!CheckBuyable())
            {
                _cantBuyFeedback?.PlayFeedbacks();
                return;
            }

            _buyFeedback?.PlayFeedbacks();
            bool firstUpgrade = CurrentLevel == 0;

            var upgrade = GetCurrentUpgrade();

            Money.Instance.Amount -= upgrade.Cost;

            upgrade.ApplyUpgrade();

            CurrentLevel++;

            // ✅ invoke first time event
            if (firstUpgrade)
                OnFirstUpgrade?.Invoke();

            UpdateVisuals();
            SetDesc();
        }

        // ------------------------
        // VISUALS
        // ------------------------

        [Button]
        public void UpdateVisuals()
        {
            if (!Revealed)
            {
                hiddenVisual.SetActive(true);
                buyableVisual.SetActive(false);
                notEnoughMoneyVisual.SetActive(false);
                maxLevelVisual.SetActive(false);
                foreach (var checkMark in levelVisuals)
                {
                    checkMark.transform.parent.parent.gameObject.SetActive(false);
                }

                return;
            }


            if (CurrentLevel < levelVisuals.Count && CurrentLevel > 0)
                levelVisuals[CurrentLevel - 1].SetActive(true);

            if (IsMaxLevel)
            {
                hiddenVisual.SetActive(false);
                buyableVisual.SetActive(false);
                notEnoughMoneyVisual.SetActive(false);
                maxLevelVisual.SetActive(true);
                return;
            }

            if (!CheckBuyable())
            {
                hiddenVisual.SetActive(false);
                buyableVisual.SetActive(false);
                notEnoughMoneyVisual.SetActive(true);
                maxLevelVisual.SetActive(false);
            }
            else
            {
                hiddenVisual.SetActive(false);
                buyableVisual.SetActive(true);
                notEnoughMoneyVisual.SetActive(false);
                maxLevelVisual.SetActive(false);
            }
        }

        void HideAll()
        {
            hiddenVisual.SetActive(false);
            buyableVisual.SetActive(false);
            notEnoughMoneyVisual.SetActive(false);
            maxLevelVisual.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoverFeedback?.PlayFeedbacks();
            SetDesc();
        }

        private void SetDesc()
        {
            var level = IsMaxLevel ? CurrentLevel - 1 : CurrentLevel;
            UpgradeDescription.Instance.SetDescription(Revealed ? upgrades[level] : null, IsMaxLevel);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Buy();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverFeedback?.StopFeedbacks();
            _hoverFeedback?.RestoreInitialValues();
            UpgradeDescription.Instance.SetDescription(null, false);
        }
    }
}