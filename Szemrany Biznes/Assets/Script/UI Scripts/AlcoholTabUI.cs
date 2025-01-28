using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class AlcoholTabUI : MonoBehaviour
{
    public static AlcoholTabUI Instance { private set; get; }

    [SerializeField]
    Button exit;
    [SerializeField]
    Button take;
    [SerializeField]
    Button getOtherDrink;

    [SerializeField]
    List<GameObject> options = new List<GameObject>();
    
    List<Item> listOfPickedAlcohols = new List<Item>();

    

    private TileScript currentTileScript;

    private int selectedAlcoholIndex =0;

    bool isCurrentPlayerOwner;
    private void Awake()
    {
        if(Instance != null) 
            Instance = this;
    }

    public void Start()
    {
        exit.onClick.AddListener(() =>
        {
            GameUIScript.OnNextPlayerTurn.Invoke();
            Hide();
        });
        take.onClick.AddListener(() =>
        {
            PlayerScript.LocalInstance.AddItemToInventory(listOfPickedAlcohols[selectedAlcoholIndex]);
            OnOptionSelected(-1);
            Hide();
        });

        getOtherDrink.onClick.AddListener(() =>
        {
            List<Item> temporaryListForItems = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(isCurrentPlayerOwner ? 3 : 2, new ItemType[] { ItemType.Alcohol });
            
            if(!isCurrentPlayerOwner)
            {
                if(temporaryListForItems[0].GetName() == listOfPickedAlcohols[0].GetName())
                {
                    listOfPickedAlcohols.Clear();
                    listOfPickedAlcohols.Add(temporaryListForItems[1]);
                }
                else
                {
                    listOfPickedAlcohols.Clear();
                    listOfPickedAlcohols.Add(temporaryListForItems[0]);
                }
            }
            else
            {
                listOfPickedAlcohols.Clear();
                listOfPickedAlcohols.AddRange(temporaryListForItems);
            }
            for (int i = 0; i < (isCurrentPlayerOwner ? 3 : 1); i++)
            {
                options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetDescription();
                options[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetName();
                options[i].GetComponent<RawImage>().texture = listOfPickedAlcohols[i]?.GetIcon().texture;
            }
        });


        for (int i = 0; i < options.Count; i++)
        {
            int optionIndex = i;
            options[i].GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) =>
            {
                if (value)
                {
                    OnOptionSelected(optionIndex);
                }
            });
        }

    }

    private void OnOptionSelected(int index)
    {
        selectedAlcoholIndex = index;
        for (int i = 0; i < options.Count; i++)
        {
            if (i == index) continue;
            options[i].GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void Show(bool isPlayerOwner, TileScript tile)
    {
        currentTileScript = tile;
        isCurrentPlayerOwner = isPlayerOwner;
        int length = isPlayerOwner ? transform.childCount : transform.childCount-2;
        int optionsAmount = isPlayerOwner ? 3 : 1;
        if (isPlayerOwner)
        {

            options[0].GetComponentInChildren<Toggle>().gameObject.SetActive(true);
        }
        else
        {
            options[0].GetComponentInChildren<Toggle>().gameObject.SetActive(false);
            selectedAlcoholIndex = 0;
        }
        listOfPickedAlcohols = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(optionsAmount, new ItemType[] { ItemType.Alcohol });
        for (int i = 0; i < optionsAmount; i++)
        {
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetDescription();
            options[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetName();
            options[i].GetComponent<RawImage>().texture = listOfPickedAlcohols[i]?.GetIcon().texture;
        }

        getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().text,tile.specialTileScript.GetPayAmount());

        for (int i = 0; i < length; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }


    }

    public void Hide()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
