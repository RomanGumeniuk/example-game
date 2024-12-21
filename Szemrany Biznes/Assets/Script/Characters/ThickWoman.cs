using Unity.Netcode;
using UnityEngine;

public class ThickWoman : Character
{
    public override int OnDiceRolled(int diceValue)
    {
        Debug.Log("Roll decresed by one");
        return diceValue-1;
    }

    public override void Greetings()
    {
        Debug.Log("ThickWoman");
        name = "Baba Grzmot";
    }


    public override bool OnPlayerStepped(TileScript tile)
    { 
        if(tile.ownerId.Value==-1 || tile.ownerId.Value == playerScript.playerIndex) return false;
        return true;
    }

}
