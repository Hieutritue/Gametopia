using System;
using System.Collections;
using DefaultNamespace;
using DefaultNamespace.Map;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileData _data;
    private int _currentHealth;
    public int RewardMoney = 10;
    public Column Column { get; set; }

    [SerializeField] MMF_Player _hurtFeedback;
    [SerializeField] MMF_Player _destroyFeedback;
    [SerializeField] MMF_Player _explosionFeedback;

    [SerializeField] Gradient _normalGradient;
    [SerializeField] Gradient _critGradient;

    [SerializeField] GameObject[] _healthIndicators;
    
    private MMF_FloatingText _floatingText;

    private void Start()
    {
        _currentHealth = _data.Hp;
        RewardMoney = _data.Reward;
        _floatingText = _hurtFeedback.GetFeedbackOfType<MMF_FloatingText>();
    }

    private void OnMouseDown()
    {
        if (!MouseDigger.Instance.CanDig) return;

        MouseDigger.Instance.StartCooldown();
        
        var damage = PlayerData.Instance.MouseDamage;

        if (PlayerData.Instance.AccumulateDamageEnabled)
        {
            damage = MouseDigger.Instance.ConsumeStacksAndGetBonusDamage(damage);
        }
        
        TakeDamage(damage,
            PlayerData.Instance.MouseCritChance,
            PlayerData.Instance.MouseCritDamage,
            PlayerData.Instance.ExplosionChance);
    }

    public void TakeDamage(int damage, float critChance = 0f, float critDamage = 1f, float explosionChance = 0f)
    {
        int finalDamage = damage;
        bool isCrit = false;

        // roll crit
        if (critChance > 0f)
        {
            if (Random.value * 100 <= critChance)
            {
                finalDamage = Mathf.RoundToInt(damage * critDamage);
                isCrit = true;
            }
        }

        _currentHealth -= finalDamage;
        
        if (_currentHealth <= _data.Hp * 0.75f)
        {
            _healthIndicators[0].SetActive(true);
        }
        if (_currentHealth <= _data.Hp * 0.5f)
        {
            _healthIndicators[1].SetActive(true);
        }
        if (_currentHealth <= _data.Hp * 0.25f)
        {
            _healthIndicators[2].SetActive(true);
        }

        if (_floatingText != null)
        {
            _floatingText.Value = finalDamage.ToString();
            _floatingText.AnimateColorGradient = isCrit ? _critGradient : _normalGradient;
        }

        _hurtFeedback?.PlayFeedbacks();

        // ✅ explosion roll
        if (Random.value * 100 <= explosionChance)
        {
            PlayerData.Instance.StartCoroutine(ExplodeSpread(finalDamage));
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ExplodeSpread(int originalDamage)
    {
        _explosionFeedback?.PlayFeedbacks();
        int radius = PlayerData.Instance.ExplosionRadius;
        float mult = PlayerData.Instance.ExplosionDamageMult;

        int explosionDamage = Mathf.RoundToInt(originalDamage * mult);

        MapManager map = MapManager.Instance;

        if (Column.X == -1) yield break;

        for (int x = Column.X - radius; x <= Column.X + radius; x++)
        {
            for (int y = Column.Y - radius; y <= Column.Y + radius; y++)
            {
                if (x < 0 || y < 0 ||
                    x >= map.Columns.GetLength(0) ||
                    y >= map.Columns.GetLength(1))
                    continue;

                Column col = map.Columns[x, y];

                if (col == null) continue;

                Tile t = col.TopTile;

                if (t == null) continue;

                if (t == this) continue;

                yield return new WaitForSeconds(0.05f);

                t.TakeDamage(explosionDamage, explosionChance: 0);
            }
        }
    }

    private void Die()
    {
        Column.Tiles.Remove(this);

        _destroyFeedback?.PlayFeedbacks();

        Money.Instance.AddMoney(
            (int)((RewardMoney + PlayerData.Instance.IncomeAdd) * PlayerData.Instance.IncomeMult + .5f)
        );

        Destroy(gameObject);
    }
}