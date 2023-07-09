using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class GameLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbiesUpdaterTimer;
    private string playerName;

    public List<Lobby> listOfAllLobbies = new List<Lobby>();

    private void Update()
    {
        HandleLobbyHeartBeat();
        HandleLobbyPollLobbiesUpdater();
    }


    private async void HandleLobbyHeartBeat() //Utrzymuje zycie lobby
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollLobbiesUpdater() //Odsieza wartosci lobby
    {
        if (joinedLobby != null)
        {
            lobbiesUpdaterTimer -= Time.deltaTime;
            if (lobbiesUpdaterTimer < 0f)
            {
                float lobbiesUpdaterTimerMax = 1.1f;
                lobbiesUpdaterTimer = lobbiesUpdaterTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }

    }

    public static GameLobby Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }


    
    public async void CreateLobby(string lobbyName, float maxPlayers, bool IsPrivate) //Tworzy lobby
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = IsPrivate,
                Player = GetPlayer(),
                /*Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Solo", DataObject.IndexOptions.S1) },
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, "Nibber") }
                }*/
            };

            Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, (int)maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Lobby has been created " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);

            PrintPlayers(hostLobby);

        }
        catch(LobbyServiceException  e)
        {
            Debug.Log(e);
        }
        
    }

    public async void ListLobbies(string lobbyNameInput) //Wypisuje wszsytkie lobby ktore spelniaja filtry
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.Name, lobbyNameInput, QueryFilter.OpOptions.CONTAINS)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            QueryLobbiesOptions QueryLobbiesOptions = queryLobbiesOptions;

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found " + queryResponse.Results.Count);
            listOfAllLobbies.Clear();
            foreach (Lobby lobby in queryResponse.Results)
            {
                //return lobby.Name lobby.MaxPlayers lobby.Id
                listOfAllLobbies.Add(lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) //Dolacza do lobby po kodzie
    {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            joinedLobby = lobby;
            Debug.Log("Joined Lobby with code " +  lobbyCode);
            PrintPlayers(joinedLobby);
        }catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
    }

    public async void JoinLobbyById(string lobbyId) //Dolacza do lobby po id
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            joinedLobby = lobby;
            Debug.Log("Joined Lobby with Id " + lobbyId);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    public async void QuickJoinLobby() //Dolacza do pierwszego znalezionego lobby
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Player GetPlayer() { //Zwraca dane graczxa
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }

    public void PrintPlayers() //Zwraca dane graczy w lobby
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby) //Zwraca dane graczy w lobby
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }



    public async void UpdateLobbyGameMode(string gameMode) //Aktualizacja trybu gry
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
            }
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void UpdatePlayerName(string newPlayerName) //Aktualizacja nazwy gracza
    {
        try {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject> {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }        
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby() //Mozliwosc opuszczenia lobby
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer() //Mozliwosc kikniecia gracza z lobby (JAKO HOST)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void MigrateHost() //Oddaje hosta nastepnemu graczu w lobby
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby() //Usuwa lobby
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
