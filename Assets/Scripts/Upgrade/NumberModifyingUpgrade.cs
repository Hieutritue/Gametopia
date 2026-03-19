using System;
using UnityEngine;

namespace DefaultNamespace.Upgrade
{
    [CreateAssetMenu(menuName = "Upgrade/Number Modifying Upgrade")]
    public class NumberModifyingUpgrade : UpgradeData
    {
        public ENumberType NumberType; // Enum to specify which number to modify (e.g., MineDamage, MineCooldown, etc.)

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            if (PlayerData.Instance != null)
            {
                switch (NumberType)
                {
                    case ENumberType.MineDamage:
                        PlayerData.Instance.MouseDamage += (int)Value;
                        break;
                    case ENumberType.MineCooldown:
                        PlayerData.Instance.MouseCooldown += Value;
                        break;
                    case ENumberType.MineCritChance:
                        PlayerData.Instance.MouseCritChance += Value;
                        break;
                    case ENumberType.MineCritDamage:
                        PlayerData.Instance.MouseCritDamage += Value;
                        break;
                    case ENumberType.GnomeSpeed:
                        PlayerData.Instance.GnomeSpeed += Value;
                        break;
                    case ENumberType.GnomeMineCooldown:
                        PlayerData.Instance.GnomeMineCooldown += Value;
                        break;
                    case ENumberType.GnomeMineDamage:
                        PlayerData.Instance.GnomeMineDamage += (int)Value;
                        break;
                    case ENumberType.GnomeCritChance:
                        PlayerData.Instance.GnomeCritChance += Value;
                        break;
                    case ENumberType.GnomeCritDamage:
                        PlayerData.Instance.GnomeCritDamage += Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}