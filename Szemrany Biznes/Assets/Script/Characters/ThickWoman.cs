using Unity.Netcode;
using UnityEngine;

public class ThickWoman : Character
{
    public override int OnDiceRolled(int diceValue)
    {
        diceValue=base.OnDiceRolled(diceValue);
        return diceValue-1;
    }

    public override void Greetings()
    {
        Debug.Log("ThickWoman");
        name = "Baba Grzmot";
        isWoman = true;
    }

    const int AMOUNT_OF_TILES_THAT_SCARES_OTHER_PLAYERS = 2;
    public override bool OnPlayerStepped(TileScript tile)
    { 
        if(tile.ownerId.Value==-1 || tile.ownerId.Value == playerScript.playerIndex) return false;
        int ownerTileIndex = NetworkManager.Singleton.ConnectedClientsList[tile.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().currentTileIndex;
        for(int i= AMOUNT_OF_TILES_THAT_SCARES_OTHER_PLAYERS*-1; i< AMOUNT_OF_TILES_THAT_SCARES_OTHER_PLAYERS+1; i++)
        {
            if (ownerTileIndex != (playerScript.currentTileIndex + i) % ((GameLogic.Instance.mapGenerator.GetSize() * 4) - 4)) continue;
            _ = AlertTabForPlayerUI.Instance.ShowTab("W³aœciciel boi siê wzi¹Ÿæ od ciebie kasy",2,true);
            return true;
        }

        return false;
    }

}
