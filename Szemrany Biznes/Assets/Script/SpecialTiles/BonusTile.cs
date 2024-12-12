using UnityEngine;

public class BonusTile : Tile
{
    public BonusTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }


    public override void OnPlayerStepped()
    {
        int number = Random.Range(1, 4);
        AlertTabForPlayerUI.Instance.ShowTab($"Dosta³eœ {number * 200}PLN", 2);
        tileScript.GiveMoney(number*200);
    }
}
