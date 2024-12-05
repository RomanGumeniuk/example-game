using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using TMPro;

public class GameLogic : NetworkBehaviour
{
    public static GameLogic Instance { get; private set; }
    
    public List<Transform> SpawnPoints = new List<Transform>();
    public List<NetworkClient> PlayersOrder = new List<NetworkClient>();

    public List<TileScript> allTileScripts = new List<TileScript>();

    public List<Transform> allPlayersListPrefab = new List<Transform>();

    public List<string> chanceList = new List<string>();

    public int index = 0;
    public int allPlayerAmount = 0;

    public Transform content;
    public GameObject playerListPrefab;

    public List<Material> PlayerColors;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetSpawnPointFromServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetSpawnPointFromServerRpc(ulong playerId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerId }
            }
        };
        GiveClientSpawnPositionClientRpc(/*SpawnPoints[index].position*/SpawnPoints[0].GetChild((int)playerId).transform.position, (int)playerId, clientRpcParams);
    }

    [ClientRpc]
    public void GiveClientSpawnPositionClientRpc(Vector3 spawnPostion,int playerIndex, ClientRpcParams clientRpcParams = default)
    {
        PlayerScript.LocalInstance.playerIndex = playerIndex;
        PlayerScript.LocalInstance.GoTo(spawnPostion);
    }
    
    void Start()
    {
        if (GameUIScript.OnStartGame == null)
            GameUIScript.OnStartGame = new UnityEvent();

        GameUIScript.OnStartGame.AddListener(OnGameStarted);

        if (GameUIScript.OnNextPlayerTurn == null)
            GameUIScript.OnNextPlayerTurn = new UnityEvent();

        GameUIScript.OnNextPlayerTurn.AddListener(OnNextPlayerTurnServerRpc);
    }
    

    void OnGameStarted()
    {
        if (!IsServer) return;
        allPlayerAmount = NetworkManager.Singleton.ConnectedClientsList.Count;
        List<ulong> allPlayerIndex = new List<ulong>();
        for(ulong i=0;i<(ulong)allPlayerAmount;i++)
        {
            allPlayerIndex.Add(i);
        }
        
        for (int i = 0; i < allPlayerAmount; i++)
        {
            int currentIndex = Random.Range(0, allPlayerAmount - i);
            PlayersOrder.Add(NetworkManager.Singleton.ConnectedClients.GetValueOrDefault<ulong, NetworkClient>(allPlayerIndex[currentIndex]));            
            allPlayerIndex.RemoveAt(currentIndex);
            AddAllPlayerPrefabListClientRpc(PlayersOrder[ i].ClientId, PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value);
            PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().SetMaterialClientRpc();
        }
        index = 0;
        
        OnNextPlayerTurnServerRpc();
    }

    [ClientRpc]
    public void AddAllPlayerPrefabListClientRpc(ulong index,int startMoney)
    {
        GameObject playerPrefabList = Instantiate(playerListPrefab, content);
        allPlayersListPrefab.Add(playerPrefabList.transform);

        if (PlayerScript.LocalInstance.playerIndex == (int)index) playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Player#" + index + " You";
        else
        {
            playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Player#" + index;
        }
        playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = PlayerColors[(int)index].color;
        playerPrefabList.transform.Find("Money").GetComponent<TextMeshProUGUI>().text = startMoney +"PLN";
        content.parent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(757, -150);
        content.parent.parent.parent.gameObject.SetActive(true);
    }

    

    

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMoneyForPlayerServerRpc(int newMoney,int playerIndex,int type =0,bool hide = true,bool changeTotalAmountOfMoney = false) // type 0-> set 1-> subtract 2->add
    {
        switch(type)
        {
            case 0:
                NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value = newMoney;
                if(changeTotalAmountOfMoney) NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().totalAmountOfMoney.Value = newMoney;
                playerIndex = -1;
                break;
            case 1:
                NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value -= newMoney;
                if (changeTotalAmountOfMoney) NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().totalAmountOfMoney.Value -= newMoney;
                newMoney = newMoney * (-1);
                break;
            case 2:
                NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value += newMoney;
                if (changeTotalAmountOfMoney) NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>().totalAmountOfMoney.Value += newMoney;
                break;
        }
        
        RefreshAllPlayerPrefbListServerRpc(playerIndex,newMoney, hide);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshAllPlayerPrefbListServerRpc(int playerIndex = -1, int moneyChanged = 0, bool hide = true)
    {
        for (int i = 0; i < allPlayersListPrefab.Count; i++)
        {
            //allPlayersListPrefab[i].GetComponentInChildren<TextMeshProUGUI>().text = PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value + "PLN";
            if (playerIndex == (int)PlayersOrder[i].ClientId)
            {
                SetUIMoneyOfPlayerClientRpc(i, PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value, moneyChanged, hide);
            }
            else SetUIMoneyOfPlayerClientRpc(i, PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value, 0, hide);

        }
    }

    [ClientRpc]
    public void SetUIMoneyOfPlayerClientRpc(int i, int money, int moneyDiff = 0, bool hide = true)
    {
        allPlayersListPrefab[i].GetComponentInChildren<TextMeshProUGUI>().text = money + "PLN";
        if (moneyDiff != 0)
        {
            allPlayersListPrefab[i].GetChild(3).GetComponent<TextMeshProUGUI>().text = moneyDiff.ToString();
            if (moneyDiff < 0) allPlayersListPrefab[i].GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.red;
            else allPlayersListPrefab[i].GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.green;
        }
        else
        {
            if (hide) allPlayersListPrefab[i].GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
        }
    }


    [ClientRpc]
    public void ClientHasPermissionToRollDiceClientRpc(ClientRpcParams clientRpcParams = default)
    {
        
        GameUIScript.Instance.ShowUIForRollDice();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnNextPlayerTurnServerRpc()
    {
        
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { PlayersOrder[index].ClientId }
            }
        };
        if(PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value>0)
        {
            PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value--;
            if (allPlayerAmount == index + 1)
            {
                index = 0;
            }
            else index++;
            OnNextPlayerTurnServerRpc();
            return;
        }

        if (allPlayerAmount == index + 1)
        {
            index = 0;
        }
        else index++;
        ClientHasPermissionToRollDiceClientRpc(clientRpcParams);
        
        
        
    }

}
