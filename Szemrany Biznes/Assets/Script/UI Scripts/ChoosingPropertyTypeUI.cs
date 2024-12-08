using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChoosingPropertyTypeUI : MonoBehaviour
{
    public static ChoosingPropertyTypeUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] private Button DontBuyButton;
    [SerializeField] private Button BuyButton;
    [SerializeField] private List<Toggle> AllOptions;
    public TileScript currentTileScript = null;

    private int selectedOption = -1;



    private void Start()
    {
        BuyButton.onClick.AddListener(() =>
        {
            int totalCost = 200; //here will be some script that calculates cost
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(totalCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.SetPropertyTypeServerRpc((TileScript.PropertyType)(selectedOption+1));
            currentTileScript.UpgradeTownServerRpc(1, PlayerScript.LocalInstance.playerIndex);
            Hide();
        });

        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();
        });


        for(int i=0; i<AllOptions.Count;i++)
        {
            int index = i;
            AllOptions[i].onValueChanged.AddListener((bool value) =>
            {
                if(!value)
                {
                    for (int i = 0; i < AllOptions.Count; i++)
                    {
                        if (AllOptions[i].isOn) return;
                    }
                    BuyButton.interactable = false;
                    return;
                }
                BuyButton.interactable = true;
                for (int i=0;i<AllOptions.Count;i++)
                {
                    if (i == index) continue;
                    AllOptions[i].isOn = false;
                }
                selectedOption = index;
            });
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

    public void ShowChoosingUI(TileScript currentTileScript)
    {
        this.currentTileScript = currentTileScript;
        BuyButton.interactable = false;
        foreach(Toggle toggle in AllOptions) toggle.isOn = false;
        foreach (Transform child in transform) child.gameObject.SetActive(true);
    }
}
