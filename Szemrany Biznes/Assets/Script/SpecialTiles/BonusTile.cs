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
        _ = AlertTabForPlayerUI.Instance.ShowTab($"Dosta³eœ {number * AMOUNT_OF_MONEY}PLN", 2);
        tileScript.GiveMoney(number* AMOUNT_OF_MONEY);
    }
}
