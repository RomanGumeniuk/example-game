using UnityEngine;

public class ShopTile : Tile
{
    public ShopTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }

    public override void OnPlayerStepped()
    {
        ShopTabUI.Instance.Show();
    }
}
