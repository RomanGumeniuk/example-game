using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySearcherScript : MonoBehaviour
{
    public static LobbySearcherScript Instance;
    [SerializeField] private TMP_InputField lobbyCodeText;
    [SerializeField] private Button joinViaCodeButton;

    [SerializeField] private TMP_InputField lobbyNameText;
    [SerializeField] private Button searchByLobbyNameButton;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
        joinViaCodeButton.onClick.AddListener(() =>
        {
            string lobbyCode = lobbyCodeText.text;
            if (lobbyCode != null)
            {
                GameLobby.Instance.JoinLobbyByCode(lobbyCode);
                SceneLoader.Instance.GoToScene("Lobby");
            }
            
        });
        searchByLobbyNameButton.onClick.AddListener(() =>
        {
            string lobbyName = lobbyNameText.text;
            if (lobbyName != null)
            {
                GameLobby.Instance.ListLobbies(lobbyName);
            }
        });

    }
}
