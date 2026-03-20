using System;
using DefaultNamespace;
using UnityEngine;
using DG.Tweening;
using DefaultNamespace.Map;
using Random = UnityEngine.Random;

public class FireballBounce : MonoBehaviour
{
    public float jumpPower = 2f;
    public float duration = 0.4f;

    public float damage = 1;

    private Tile currentTile;

    private void Start()
    {
        var tile = GetRandomTile();
        StartBounce(tile);
    }

    public void StartBounce(Tile startTile)
    {
        currentTile = startTile;
        transform.position = startTile.transform.position;

        BounceNext();
    }

    void BounceNext()
    {
        Tile nextTile = GetRandomTile();

        if (nextTile == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 targetPos = nextTile.transform.position;

        transform.DOJump(
                targetPos,
                jumpPower,
                1,
                duration
            ).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                OnHit(nextTile);
                currentTile = nextTile;
                BounceNext();
            });
    }

    void OnHit(Tile tile)
    {
        if (!tile) return;

        tile.TakeDamage(
            PlayerData.Instance.GnomeMineDamage,
            PlayerData.Instance.GnomeCritChance,
            PlayerData.Instance.GnomeCritDamage,
            0f
        );
    }

    Tile GetRandomTile()
    {
        var tiles = MapManager.Instance.GetAllTilesOfHighestHeight();

        if (tiles == null || tiles.Count == 0)
            return null;

        return tiles[Random.Range(0, tiles.Count)];
    }
}