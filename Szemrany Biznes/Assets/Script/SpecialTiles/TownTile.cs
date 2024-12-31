using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class TownTile : Tile
{
    public TownTile(TileScript tile)
    {
        tileScript = tile;
    }

    const float AIRPORT_MULTIPLIER = 1.1f;
    const float UNMARKED_TRUCKS_MULTIPLIER = 1.1f;
    const float PIMP_CAR_MULTIPLIER = 1.1f;
    public override int GetPayAmount()
    {
        if (tileScript.destroyPercentage.Value > 0) return 0;

        if (tileScript.propertyType == PropertyType.None || tileScript.propertyType == PropertyType.Gambling) return base.GetPayAmount();
        int curentPayValue;
        bool isAirportInStock;
        switch (tileScript.propertyType)
        {
            case PropertyType.Prostitution:
                curentPayValue = base.GetPayAmount();
                isAirportInStock = false;
                foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
                {
                    if (tile.tileType != TileType.SpecialTile) continue;
                    if ((tile.specialTileScript as SpecialTile).type != SpecialTile.Type.PimpCar) continue;
                    isAirportInStock = true;
                    break;
                }
                return isAirportInStock ? (int)(curentPayValue * PIMP_CAR_MULTIPLIER) : curentPayValue;
            case PropertyType.Alcohol:

                curentPayValue = base.GetPayAmount();
                isAirportInStock = false;
                foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
                {
                    if (tile.tileType != TileType.SpecialTile) continue;
                    if ((tile.specialTileScript as SpecialTile).type != SpecialTile.Type.UnmarkedTrucks) continue;
                    isAirportInStock = true;
                    break;
                }
                return isAirportInStock ? (int)(curentPayValue * UNMARKED_TRUCKS_MULTIPLIER) : curentPayValue;

            case PropertyType.Drugs:

                curentPayValue = base.GetPayAmount();
                isAirportInStock = false;
                foreach (TileScript  tile in NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList()) 
                {
                    if (tile.tileType != TileType.SpecialTile) continue;
                    if((tile.specialTileScript as SpecialTile).type != SpecialTile.Type.FieldAirport) continue;
                    isAirportInStock = true;
                    break;
                }
                return isAirportInStock ? (int)(curentPayValue * AIRPORT_MULTIPLIER) : curentPayValue;
        }
        return base.GetPayAmount();

    }

    public override void OnOwnerIDChanged(int prevValue, int newValue)
    {
        //Debug.Log("owner id changed " + tileScript.name);
        if (newValue != -1)
        {
            foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[newValue].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
            {
                if (TileType.SpecialTile != tile.tileType) continue;
                tile.UpdateOwnerTextServerRpc();
            }
        }
        
        if (prevValue == -1) return;
        foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[prevValue].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
        {
            if (TileType.SpecialTile != tile.tileType) continue;
            tile.UpdateOwnerTextServerRpc();
        }
    }

    public override void OnTownLevelChanged(int prevValue, int newValue)
    {
        Debug.Log("town level changed " + tileScript.name + " "+  tileScript.ownerId.Value + " " + prevValue + " " + newValue);
        if (tileScript.ownerId.Value == -1) return;
        foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
        {
            if (TileType.SpecialTile != tile.tileType) continue;
            tile.UpdateOwnerTextServerRpc();
        }
    }

    public override async void OnPlayerStepped(int value1 = 0, float value2 = 0)
    {
        if (tileScript.tileType!=TileType.TownTile)
        {
            base.OnPlayerStepped(value1, value2);
            return; 
        }
        if(tileScript.ownerId.Value == PlayerScript.LocalInstance.playerIndex)
        {
            await OnPropertyAction();
        }
        tileScript.OnTownEnter(value1, (byte)value2);
    }

    public override async Task OnPropertyAction()
    {
        Debug.Log("COKOLWIEK");
        switch (tileScript.propertyType)
        {
            case PropertyType.Prostitution:

                break;
            case PropertyType.Gambling:
                await GamblingTabUI.Instance.Show(tileScript);
                break;
            case PropertyType.Alcohol:

                break;
            case PropertyType.Drugs:

                break;
        }
    }

}