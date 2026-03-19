using System;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class Money : Singleton<Money>
    {
        private int _amount;
        [SerializeField] private TMP_Text _amountText;

        public Action<int> OnMoneyChanged;

        private void Start()
        {
            _amountText.SetText("Moneyyy: 0");
        }

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                _amountText.SetText("Moneyyy: " + value.ToString());
                OnMoneyChanged?.Invoke(_amount);
            }
        }
        
        [Button]
        public void AddMoney(int amount)
        {
            Amount += amount;
        }
    }
}