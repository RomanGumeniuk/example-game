using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PrisonTabUI : NetworkBehaviour,IQueueWindows
{
    public static PrisonTabUI Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    [SerializeField]
    private Button option1;
    [SerializeField]
    private Button option2;
    [SerializeField] 
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI opiton1Title;
    [SerializeField]
    private TextMeshProUGUI opiton1Description;
    [SerializeField]
    private TextMeshProUGUI opiton2Title;
    [SerializeField]
    private TextMeshProUGUI opiton2Description;

    private bool option1Picked = false;
    private bool isWaitingForDiceRoll = false;
    private void Start()
    {
        option1.onClick.AddListener(() =>
        {

            option1Picked = true;
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex,2,true,false);
            isWaitingForDiceRoll = true;
            Hide();
        });

        option2.onClick.AddListener(() =>
        {
            if(PlayerScript.LocalInstance.isInPrison.Value)
            {
                PlayerScript.LocalInstance.SetIsInPrisonServerRpc(false);
                PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(0, PlayerScript.LocalInstance.playerIndex);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(DEPOSIT, PlayerScript.LocalInstance.playerIndex, 1, true, true);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Zap³aci³eœ kaucje i jesteœ ju¿ wolny!", 2f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                Hide();
                return;
            }
            option1Picked = false;
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex, 2, true, false);
            isWaitingForDiceRoll = true;
            Hide();
        });
    }

    const int MIN_DICE_VALUE_TO_HELP = 8;
    const int MIN_DICE_VALUE_TO_BETRAY = 10;

    public void OnDiceValueReturned(int diceValue)
    {
        if (!isWaitingForDiceRoll) return;
        isWaitingForDiceRoll = false;
        Debug.Log("On dice value returned: " + diceValue);
        if (PlayerScript.LocalInstance.isInPrison.Value)
        {
            if(diceValue == 3 || diceValue ==7)
            {
                PlayerScript.LocalInstance.SetIsInPrisonServerRpc(false);
                PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(0, PlayerScript.LocalInstance.playerIndex);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Uda³o ci siê uciec i jesteœ ju¿ wolny!", 2f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                return;
            }
            _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda³o ci siê uciec!", 2f);
            PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(PlayerScript.LocalInstance.cantMoveFor.Value-1, PlayerScript.LocalInstance.playerIndex);
            return;
        }
        if (option1Picked)
        {
            if (diceValue < MIN_DICE_VALUE_TO_HELP)
            {
                _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda³o ci siê uwolniæ gracza z wiêzienia!", 2.5f);
                return;
            }
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!client.PlayerObject.GetComponent<PlayerScript>().isInPrison.Value) continue;

                client.PlayerObject.GetComponent<PlayerScript>().SetIsInPrisonServerRpc(false);
                client.PlayerObject.GetComponent<PlayerScript>().UpdatePlayerCantMoveVariableServerRpc(0, (int)client.ClientId);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Uda³o ci siê uwolniæ gracza z wiêzienia!", 2.5f);
                AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Inny gracz pomóg³ ci w ucieczce z wiêzienia, jesteœ wolny", 2.5f, (int)client.ClientId);
                return;
            }
            Debug.LogWarning("Nie znaleziono gracza w wiêzieniu");
            GameUIScript.OnNextPlayerTurn.Invoke();
            return;
        }

        if (diceValue < MIN_DICE_VALUE_TO_BETRAY)
        {
            _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda³o ci siê podkapowaæ gracza w wiêzieniu!", 2.5f);
            return;
        }
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerScript>().isInPrison.Value) continue;
            client.PlayerObject.GetComponent<PlayerScript>().SetWasBetrayedServerRpc(true);
            _ = AlertTabForPlayerUI.Instance.ShowTab("Uda³o ci siê odkapowaæ gracza w wiêzieniu!", 2.5f);
            AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Inny gracz podkapowa³ ciê, w nastêpnej turze nie bêdziesz móg³ nic zrobiæ", 2.5f, (int)client.ClientId);
            return;
        }
        Debug.LogWarning("Nie znaleziono gracza w wiêzieniu");
        GameUIScript.OnNextPlayerTurn.Invoke();
        return;

    }
    const int DEPOSIT = 1500;
    
    

    public void Show(int playerIndex=-1)
    {
        Debug.Log("a");
        if(playerIndex == -1) playerIndex = PlayerScript.LocalInstance.playerIndex;
        ShowServerRpc(playerIndex); 
    }


    [ServerRpc(RequireOwnership =false)]
    private void ShowServerRpc(int playerIndex)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        Debug.Log("b " + playerIndex);
        ShowClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void ShowClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //Debug.Log("c");
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    void Show()
    {
        option2.interactable = true;
        if (PlayerScript.LocalInstance.isInPrison.Value)
        {
            title.text = "Wybierz co chesz zrobiæ?";
            opiton1Title.text = "Spróbuj uciec";
            opiton1Description.text = "Wyrzuæ sume 3 lub 7 aby uciec";
            option1.GetComponentInChildren<TextMeshProUGUI>().text = "Ucieczka";
            if (PlayerScript.LocalInstance.amountOfMoney.Value < DEPOSIT) option2.interactable = false;
            opiton2Title.text = "Zap³aæ kaucje";
            opiton2Description.text = "Zap³aæ kaucje w wysokoœci " + DEPOSIT + " PLN";
            option2.GetComponentInChildren<TextMeshProUGUI>().text = "Zap³aæ";
        }
        else
        {
            title.text = "Odwiedzi³eœ odsiaduj¹cego gracza w wiêzieniu";
            opiton1Title.text = "Pomó¿ w ucieczce";
            opiton1Description.text = "Wyrzuæ co najmniej 8 aby uwolniæ gracza";
            option1.GetComponentInChildren<TextMeshProUGUI>().text = "Pomó¿";
            opiton2Title.text = "Podkapuj";
            opiton2Description.text = "Wyrzuæ co najmniej 10 aby podkapowaæ gracza";
            option2.GetComponentInChildren<TextMeshProUGUI>().text = "Podkapuj";

        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        PlayerScript.LocalInstance.GoToNextAction();
    }

    public void ResumeAction()
    {
        Show();
    }
}
