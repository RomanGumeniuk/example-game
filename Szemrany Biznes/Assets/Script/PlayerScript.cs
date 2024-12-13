using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;
using Unity.Cinemachine;

public class PlayerScript : NetworkBehaviour
{
    public int currentTileIndex = 0;
    public int playerIndex;
    public NetworkVariable<int> amountOfMoney = new NetworkVariable<int>(7000);
    [SerializeField]
    private List<TileScript> tilesThatPlayerOwnList = new List<TileScript>();
    public NetworkVariable<int> totalAmountOfMoney = new NetworkVariable<int>(7000);
    public byte currentAvailableTownUpgrade = 2;
    public byte minTownLevel=1;
    public NetworkVariable<int> cantMoveFor = new NetworkVariable<int>(0);

    public static PlayerScript LocalInstance;

    public NavMeshAgent navMeshAgent;

    private CinemachineCamera camera;

    public Character character;

    public Tile tileType;
    

    public override void OnNetworkSpawn()
    {
        
        if (IsOwner)
        {
            LocalInstance = this;
            navMeshAgent = GetComponent<NavMeshAgent>();
            //Debug.Log("AAA" +PlayerScript.LocalInstance.playerIndex);
            /*camera = FindAnyObjectByType<CinemachineCamera>();
            CameraTarget target = new CameraTarget();
            target.TrackingTarget = transform;
            camera.Target = target;
            camera.LookAt = transform;*/
        }
    }

    public void GoTo(Vector3 postition)
    {
        transform.position = postition;
        navMeshAgent.enabled = true;
    }


    protected async Task ChangeCurrentTileIndex(int diceValue, int i)
    {
        int index = 0;
        if (currentTileIndex > GameLogic.Instance.mapGenerator.GetSize() - 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 2)
        {
            index = GameLogic.Instance.mapGenerator.GetSize()-1;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 1 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 3)
        {
            index = (GameLogic.Instance.mapGenerator.GetSize() - 1)*2;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 4 + 4)
        {
            index = (GameLogic.Instance.mapGenerator.GetSize() - 1) * 3;
        }


        if (currentTileIndex != ((GameLogic.Instance.mapGenerator.GetSize() - 1) * 4)-1)
        {
            currentTileIndex++;
        }
        else currentTileIndex = 0;
        
        navMeshAgent.destination = GameLogic.Instance.allTileScripts[currentTileIndex].transform.position - (GameLogic.Instance.allTileScripts[index].transform.position - GameLogic.Instance.SpawnPoints[index/ (GameLogic.Instance.mapGenerator.GetSize() - 1)].GetChild(playerIndex).transform.position);
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (navMeshAgent.remainingDistance < 0.1f) break;
        }

        
        if (i + 1 < diceValue)
        {
            GameLogic.Instance.allTileScripts[currentTileIndex].specialTileScript?.OnPlayerPassBy();
        }
    }
    [ClientRpc]
    public void OnDiceNumberReturnClientRpc(int diceValue, ClientRpcParams clientRpcParams = default)
    {
        diceValue = LocalInstance.character.OnDiceRolled(diceValue);
        GameUIScript.Instance.OnDiceNumberReturn(diceValue);
        LocalInstance.Move(diceValue);
    }


    public async void Move(int diceValue)
    {
        navMeshAgent.isStopped = false;
        for (int i=0;i<diceValue;i++)
        {
            await ChangeCurrentTileIndex(diceValue,i);
        }
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade);
        navMeshAgent.isStopped = true;

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
            if (minTownLevel > tileScript.townLevel.Value) minTownLevel = (byte)(tileScript.townLevel.Value+1);
        }
        currentAvailableTownUpgrade = minTownLevel;
    }
    [ClientRpc]
    public void SetMaterialClientRpc()
    {
        GetComponentInChildren<MeshRenderer>().material = GameLogic.Instance.PlayerColors[(int)GetComponent<NetworkObject>().OwnerClientId];
    }

    [ServerRpc(RequireOwnership =false)]
    public void AddTilesThatPlayerOwnListServerRpc(int tileIndex)
    {
        AddTilesThatPlayerOwnListClientRpc(tileIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveTilesThatPlayerOwnListServerRpc(int tileIndex)
    {
        RemoveTilesThatPlayerOwnListClientRpc(tileIndex);
    }

    [ClientRpc]
    private void AddTilesThatPlayerOwnListClientRpc(int tileIndex)
    {
        tilesThatPlayerOwnList.Add(GameLogic.Instance.board.transform.GetChild(tileIndex).GetComponent<TileScript>());
    }

    [ClientRpc]
    private void RemoveTilesThatPlayerOwnListClientRpc(int tileIndex)
    {
        tilesThatPlayerOwnList.Remove(GameLogic.Instance.board.transform.GetChild(tileIndex).GetComponent<TileScript>());
    }

    public List<TileScript> GetTilesThatPlayerOwnList()
    {
        return tilesThatPlayerOwnList;
    }

}
