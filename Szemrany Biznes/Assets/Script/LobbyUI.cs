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

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby("LobbyName", 5 ,false);
        });
        joinLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoinLobby();
        });
    }
}
