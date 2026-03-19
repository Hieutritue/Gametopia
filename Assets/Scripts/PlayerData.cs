using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class PlayerData : Singleton<PlayerData>
    {
        [Header("Mouse Stats")] public int MouseDamage;
        public float MouseCooldown;
        public float MouseCritChance;
        public float MouseCritDamage;

        [Header("Income")] public int IncomeAdd;
        public float IncomeMult;

        [Header("Gnome Stats")] public float GnomeSpeed;
        public float GnomeMineCooldown;
        public int GnomeMineDamage;
        public float GnomeCritChance;
        public float GnomeCritDamage;
        public float GnomeChanceToTriggerAttackTwice;
        public float GnomeJumpCooldown;

        [Header("Explosion")] public float ExplosionChance = 0.2f;
        public float ExplosionDamageMult = 0.15f;
        public int ExplosionRadius = 1;

        [Header("Accumulate Damage")] 
        public bool AccumulateDamageEnabled = true;
        public int MaxAccumulate = 5;
        public float AccumulateDuration = 5f;
        public float DamagePerStack = 0.2f;
    }
}