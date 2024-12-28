using UnityEngine;

public class BonusTile : Tile
{
    public BonusTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }

    const int AMOUNT_OF_MONEY = 200;

    public override void OnPlayerStepped()
    {
        int number = Random.Range(1, 4);
        if (PlayerScript.LocalInstance.character.GetType() == typeof(Student)) number = Mathf.CeilToInt(number * 1.5f);
        //tileScript.GiveMoney(number* AMOUNT_OF_MONEY);
        int tileIndex = Random.Range(0,GameLogic.Instance.allTileScripts.Count);
        GameLogic.Instance.SpawnDeadDropBoxServerRpc(tileIndex, number * AMOUNT_OF_MONEY);
        _ = AlertTabForPlayerUI.Instance.ShowTab($"Znajomy zrzuci³ ci paczkê. Dotrzyj do niej pierwszy a zyskasz {number * AMOUNT_OF_MONEY} PLN", 2.5f);
        
    }
}
