using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class GameLogic : NetworkBehaviour
{
    public static GameLogic Instance { get; private set; }
    // tu jest komentarz

    public List<Transform> SpawnPoints = new List<Transform>();
    public List<NetworkClient> PlayersOrder = new List<NetworkClient>();

    public int index = 0;
    public int allPlayerAmount = 0;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Spawned");
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
        Debug.Log("Server");
        GiveClientSpawnPositionClientRpc(/*SpawnPoints[index].position*/SpawnPoints[0].GetChild(index).transform.position,index, clientRpcParams);
        index++;
    }

    [ClientRpc]
    public void GiveClientSpawnPositionClientRpc(Vector3 spawnPostion,int playerIndex, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Client");
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
        for(int i=0;i<allPlayerAmount;i++)
        {
            allPlayerIndex.Add((ulong)i);
        }
        
        for (int i = 0; i < allPlayerAmount; i++)
        {
            int currentIndex = Random.Range(0, allPlayerAmount - i);
            PlayersOrder.Add(NetworkManager.Singleton.ConnectedClients.GetValueOrDefault<ulong, NetworkClient>(allPlayerIndex[currentIndex]));            allPlayerIndex.RemoveAt(currentIndex);
        }
        index = 0;
        OnNextPlayerTurnServerRpc();
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
        if (allPlayerAmount == index + 1)
        {

            index = 0;
        }
        else index++;
        ClientHasPermissionToRollDiceClientRpc(clientRpcParams);
    }
     
}
