using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameUIScript : NetworkBehaviour
{
    public static GameUIScript Instance { get; private set; }


    [SerializeField] private Button RollDiceButton;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private TextMeshProUGUI TextAboutStateOfGame;

    [SerializeField] private Button GuideBookButton;

    public static UnityEvent OnStartGame;
    public static UnityEvent OnNextPlayerTurn;

    [Header("Books")]
    [SerializeField] private RawImage Circle;

    public void OnDiceNumberReturn(int diceNumber)
    {
        TextAboutStateOfGame.text = "You rolled " + diceNumber;
        //PlayerScript.LocalInstance.Move(diceNumber);
    }

    public override void OnNetworkSpawn()
    {
        RollDiceButton.onClick.AddListener(() =>
        {
            RollDiceButton.gameObject.SetActive(false);
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex,2);
        });
        if (!IsServer)return;
        
        StartGameButton.gameObject.SetActive( true);
        StartGameButton.onClick.AddListener(StartTheGameServerRpc);

        
    }
    [ServerRpc(RequireOwnership =false)]
    private void StartTheGameServerRpc()
    {
        OnStartGame.Invoke();
        StartGameButton.gameObject.SetActive(false);
        GuideBookButton.gameObject.SetActive(true);
        HideClientRpc();
    }
    [ClientRpc]
    public void HideClientRpc()
    {
        TestUIScript.Instance.gameObject.SetActive(false);
    }



    public void ShowUIForRollDice()
    { 
        RollDiceButton.gameObject.SetActive(true);
        TextAboutStateOfGame.text = "Roll Dice!!!";
        TextAboutStateOfGame.gameObject.SetActive(true);
    }


    private void Awake()
    {
        Instance = this;
    }

    public void HideButton()
    {
        RollDiceButton.gameObject.SetActive(false);
    }
    public void OnBookGuideClicked()
    {
        RollDiceButton.gameObject.SetActive(false);
        GuideBookButton.gameObject.SetActive(false);

        Circle.gameObject.SetActive(true);
    }
}
