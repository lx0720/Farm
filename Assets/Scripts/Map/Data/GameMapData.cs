using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="GameMapData",menuName = "Map/MapData")]
public class GameMapData : ScriptableObject
{
    public GameScene gameScene;
    public Vector2Int mapSize;
    public Vector2Int mapOriginPosition;
    public List<GameTileData> gameTileDatas;
}

[System.Serializable]
public class GameTileData
{
    public Vector2Int tileCoordinate;
    public TileType tileType;
}