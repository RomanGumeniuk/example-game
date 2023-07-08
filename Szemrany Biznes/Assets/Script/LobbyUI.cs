using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{   
    public static LobbyUI Instance { get; private set; }
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            createLobbyButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.GoToScene("MenuCreator");
            });
            joinLobbyButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.GoToScene("LobbySearching");
            });
            settingsButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.GoToScene("Settings");
            });
            creditsButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.GoToScene("Credits");
            });
            exitButton.onClick.AddListener(() =>
            {
                //Exit the game
                Application.Quit();
            });
        }
    }
}
