using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class ShopTabUI : MonoBehaviour,IQueueWindows
{
    public static ShopTabUI Instance { private set; get; }

    public Button dontBuy;
    public Button buy;
    public List<GameObject> allOptions;

    public List<Item> avaliableItemsToBuy = new List<Item>();

    int selectedIndex;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < allOptions.Count; i++)
        {
            int optionIndex = i;
            allOptions[i].GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) =>
            {
                if (value)
                {
                    OnOptionSelected(optionIndex);
                }
            });
        }

        buy.onClick.AddListener(() =>
        {
            PlayerScript.LocalInstance.AddItemToInventory(avaliableItemsToBuy[selectedIndex]);
            int cost = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedAmountOfMoney(avaliableItemsToBuy[selectedIndex].GetCost(),TypeOfMoneyTransaction.BuyingItem);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(cost,PlayerScript.LocalInstance.playerIndex,1,true,true);
            _ =AlertTabForPlayerUI.Instance.ShowTab($"Kupi³eœ {avaliableItemsToBuy[selectedIndex].GetName()} za {avaliableItemsToBuy[selectedIndex].GetCost()}PLN \nZapraszamy ponownie!",2);
            Hide();
            OnOptionSelected(-1);
        });

        dontBuy.onClick.AddListener(() =>
        {
            GameUIScript.OnNextPlayerTurn.Invoke();
            Hide();
        });

    }

    private void OnOptionSelected(int index)
    {
        selectedIndex = index;
        for (int i = 0; i<allOptions.Count; i++)
        {
            if (i == index) continue;
            allOptions[i].GetComponentInChildren<Toggle>().isOn = false;
        }
    }



    public void Show()
    {
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    void ShowActualWindow()
    {
        avaliableItemsToBuy = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(allOptions.Count);
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].GetComponent<RawImage>().texture = avaliableItemsToBuy[i].GetIcon()?.texture;
            allOptions[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = avaliableItemsToBuy[i].GetCost().ToString() + " PLN";
            allOptions[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = avaliableItemsToBuy[i].GetName();
            if (avaliableItemsToBuy[i].GetCost() > PlayerScript.LocalInstance.amountOfMoney.Value) allOptions[i].GetComponentInChildren<Toggle>().interactable = false;
            else allOptions[i].GetComponentInChildren<Toggle>().interactable = true;
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
        ShowActualWindow();
    }
}
