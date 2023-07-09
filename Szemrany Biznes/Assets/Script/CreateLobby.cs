using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameField; 
    [SerializeField] private Toggle isPrivateToggle;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Button createLobbyButton;

    public void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            string lobbyName = lobbyNameField.text;
            bool isPrivate = isPrivateToggle.isOn;
            float maxPlayers = maxPlayersSlider.value;

            if (lobbyName != null)
            {                
                GameLobby.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);
                SceneLoader.Instance.GoToScene("Lobby");
            }
            else
            {

            }
        });
    }
}
