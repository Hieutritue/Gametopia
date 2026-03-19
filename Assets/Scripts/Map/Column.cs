using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.Map
{
    public class Column : MonoBehaviour
    {
        public int X, Y;
        public List<Tile> Tiles;
        public Tile TopTile => Tiles.Count > 0 ? Tiles[^1] : null;

        private void Start()
        {
            SetupChild();
        }

        private void SetupChild()
        {
            foreach (var t in Tiles)
            {
                t.Column = this;
            }
        }
        
        [Button]
        public void SetupChildPosition()
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].transform.localPosition = new Vector3(0, i * 0.4f, 0);
            }
        }
    }
}