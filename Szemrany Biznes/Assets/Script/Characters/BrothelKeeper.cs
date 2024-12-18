using Unity.Netcode;
using UnityEngine;

public class BrothelKeeper : Character
{

    public override void Greetings()
    {
        Debug.Log("BrothelKeeper");
        name = "Burdel Mama";
    }

    const float EARNINGS_MULTIPLIER = 0.05f;

    public override int CheckCharacterMultipliersForPayments(int amountOfMoney, PropertyType propertyType)
    {
        if(propertyType == PropertyType.Prostitution) return Mathf.CeilToInt(amountOfMoney * GetCombineMultiplier());
        return Mathf.CeilToInt(amountOfMoney);
    }


    private float GetCombineMultiplier()
    {
        float combineMultiplier = 0;
        foreach (TileScript tile in playerScript.GetTilesThatPlayerOwnList())
        {
            if(tile.propertyType == PropertyType.Prostitution)
            {
                combineMultiplier++;
            }

        }
        return (combineMultiplier* EARNINGS_MULTIPLIER) +1;
    }

    public override void OnOwnedTileChange(TileScript tile)
    {
        foreach (TileScript playerTile in playerScript.GetTilesThatPlayerOwnList())
        {
            Debug.Log("town updated " + playerTile.name + " " + playerTile.ownerId.Value + " " + playerScript.playerIndex);
            if (playerTile.propertyType == PropertyType.Prostitution)
            {
                playerTile.UpdateOwnerTextServerRpc();
                Debug.Log("updated");
            }
                
        }



    }

}
