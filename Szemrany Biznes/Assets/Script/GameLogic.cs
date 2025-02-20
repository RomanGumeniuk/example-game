using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using TMPro;


public class GameLogic : NetworkBehaviour
{
    public static GameLogic Instance { get; private set; }
    public MapGeneratorSO mapGenerator;
    public GameObject board;

    public List<Transform> SpawnPoints = new List<Transform>();
    public List<NetworkClient> PlayersOrder = new List<NetworkClient>();

    public ChancesSO chancesSO;

    public List<TileScript> allTileScripts = new List<TileScript>();

    public List<Transform> allPlayersListPrefab = new List<Transform>();

    public List<string> chanceList = new List<string>();

    public int index = -1;
    public int allPlayerAmount = 0;

    public Transform content;
    public GameObject playerListPrefab;

    public List<Material> PlayerColors;

    public List<Character> allCharacters = new List<Character>();

    public bool isDoublet = false;

    public GameObject deadDropBoxPrefab;

    public ItemDataBaseSO itemDataBase;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetSpawnPointFromServerRpc(NetworkManager.Singleton.LocalClientId);
        if (GameUIScript.OnStartGame == null)
            GameUIScript.OnStartGame = new UnityEvent();

        GameUIScript.OnStartGame.AddListener(OnGameStarted);

        if (GameUIScript.OnNextPlayerTurn == null)
            GameUIScript.OnNextPlayerTurn = new UnityEvent();

        GameUIScript.OnNextPlayerTurn.AddListener(OnTryCallingNextPlayerTurnServerRpc);
        allCharacters.Add(new ThickWoman());
        allCharacters.Add(new Homeless());
        allCharacters.Add(new NPC());
        allCharacters.Add(new BrothelKeeper());
        allCharacters.Add(new Jew());
        allCharacters.Add(new Seba());
        allCharacters.Add(new Jamal());
        allCharacters.Add(new Student());
    }


    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        allTileScripts = GetAllTileScriptFromBoard();
        itemDataBase.GenerateItems();
        
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
        
        GiveClientSpawnPositionClientRpc(SpawnPoints[0].GetChild((int)playerId).transform.position, (int)playerId, clientRpcParams);
        GiveAllPlayersTherisIndexClientRpc((int)playerId);
    }


    

    


    [ClientRpc]
    public void GiveClientSpawnPositionClientRpc(Vector3 spawnPostion,int playerIndex,  ClientRpcParams clientRpcParams = default)
    {
        PlayerScript.LocalInstance.GoTo(spawnPostion);
    }
    [ClientRpc]
    public void GiveAllPlayersTherisIndexClientRpc(int clientIndex)
    {
        NetworkManager.Singleton.ConnectedClientsList[clientIndex].PlayerObject.GetComponent<PlayerScript>().playerIndex = clientIndex;
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
            
            //Debug.Log(PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>());//.SetMaterialClientRpc();
            PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().SetMaterialClientRpc();
            //PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value = 10000;
            //PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().totalAmountOfMoney.Value = 10000;
        }
        List<int> charatersIds = new List<int>();
        for (int i = 0; i < allCharacters.Count; i++) charatersIds.Add(i);
        IListExtensions.Shuffle(charatersIds);
        for (int i = 0; i < PlayersOrder.Count; i++)
        {
            
            if (allCharacters[charatersIds[i]].GetType() == typeof(Student) && i+1 < PlayersOrder.Count)
            {
                OnStudentSwapPlaces((int)PlayersOrder[i].ClientId, i);
            }
            SetAllCharactersClientRpc(charatersIds[i], (int)PlayersOrder[i].ClientId,i);
        }
        AddAllPlayerPrefabList();
        
    }

    [ClientRpc]
    public void SetAllCharactersClientRpc(int i, int playerIndex,int playerOrderIndex)
    {
        PlayerScript player = NetworkManager.Singleton.ConnectedClientsList[playerIndex].PlayerObject.GetComponent<PlayerScript>();
        player.character = allCharacters[i];
        player.character.Greetings();
        player.character.playerScript = player;
        player.character.OnCharacterCreated();
        
    }

    private async void OnStudentSwapPlaces(int playerIndex,int playerOrderIndex)
    {
        await Awaitable.WaitForSecondsAsync(0.05f);
        Debug.Log("StudentSwapp" + playerIndex + " " + playerOrderIndex + " " + PlayersOrder.Count) ;
        PlayersOrder.RemoveAt(playerOrderIndex);
        PlayersOrder.Add(NetworkManager.Singleton.ConnectedClients.GetValueOrDefault<ulong, NetworkClient>((ulong)playerIndex));
    }

    private async void AddAllPlayerPrefabList()
    {
        await Awaitable.WaitForSecondsAsync(0.08f);
        for (int i = 0; i < PlayersOrder.Count; i++)
        {
            AddAllPlayerPrefabListClientRpc(PlayersOrder[i].ClientId, PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value, PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().character.GetName());
            PlayersOrder[i].PlayerObject.GetComponent<PlayerScript>().character.Greetings();
        }
        OnNextPlayerTurnServerRpc();
    }


    [ClientRpc]
    public void AddAllPlayerPrefabListClientRpc(ulong index,int startMoney,string characterType)
    {
        GameObject playerPrefabList = Instantiate(playerListPrefab, content);
        allPlayersListPrefab.Add(playerPrefabList.transform);


        if (PlayerScript.LocalInstance.playerIndex == (int)index) playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = characterType + "#" + index + " You";
        else
        {
            playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = characterType + "#" + index;
        }
        playerPrefabList.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = PlayerColors[(int)index].color;
        playerPrefabList.transform.Find("Money").GetComponent<TextMeshProUGUI>().text = startMoney +"PLN";
        content.parent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(609, -123);
        content.parent.parent.parent.gameObject.SetActive(true);
    }

    

    

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMoneyForPlayerServerRpc(int newMoney,int playerIndex,int type =0,bool hide = true,bool changeTotalAmountOfMoney = false) // type 0-> set 1-> subtract 2->add
    {
        PlayerScript player = NetworkManager.Singleton.ConnectedClients[(ulong)playerIndex].PlayerObject.GetComponent<PlayerScript>();
        switch (type)
        {
            case 0:
                player.amountOfMoney.Value = newMoney;
                if(changeTotalAmountOfMoney) player.totalAmountOfMoney.Value = newMoney;
                playerIndex = -1;
                break;
            case 1:
                player.amountOfMoney.Value -= newMoney;
                if (changeTotalAmountOfMoney) player.totalAmountOfMoney.Value -= newMoney;
                newMoney = newMoney * (-1);
                break;
            case 2:
                player.amountOfMoney.Value += newMoney;
                if (changeTotalAmountOfMoney) player.totalAmountOfMoney.Value += newMoney;
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
    [ServerRpc(RequireOwnership =false)]
    public void SetIsDoubletServerRPC(bool isDoublet)
    {
        this.isDoublet = isDoublet;
    }




    public int amountOfBlockersForNextPlayerTurn = 0; // 0 - can call next player turn, not 0 - can't call next player turn until value is not 0
    [ServerRpc(RequireOwnership = false)]
    public void IncreasCallNextPlayerTurnServerRpc()
    {
        amountOfBlockersForNextPlayerTurn++;
        Debug.Log("increase");
    }
    [ServerRpc(RequireOwnership = false)]
    public void DecreaseCallNextPlayerTurnServerRpc()
    {
        if (amountOfBlockersForNextPlayerTurn == 0) return;
        amountOfBlockersForNextPlayerTurn--;
        Debug.Log("decrease");
    }

    bool isBusy = false;
    [ServerRpc(RequireOwnership = false)]
    public void OnTryCallingNextPlayerTurnServerRpc()
    {
        if (isBusy) return;
        WaitForPermissionToCallNextPlayer();
    }


    async void WaitForPermissionToCallNextPlayer()
    {
        isBusy = true;
        while (amountOfBlockersForNextPlayerTurn > 0)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
        await Awaitable.WaitForSecondsAsync(0.1f);
        if(amountOfBlockersForNextPlayerTurn >0)
        {
            WaitForPermissionToCallNextPlayer();
            return;
        }
        OnNextPlayerTurnServerRpc();
        isBusy = false;
    }



    [ServerRpc(RequireOwnership = false)]
    public void OnNextPlayerTurnServerRpc()
    {
        //Debug.Log("on next player invoke");
        ClientRpcParams clientRpcParams;
        if (index != -1)
        {
            clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { PlayersOrder[index].ClientId }
                }
            };
            PlayerScript.LocalInstance.OnPlayerTurnEndClientRpc(clientRpcParams);
        }
        
        
        if (!isDoublet)
        {
            if (allPlayerAmount == index + 1)
            {
                index = 0;
            }
            else index++;
        }
        else isDoublet = false;

        clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { PlayersOrder[index].ClientId }
            }
        };
        //Debug.Log("player id: "+PlayersOrder[index].ClientId);
        if (PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().wasBetrayed)
        {
            //Debug.Log("betray");
            AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Inny gracz ciê podkapowa³, nie mo¿esz nic zrobiæ w tej turze", 2.5f, (int)PlayersOrder[index].ClientId);
            PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().SetWasBetrayedServerRpc(false);
            OnNextPlayerTurnServerRpc();
            return;
        }

        if(PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().isInPrison.Value && PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value > 0)
        {
            PrisonTabUI.Instance.Show((int)PlayersOrder[index].ClientId);
            //Debug.Log("Prison");
            return;
        }

        if(PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value>0)
        {
            //Debug.Log("Cant move");
            Debug.Log(PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value + " " + index);
            PlayersOrder[index].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value--;
            OnNextPlayerTurnServerRpc();
            return;
        }

        
        ClientHasPermissionToRollDiceClientRpc(clientRpcParams);
        //Debug.Log("end");


    }

    public List<TileScript> GetAllTileScriptFromBoard()
    {
        List<TileScript> tileScripts = new List<TileScript>();
        tileScripts.Add(board.transform.GetChild(0).GetComponent<TileScript>());
        int index = 4;
        for (int i = 0; i < mapGenerator.GetSize() - 2; i++)
        {
            tileScripts.Add(board.transform.GetChild(index).GetComponent<TileScript>());
            index++;
        }
        tileScripts.Add(board.transform.GetChild(1).GetComponent<TileScript>());
        for (int i = 0; i < mapGenerator.GetSize() - 2; i++)
        {
            tileScripts.Add(board.transform.GetChild(index).GetComponent<TileScript>());
            index++;
        }
        tileScripts.Add(board.transform.GetChild(2).GetComponent<TileScript>());
        for (int i = 0; i < mapGenerator.GetSize() - 2; i++)
        {
            tileScripts.Add(board.transform.GetChild(index).GetComponent<TileScript>());
            index++;
        }
        tileScripts.Add(board.transform.GetChild(3).GetComponent<TileScript>());
        for (int i = 0; i < mapGenerator.GetSize() - 2; i++)
        {
            tileScripts.Add(board.transform.GetChild(index).GetComponent<TileScript>());
            index++;
        }
        return tileScripts;
    }

    public static int GetRealTileIndexFromAllTiles(int tileIndex)
    {
        for(int i=0; i<Instance.allTileScripts.Count;i++)
        {
            if (Instance.allTileScripts[i].index != tileIndex) continue;
            return i;
        }
        return -1;
    }

    [ServerRpc(RequireOwnership =false)]
    public void SpawnDeadDropBoxServerRpc(int tileIndex,int amountOfMoney)
    {
        GameObject deadBox = Instantiate(deadDropBoxPrefab, allTileScripts[tileIndex].transform.position + new Vector3(0,10,0), Quaternion.identity);
        deadBox.GetComponent<NetworkObject>().Spawn();
        deadBox.transform.parent = allTileScripts[tileIndex].transform;
        deadBox.GetComponent<DeadDropBox>().SetAmountOfMoney(amountOfMoney);
        deadBox.GetComponent<DeadDropBox>().index = tileIndex;
    }    
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
