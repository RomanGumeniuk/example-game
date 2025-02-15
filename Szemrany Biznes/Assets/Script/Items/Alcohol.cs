using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alcohol : Item
{
    public Alcohol(string name,string description,int amountOfUses,int cost,RawImage icon,ItemType itemType , ItemTier alcoholType)
    {
        this.name = name;
        this.description = description;
        this.amountOfUses = amountOfUses;
        this.cost = cost;
        this.icon = icon;
        this.itemType = itemType;
        itemTier = alcoholType;
        
    }



    

    public override void OnItemUse()
    {
        int randomNumber;
        List<int> currentListOfTilesIndex = new List<int>();
        switch (itemTier)
        {
            case ItemTier.Junk:
                randomNumber = Random.Range(0, (GameLogic.Instance.mapGenerator.GetSize() * 4) - 4);
                playerScriptThatOwnsItem.TeleportToTile(randomNumber);
                break;
            case ItemTier.Normal:
                for(int i=0;i<GameLogic.Instance.allTileScripts.Count;i++)
                {
                    if (GameLogic.Instance.allTileScripts[i].tileType == TileType.PrisonTile) continue;
                    if (GameLogic.Instance.allTileScripts[i].tileType == TileType.BillTile) continue;
                    if (GameLogic.Instance.allTileScripts[i].tileType == TileType.PatrolTile) continue;
                    currentListOfTilesIndex.Add(i);
                }
                randomNumber = Random.Range(0,currentListOfTilesIndex.Count);
                playerScriptThatOwnsItem.TeleportToTile(currentListOfTilesIndex[randomNumber]);
                break;
            case ItemTier.Decent:
                for (int i = 0; i < GameLogic.Instance.allTileScripts.Count; i++)
                {
                    if (GameLogic.Instance.allTileScripts[i].tileType != TileType.TownTile) continue;
                    if (!(GameLogic.Instance.allTileScripts[i].ownerId.Value == -1 || GameLogic.Instance.allTileScripts[i].ownerId.Value == PlayerScript.LocalInstance.playerIndex)) continue;
                    currentListOfTilesIndex.Add(i);
                }
                randomNumber = Random.Range(0, currentListOfTilesIndex.Count);
                playerScriptThatOwnsItem.TeleportToTile(currentListOfTilesIndex[randomNumber]);
                break;
            case ItemTier.Exclusive:
                List<TileScript> playerOwnedTiles = PlayerScript.LocalInstance.GetTilesThatPlayerOwnList();
                for (int i=0;i< playerOwnedTiles.Count;i++)
                {
                    currentListOfTilesIndex.Add(GameLogic.GetRealTileIndexFromAllTiles(playerOwnedTiles[i].index));
                }
                randomNumber = Random.Range(0, currentListOfTilesIndex.Count);
                playerScriptThatOwnsItem.TeleportToTile(currentListOfTilesIndex[randomNumber]);
                break;
            case ItemTier.Relic:
                ShowUIForChoosingTile();
                break;
        }
    }


    async void ShowUIForChoosingTile()
    {
        TileScript tile = await ChooseingTileUI.Instance.Show();
        playerScriptThatOwnsItem.TeleportToTile(GameLogic.GetRealTileIndexFromAllTiles(tile.index));
    }

}
