using UnityEngine;
using DefaultNamespace.Map;
using Sirenix.OdinInspector;

public class GnomeSpawner : Singleton<GnomeSpawner>
{
    public GameObject gnomePrefab;
    
    [Button]
    public void SpawnGnome()
    {
        Tile tile = GetRandomTopTile();

        if (tile == null) return;

        Vector3 pos = tile.transform.position + Vector3.up;

        Instantiate(
            gnomePrefab,
            pos,
            Quaternion.identity,
            transform
        );
    }

    Tile GetRandomTopTile()
    {
        var tiles = MapManager.Instance.GetAllTilesOfHighestHeight();

        if (tiles.Count == 0) return null;

        int randomIndex = Random.Range(0, tiles.Count);
        return tiles[randomIndex];
    }
}