using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreatorUI : MonoBehaviour
{
    public static LobbyCreatorUI Instance { get; private set; }
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button lobbyViewButton;
    private void Awake()
    {
        if(Instance == null)
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
            SceneLoader.Instance.GoToScene("Lobby");
        });

    }
}
