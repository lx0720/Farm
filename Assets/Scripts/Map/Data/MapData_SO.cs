using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapData_SO",menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    public GameScene gameScene;
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;
    public List<TileProperty> tileProperties;
}
