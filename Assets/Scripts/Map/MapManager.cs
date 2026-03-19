using System;
using System.Collections.Generic;
using DefaultNamespace.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Column _columnPrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _depth;
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
}