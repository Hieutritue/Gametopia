using UnityEngine;

[CreateAssetMenu(menuName = "Tile")]
public class TileData : ScriptableObject
{
    public int Hp;
    public int Reward;
    public int LayerType;
    public Material Material;
}