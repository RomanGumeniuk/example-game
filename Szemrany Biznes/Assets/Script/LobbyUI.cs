using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.GoToScene("MenuCreator");
        });
        /*joinLobbyButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.GoToScene("LobbyJoiner");
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
        });*/
    }
}
