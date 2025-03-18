using UnityEngine;

public class MelangeTile : Tile
{
    MelangeTile(TileScript tile)
    {
        tileScript = tile;
    }

    public override void OnPlayerStepped()
    {
        GameLogic gameLogic = GameLogic.Instance;
        DiceSpawn diceSpawn = DiceSpawn.Instance;
        for (int i =0;i < gameLogic.allPlayerAmount;i++)
        {
            int playerId =(int)gameLogic.PlayersOrder[i].ClientId;
            diceSpawn.RollTheDiceServerRpc(playerId,1,true,false,DiceType.EightSide);
            diceSpawn.RollTheDiceServerRpc(playerId, 1, true, false, DiceType.SixSide);
            diceSpawn.RollTheDiceServerRpc(playerId, 1, true, false, DiceType.FourSide);
        }
    
    }
}
