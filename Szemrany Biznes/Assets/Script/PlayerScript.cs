using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerScript : NetworkBehaviour
{
    public int directionX = 1;
    public int directionZ = 0;
    public int currentTileIndex = 0;
    public int playerIndex;
    public NetworkVariable<int> amountOfMoney = new NetworkVariable<int>( 3000);
    public List<TileScript> tilesThatPlayerOwnList = new List<TileScript>();
    public NetworkVariable<int> totalAmountOfMoney = new NetworkVariable<int>(3000);
    public byte currentAvailableTownUpgrade = 2;
    public byte minTownLevel=1;
    public NetworkVariable<int> cantMoveFor = new NetworkVariable<int>(0);

    public static PlayerScript LocalInstance { get; private set; }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }
    }

    public void GoTo(Vector3 postition)
    {
        transform.position = postition;
    }

    protected void CheckForSteppingOnEdge()
    {
        if (currentTileIndex % 10 != 0)
        {
            return;
        }

        switch (currentTileIndex)
        {
            case 0:
                directionX = -1;
                directionZ = 0;
                transform.position = GameLogic.Instance.SpawnPoints[0].GetChild(playerIndex).transform.position;
                break;
            case 10:
                directionX = 0;
                directionZ = 1;
                transform.position = GameLogic.Instance.SpawnPoints[1].GetChild(playerIndex).transform.position;
                break;
            case 20:
                directionX = 1;
                directionZ = 0;
                transform.position = GameLogic.Instance.SpawnPoints[2].GetChild(playerIndex).transform.position;
                break;
            case 30:
                directionX = 0;
                directionZ = -1;
                transform.position = GameLogic.Instance.SpawnPoints[3].GetChild(playerIndex).transform.position;
                break;
        }
    }

    protected void ChangeCurrentTileIndex(int diceValue,int i)
    {
        transform.position = new Vector3(transform.position.x + 1.17f * directionX, transform.position.y, transform.position.z + 1.17f * directionZ);
        if (currentTileIndex != (9 * 4) + 3)
        {
            currentTileIndex++;
            return;
        }
        currentTileIndex = 0;
        if (i + 1 < diceValue)
        {
            GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade, true);
        }
    }


    public void Move(int diceValue)
    {
        for(int i=0;i<diceValue;i++)
        {
            CheckForSteppingOnEdge();
            ChangeCurrentTileIndex(diceValue,i);
        }
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade);
        
    }

    public void ShowSellingTab(int amountOfMoneyThatNeedsToBePaid,int playerIndexThatGetsPaid)
    {
        if(amountOfMoneyThatNeedsToBePaid>totalAmountOfMoney.Value)
        {
            Debug.Log("Bankrut!!!");
            return;
        }
        SellingTabUI.Instance.Show(amountOfMoneyThatNeedsToBePaid, amountOfMoney.Value,playerIndexThatGetsPaid);
        foreach (TileScript tile in tilesThatPlayerOwnList)
        {
            tile.ShowSellingView();
        }
    }

    public void OnTownUpgrade()
    {
        minTownLevel = 6;
        foreach(TileScript tileScript in tilesThatPlayerOwnList)
        {
            if (tileScript.townLevel.Value == 0) continue;
            if (minTownLevel > tileScript.townLevel.Value) minTownLevel = (byte)tileScript.townLevel.Value;
        }
        currentAvailableTownUpgrade = minTownLevel;
    }

    [ClientRpc]
    public void SetMaterialClientRpc()
    {
        GetComponentInChildren<MeshRenderer>().material = GameLogic.Instance.PlayerColors[(int)GetComponent<NetworkObject>().OwnerClientId];
    }

}
