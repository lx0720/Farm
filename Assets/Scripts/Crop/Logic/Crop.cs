using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private Animator anim;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;

        //需要执行的次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        //收获次数
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            //如果当前收获的物体被收获时有动画则播放动画
            if (anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("RotateRight");
                else
                    anim.SetTrigger("RotateLeft");
            }
            //如果有粒子效果则播放粒子效果
            if (cropDetails.hasParticalEffect)
                EventCenter.CallParticleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
            //音效
            if (cropDetails.soundEffect != SoundName.none)
            {
                EventCenter.CallPlaySoundEvent(cropDetails.soundEffect);
            }
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                //收获次数大于需要收获的次数
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");
                else
                    anim.SetTrigger("FallingLeft");
                EventCenter.CallPlaySoundEvent(SoundName.TreeFalling);
                StartCoroutine(HarvestAfterAnimation());
            }
        }
    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }

        SpawnHarvestItems();
        //
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }

    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daysSinceLastHarvest = -1;
        tileDetails.growthDays = 0;

        EventCenter.CallRefreshCurrentMap();
    }

    /// <summary>
    /// 生成庄稼在背包
    /// </summary>
    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;

            //生成确定的数量
            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else    //否则随机生成最小和最大之间
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            }

            //
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventCenter.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else   
                {
                    //根据当前物体和玩家的距离来判断生成的方向
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    //生成的位置
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                    transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                    EventCenter.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                }
            }
        }

        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;

            //作物可以重复生长
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes - 1)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //改变tile的数据，然后刷新地图
                EventCenter.CallRefreshCurrentMap();
            }
            else  
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
            }

            Destroy(gameObject);
        }

    }
}
