using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class PlayerScript : NetworkBehaviour
{
    public int currentTileIndex = 0;
    public int playerIndex;
    public NetworkVariable<int> amountOfMoney = new NetworkVariable<int>(10000);
    [SerializeField]
    private List<TileScript> tilesThatPlayerOwnList = new List<TileScript>();
    public NetworkVariable<int> totalAmountOfMoney = new NetworkVariable<int>(10000);
    public byte currentAvailableTownUpgrade = 2;
    public byte minTownLevel=1;
    public NetworkVariable<int> cantMoveFor = new NetworkVariable<int>(0);

    public NetworkVariable<bool> isInPrison = new NetworkVariable<bool>(false);

    public static PlayerScript LocalInstance;

    public NavMeshAgent navMeshAgent;

    private CinemachineCamera camera;

    public Character character;

    public Tile tileType;

    public bool wasBetrayed = false;

    public List<Item> inventory = new List<Item>();

    public PlayerDrugsSystem playerDrugsSystem;

    public BlessingSystem playerBlessingSystem;

    public bool isBancroupt=false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            navMeshAgent = GetComponent<NavMeshAgent>();
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

        if(diceValue <0)
        {
            if (currentTileIndex != 0) currentTileIndex--;
            else currentTileIndex = 51;
        }
        else
        {
            if (currentTileIndex != ((GameLogic.Instance.mapGenerator.GetSize() - 1) * 4) - 1) currentTileIndex++;
            else currentTileIndex = 0;
        }


        //Debug.Log("Set Tile");
        SetCurrentTileIndexServerRpc(currentTileIndex,playerIndex);

        navMeshAgent.destination = GameLogic.Instance.allTileScripts[currentTileIndex].transform.position - (GameLogic.Instance.allTileScripts[index].transform.position - GameLogic.Instance.SpawnPoints[index/ (GameLogic.Instance.mapGenerator.GetSize() - 1)].GetChild(playerIndex).transform.position);
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (navMeshAgent.remainingDistance < 0.1f) break;
        }

        
        if (i + 1 < Mathf.Abs(diceValue))
        {
            GameLogic.Instance.allTileScripts[currentTileIndex].specialTileScript?.OnPlayerPassBy(diceValue);
            character.BeforeOnPlayerPassBy(GameLogic.Instance.allTileScripts[currentTileIndex]);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetCurrentTileIndexServerRpc(int newCurrentTileIndex, int playerIndex)
    {
        SetCurrentTileIndexClientRpc(newCurrentTileIndex,playerIndex);
    }

    [ClientRpc]
    public void SetCurrentTileIndexClientRpc(int newCurrentTileIndex,int playerIndex)
    {
        //Debug.Log(playerIndex + " " + this.playerIndex);
        if (playerIndex == LocalInstance.playerIndex) return;
        //Debug.Log("Tile index set");
        currentTileIndex = newCurrentTileIndex;
    }





    [ClientRpc]
    public void OnDiceNumberReturnClientRpc(int diceValue, bool movePlayer = true,bool isMultiplePlayersThrow=false, ClientRpcParams clientRpcParams = default)
    {
        diceValue = LocalInstance.character.OnDiceRolled(diceValue);
        GameUIScript.Instance.OnDiceNumberReturn(diceValue, isMultiplePlayersThrow);
        if(movePlayer)LocalInstance.Move(diceValue);
        else LocalInstance.OnDiceRollValueRecived(diceValue);
    }


    public async void Move(int diceValue)
    {
        GameLogic.Instance.IncreasCallNextPlayerTurnServerRpc();
        navMeshAgent.isStopped = false;
        for (int i=0;i<Mathf.Abs(diceValue);i++)
        {
            await ChangeCurrentTileIndex(diceValue,i);
        }
        GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade);
        navMeshAgent.isStopped = true;
        
    }

    public void ShowSellingTab(int amountOfMoneyThatNeedsToBePaid,int playerIndexThatGetsPaid)
    {
        if(amountOfMoneyThatNeedsToBePaid>totalAmountOfMoney.Value)
        {
            BancruptServerRpc();
            _ = AlertTabForPlayerUI.Instance.ShowTab("Zbankrutowa³eœ!", 2);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(totalAmountOfMoney.Value, playerIndexThatGetsPaid, 2, true, true);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(0, playerIndex, 0, true, true);
            return;
        }
        SellingTabUI.Instance.Show(amountOfMoneyThatNeedsToBePaid, amountOfMoney.Value,playerIndexThatGetsPaid);
        foreach (TileScript tile in tilesThatPlayerOwnList)
        {
            tile.ShowSellingView();
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void BancruptServerRpc()
    {
        BuncruptClientRpc();
    }


    [ClientRpc]
    private void BuncruptClientRpc()
    {
        isBancroupt = true;
    }

    public void ShowTownDamageTab()
    {
        bool areTherAnyTownsToDestroy = false;
        foreach (TileScript tile in GameLogic.Instance.allTileScripts)
        {
            if (tile.ownerId.Value == -1 || tile.ownerId.Value == playerIndex || tile.destroyPercentage.Value > 0) continue;
            tile.ShowSellingView();
            areTherAnyTownsToDestroy = true;
        }
        if(!areTherAnyTownsToDestroy)
        {
            GameUIScript.OnNextPlayerTurn.Invoke();
            return;
        }
        ChooseTownToDestroyTabUI.Instance.Show();
        
    }
    [ServerRpc(RequireOwnership =false)]
    public void ChangeCantMoveValueServerRpc(int value)
    {
        Debug.Log("Cant move changed by: "+value);
        cantMoveFor.Value += value;
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
        for(int i=0;i<tilesThatPlayerOwnList.Count;i++)
        {
            if (tilesThatPlayerOwnList[i].index == tileIndex)
            {
                tilesThatPlayerOwnList.RemoveAt(i);
                return;
            }
        }
        //tilesThatPlayerOwnList.Remove(GameLogic.Instance.board.transform.GetChild(tileIndex).GetComponent<TileScript>());
    }

    public List<TileScript> GetTilesThatPlayerOwnList()
    {
        return tilesThatPlayerOwnList;
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetIsInPrisonServerRpc(bool value)
    { 
        isInPrison.Value = value;
    }


    public void OnDiceRollValueRecived(int diceRollValue)
    {
        
        PrisonTabUI.Instance.OnDiceValueReturned(diceRollValue);
            
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerCantMoveVariableServerRpc(int value, int playerId)
    {
        NetworkManager.Singleton.ConnectedClientsList[playerId].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value = value;
    }
    [ServerRpc(RequireOwnership =false)]
    public void SetWasBetrayedServerRpc(bool value)
    {
        SetWasBetrayedClientRpc(value);
    }
    [ClientRpc]
    private void SetWasBetrayedClientRpc(bool value)
    {
        wasBetrayed = value;
    }

    public void TeleportToTile(int pickedIndex)
    {
        Debug.Log(IsOwner+" Tu jestem " + navMeshAgent + " " + playerIndex);
        currentTileIndex = pickedIndex;
        int index = 0;
        if (currentTileIndex > GameLogic.Instance.mapGenerator.GetSize() - 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 2)
        {
            index = GameLogic.Instance.mapGenerator.GetSize() - 1;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 2 + 1 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 3)
        {
            index = (GameLogic.Instance.mapGenerator.GetSize() - 1) * 2;
        }
        else if (currentTileIndex > (GameLogic.Instance.mapGenerator.GetSize() - 2) * 3 + 2 && currentTileIndex < (GameLogic.Instance.mapGenerator.GetSize() - 2) * 4 + 4)
        {
            index = (GameLogic.Instance.mapGenerator.GetSize() - 1) * 3;
        }
        navMeshAgent.enabled = false;
        transform.position = GameLogic.Instance.allTileScripts[currentTileIndex].transform.position - (GameLogic.Instance.allTileScripts[index].transform.position - GameLogic.Instance.SpawnPoints[index / (GameLogic.Instance.mapGenerator.GetSize() - 1)].GetChild(playerIndex).transform.position);
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade);
        navMeshAgent.enabled = true;
    }
    [ClientRpc]
    public void TeleportToTileClientRpc(int pickedIndex,ClientRpcParams clientRpcParams = default)
    {
        LocalInstance.TeleportToTile(pickedIndex);
    }

    public void AddItemToInventory(Item item)
    {
        item.playerScriptThatOwnsItem = this;
        inventory.Add(item);
    }

    [ClientRpc]
    public void AddItemToInventoryClientRpc(int itemID,ClientRpcParams clientRpcParams = default)
    {
        LocalInstance.AddItemToInventory(GameLogic.Instance.itemDataBase.allItems[itemID]);
        Debug.Log("AAA " +  itemID);
    }

    Queue<IQueueWindows> queueOfWindowActions = new Queue<IQueueWindows>();

    bool goToNextActionWindow = true;

    public void AddToQueueOfWindows(IQueueWindows script)
    {
        
        queueOfWindowActions.Enqueue(script);
        if (queueOfWindowActions.Count == 1)
        {
            GoThroughQueueOfWindowActions();
        }
        GameLogic.Instance.IncreasCallNextPlayerTurnServerRpc();
    }


    public void GoToNextAction()
    {
        goToNextActionWindow = true;
        GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
    }

    async void GoThroughQueueOfWindowActions()
    {
        while(queueOfWindowActions.Count > 0)
        {
            if (!goToNextActionWindow)
            {
                await Awaitable.WaitForSecondsAsync(0.1f);
                continue;
            }
            queueOfWindowActions.Dequeue().ResumeAction();
            goToNextActionWindow = false;
        }
    }

    [ClientRpc]
    public void OnPlayerTurnEndClientRpc(ClientRpcParams clientRpcParams = default)
    {
        playerDrugsSystem.OnPlayerTurnEnded();
        playerBlessingSystem.OnPlayerTurnEnd();
    }

    [ClientRpc]
    public void ClearAllIllegalItemsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        inventory.RemoveAll(inventory => inventory.isIllegal);
    }

}
