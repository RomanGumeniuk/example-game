using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RepairTabUI : MonoBehaviour,IQueueWindows
{
    public static RepairTabUI Instance { get; private set; }

    public Button DontBuyButton;
    public Button BuyButton;
    public TextMeshProUGUI TextLable;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI TitleScreen;

    public int currentCost = 0;

    public TileScript currentTileScript = null;

    private void Start()
    {
        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();
        });

        BuyButton.onClick.AddListener(() =>
        {
            currentTileScript.RepairTownServerRpc(currentCost, PlayerScript.LocalInstance.playerIndex);
            GameUIScript.OnNextPlayerTurn.Invoke();
            currentTileScript.displayPropertyUI?.HideButton();
            Hide();
        });
    }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }


    public void ShowRepairUI(int cost,TileScript tile)
    {
        currentTileScript = tile;
        if (currentTileScript.destroyPercentage.Value == 0) return;
        currentCost = cost;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    void ShowRepairUI()
    {
        Cost.text = currentCost.ToString() + " PLN";
        TextLable.text = currentTileScript.name;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    

    private void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        PlayerScript.LocalInstance.GoToNextAction();
    }

    public void ResumeAction()
    {
        ShowRepairUI();
    }
}
