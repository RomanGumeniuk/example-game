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

    public static UnityEvent OnStartGame;
    public static UnityEvent OnNextPlayerTurn;

    public void OnDiceNumberReturn(int diceNumber)
    {
        TextAboutStateOfGame.text = "You rolled " + diceNumber;
        RollDiceButton.gameObject.SetActive(false);
        PlayerScript.LocalInstance.Move(diceNumber);
    }
    public override void OnNetworkSpawn()
    {
        RollDiceButton.onClick.AddListener(() =>
        {

            DiceSpawn.Instance.RollTheDice();
            //OnNextPlayerTurn.Invoke();
        });
        if (!IsServer)return;
        
        StartGameButton.gameObject.SetActive( true);
        StartGameButton.onClick.AddListener(() =>
        {
            OnStartGame.Invoke();
            StartGameButton.gameObject.SetActive(false);
        });

        
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

    


}
