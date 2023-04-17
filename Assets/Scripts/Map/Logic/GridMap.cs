using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public GameMapData mapData;
    public TileType tileType;
    private Tilemap currentTilemap;
    private void OnEnable()
    {
        if (!IsPlaying())
        {
            currentTilemap ??= GetComponent<Tilemap>();

            if (mapData != null)
                mapData.gameTileDatas.Clear();
        }
    }
    private void OnDisable()
    {
        if (!IsPlaying())
        {
            currentTilemap ??= GetComponent<Tilemap>();
            UpdateTileDatas();

#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData);
#endif
        }
    }
    private void UpdateTileDatas()
    {
        currentTilemap.CompressBounds();
        if (!IsPlaying() && mapData != null)
        {
            Vector3Int startPos = currentTilemap.cellBounds.min;
            Vector3Int endPos = currentTilemap.cellBounds.max;

            for (int x = startPos.x; x < endPos.x; x++)
            {
                for (int y = startPos.y; y < endPos.y; y++)
                {
                    if (currentTilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                    {
                        GameTileData tileData = new GameTileData
                        {
                            tileCoordinate = new Vector2Int(x, y),
                            tileType = tileType,
                        };
                        mapData.gameTileDatas.Add(tileData);
                    }
                }
            }
        }
    }
    public bool IsPlaying()
    {
        return Application.IsPlaying(this);
    }
}
