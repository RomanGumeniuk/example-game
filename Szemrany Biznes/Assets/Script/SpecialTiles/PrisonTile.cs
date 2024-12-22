using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class PrisonTile : Tile
{
    public PrisonTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }


    const int MAX_AMOUNT_OF_TURNS_TO_EXIT_PRISON = 5;

    public override void OnPlayerStepped()
    {
        int prisonerIndex = -1;
        for(int i=0;i<NetworkManager.Singleton.ConnectedClientsList.Count;i++)
        {
            if (i == PlayerScript.LocalInstance.playerIndex) continue;
            if (!NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerScript>().isInPrison.Value) continue;
            prisonerIndex = i;
        }
        Debug.Log("Prison index: "+prisonerIndex);
        if(prisonerIndex == -1)
        {
            PlayerScript.LocalInstance.SetIsInPrisonServerRpc(true);
            tileScript.UpdatePlayerCantMoveVariableServerRpc(MAX_AMOUNT_OF_TURNS_TO_EXIT_PRISON, PlayerScript.LocalInstance.playerIndex);
            _ = AlertTabForPlayerUI.Instance.ShowTab("Trafi³eœ do wiêzienia", 1.5f);
            return;
        }
        PrisonTabUI.Instance.Show();
    }


}
