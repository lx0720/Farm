using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileDetails 
{
    public GameScene gameScene;
    public int tileX, tileY;
    public bool canDig;
    public bool canDrop;
    public bool canPlace;
    public bool cantWalk;
    public int seedId = -1;
    public int growthDays = -1;
    public int dugDaysCount = -1;
    public int waterDaysCount = -1;
    public int daysSinceLastHarvest = -1;
}
