using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyCreatorUI : MonoBehaviour
{
    public static LobbyCreatorUI Instance { get; private set; }
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button lobbyViewButton;
    [SerializeField] private Button refreshLobbyListButton;
    [SerializeField] private GameObject contentGameObject;
    [SerializeField] private GameObject prefabGameObject;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.GoToScene("MainMenu");
        });
        lobbyViewButton.onClick.AddListener(() =>
        {
            //SceneLoader.Instance.GoToScene("Lobby");
        });
        refreshLobbyListButton.onClick.AddListener(() =>
        {
            // Destroy all childs
            foreach(Transform child in contentGameObject.transform)
            {
                Destroy(child.gameObject);
            }
            foreach(Lobby lobby in GameLobby.Instance.listOfAllLobbies)
            {
                GameObject prefab = Instantiate(prefabGameObject, contentGameObject.transform);
                prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                prefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
                prefab.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    GameLobby.Instance.JoinLobbyById(lobby.Id);
                });
            }
        });

        GameLobby.Instance.ListLobbies("");
        StartCoroutine(LateStart());
    }


    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        refreshLobbyListButton.onClick.Invoke();
    }
}
