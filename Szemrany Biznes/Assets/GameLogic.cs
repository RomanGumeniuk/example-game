using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class GameLogic : NetworkBehaviour
{
    [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();
    [SerializeField] public List<NetworkClient> PlayersOrder = new List<NetworkClient>();


    public int index = 0;
    public int allPlayerAmount = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Spawned");
        GetSpawnPointFromServerRpc(NetworkManager.Singleton.LocalClientId);
        /*PlayerScript playerObj = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<PlayerScript>();
        playerObj.GoTo(SpawnPoints[index].position);*/
        //calltoclientconectedServerRPC(NetworkManager.Singleton.LocalClientId);
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
        GiveClientSpawnPositionClientRpc( SpawnPoints[index].position, clientRpcParams);
        index++;
    }


    [ClientRpc]
    public void GiveClientSpawnPositionClientRpc(Vector3 spawnPostion,ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Client");
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
        List<NetworkClient> allPlayers = (List<NetworkClient>)NetworkManager.Singleton.ConnectedClientsList;
        for (int i=0;i< allPlayerAmount; i++)
        {
            int currentIndex = Random.Range(0, allPlayerAmount - i);
            PlayersOrder.Add(allPlayers[currentIndex]);
            allPlayers.RemoveAt(currentIndex);
        }
        index = 0;
        OnNextPlayerTurnServerRpc();
    }

    [ClientRpc]
    public void ClientHasPermissionToRollDiceClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("I have permission");
        GameUIScript.Instance.ShowUIForRollDice();
    }

    [ServerRpc(RequireOwnership =false)]
    public void OnNextPlayerTurnServerRpc()
    { 
        
        
       
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { PlayersOrder[index].ClientId }
            }
        };
        if (allPlayerAmount == index+1)
        {
            
            index = 0;
        }
        else index++;
        ClientHasPermissionToRollDiceClientRpc(clientRpcParams);
    }

    /*
    [ServerRpc(RequireOwnership = false)]
    public void calltoclientconectedServerRPC(ulong id)
    {
        Transform playerObj = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform;

        ClientConnectedClientRPC();
    }

    [ClientRpc]
    private void ClientConnectedClientRPC()
    {
        //deliver the object refrence to each client
        //player.GetComponent<PlayerScript>().GoTo(SpawnPoints[index].position);
        index++;
    }*/
}
