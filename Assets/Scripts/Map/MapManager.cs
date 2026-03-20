using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Map;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Column _columnPrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _depth;
    [SerializeField] private ParticleSystem _launchEffect;
    [SerializeField] private ParticleSystem _nukeEffect;
    [SerializeField] private UpgradeOpenClose _upgradeOpenClose;
    [SerializeField] private MMF_Player _nukeFeedback;
    public Column[,] Columns { get; set; }

    private void Start()
    {
        SpawnColumns();
    }

    private void SpawnColumns()
    {
        var xOffset = 0;
        Columns = new Column[_width, _depth];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _depth; y++)
            {
                Column column = Instantiate(_columnPrefab, transform);
                column.name = $"Column_{x}_{y}";
                column.transform.localPosition = new Vector3(x , 0, y);
                Columns[x, y] = column;
                column.X = x;
                column.Y = y;
            }
        }
    }
    
    public List<Tile> GetAllTilesOfHighestHeight()
    {
        List<Tile> tiles = new List<Tile>();
        int maxHeight = HighestHeight();
        foreach (var c in Columns)
        {
            if (c.Tiles.Count == maxHeight + 1)
                tiles.Add(c.TopTile);
        }

        return tiles;
    }

    public int HighestHeight()
    {
        int max = 0;
        foreach (var c in Columns)
        {
            if (c.Tiles.Count > max)
                max = c.Tiles.Count;
        }

        return max - 1;
    }

    [Button]
    public void StartNuke()
    {
        StartCoroutine(Nuke());
    }
    
    public void DamageAllTopTiles()
    {
        _nukeEffect.transform.position = new Vector3(_width / 2f, HighestHeight() * .4f, _depth / 2f);
        _nukeFeedback?.PlayFeedbacks();
        _nukeEffect.Play();
        foreach (var c in Columns)
        {
            if (c.Tiles.Count == 0) continue;
            
            c.TopTile.TakeDamage(1, explosionChance: 0);
        }
    }

    private IEnumerator Nuke()
    {
        _upgradeOpenClose.OnPointerEnter(null);
        _launchEffect.Play();
        yield return new WaitForSeconds(2f);
        _launchEffect.Stop();
        yield return new WaitForSeconds(1.2f);
        DamageAllTopTiles();
    }
}