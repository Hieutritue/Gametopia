using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.Upgrade
{
    public class UpgradeData : ScriptableObject
    {
        public bool InitiallyAvailable;
        public string UpgradeName;
        [PreviewField] public Sprite Icon;
        [TextArea] public string Description;
        public int Cost;
        public float Value = -1;

        public virtual void ApplyUpgrade()
        {
            // This method can be overridden by specific upgrade types to apply their effects.
        }

        public string GetDescription()
        {
            return string.Format(Description, Value);
        }
    }

    public enum ENumberType
    {
        MineDamage,
        MineCooldown,
        MineCritChance,
        MineCritDamage,
        GnomeSpeed,
        GnomeMineCooldown,
        GnomeMineDamage,
        GnomeCritChance,
        GnomeCritDamage,
        
    }
}