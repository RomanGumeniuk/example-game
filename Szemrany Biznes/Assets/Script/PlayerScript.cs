using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;
using Unity.Cinemachine;

public abstract class PlayerScript : NetworkBehaviour
{
    public int directionX = 1;
    public int directionZ = 0;
    public int currentTileIndex = 0;
    public int playerIndex;
    public NetworkVariable<int> amountOfMoney = new NetworkVariable<int>(5000);
    public List<TileScript> tilesThatPlayerOwnList = new List<TileScript>();
    public NetworkVariable<int> totalAmountOfMoney = new NetworkVariable<int>(5000);
    public byte currentAvailableTownUpgrade = 2;
    public byte minTownLevel=1;
    public NetworkVariable<int> cantMoveFor = new NetworkVariable<int>(0);

    public static PlayerScript LocalInstance { get; protected set; }

    public NavMeshAgent navMeshAgent;

    private CinemachineCamera camera;


    public GameLogic.CharacterType characterType;
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
            Debug.Log("AAA" +PlayerScript.LocalInstance.playerIndex);
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
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
    }


    protected async Task ChangeCurrentTileIndex(int diceValue, int i)
    {
        int index = 0;
        if (currentTileIndex > GameLogic.Instance.mapGenerator.GetSize() - 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 2)
        {
            index = 1;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 1 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 3)
        {
            index = 2;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 4 + 4)
        {
            index = 3;
        }


        if (currentTileIndex != ((GameLogic.Instance.mapGenerator.GetSize() - 1) * 4)-1)
        {
            currentTileIndex++;
        }
        else currentTileIndex = 0;
        
        navMeshAgent.destination = GameLogic.Instance.allTileScripts[currentTileIndex].transform.position - (GameLogic.Instance.board.transform.GetChild(index).transform.position - GameLogic.Instance.SpawnPoints[index].GetChild(playerIndex).transform.position);
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (navMeshAgent.remainingDistance < 0.1f) break;
        }

        
        if (i + 1 < diceValue && currentTileIndex==0)
        {
            GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade, true);
        }
    }


    public virtual async void Move(int diceValue)
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
