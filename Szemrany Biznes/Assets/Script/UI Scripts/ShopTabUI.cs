using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopTabUI : MonoBehaviour
{
    public Button dontBuy;
    public Button buy;
    public List<GameObject> allOptions;

    public List<Item> avaliableItemsToBuy = new List<Item>();

    public int selectedIndex;

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
            PlayerScript.LocalInstance.inventory.Add(avaliableItemsToBuy[selectedIndex]);
            int cost = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedAmountOfMoney(avaliableItemsToBuy[selectedIndex].GetCost(),TypeOfMoneyTransaction.BuyingItem);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(cost,PlayerScript.LocalInstance.playerIndex,1,true,true);
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

        avaliableItemsToBuy = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(allOptions.Count);
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].GetComponent<RawImage>().texture = avaliableItemsToBuy[i].GetIcon().texture;
            allOptions[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = avaliableItemsToBuy[i].GetCost().ToString() + " PLN";
            allOptions[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = avaliableItemsToBuy[i].GetName();
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
    }
}
