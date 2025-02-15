using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class AlcoholTabUI : MonoBehaviour,IQueueWindows
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
        Instance = this;
        tierValues.Add(1, new List<int> { 90, 9,1});
        tierValues.Add(2, new List<int> { 50, 35, 15 });
        tierValues.Add(3, new List<int> { 0, 45, 40, 15 });
        tierValues.Add(4, new List<int> { 0, 20, 40, 40 });
        tierValues.Add(5, new List<int> { 0, 0,25,45,30 });
        tierValues.Add(6, new List<int> { 0, 0, 1,29,70 });
    }

    Dictionary<int,List<int>> tierValues = new Dictionary<int, List<int>>();

    public void Start()
    {
        exit.onClick.AddListener(() =>
        {
            GameUIScript.OnNextPlayerTurn.Invoke();
            Hide();
        });
        take.onClick.AddListener(() =>
        {
            Debug.Log(listOfPickedAlcohols.Count + " / " + selectedAlcoholIndex);
            
            PlayerScript.LocalInstance.AddItemToInventory(listOfPickedAlcohols[selectedAlcoholIndex]);
            OnOptionSelected(-1);
            Hide();
        });

        getOtherDrink.onClick.AddListener(() =>
        {
            List<int> newTierValues = new List<int>();
            for(int i = 0; i < tierValues[currentTileScript.townLevel.Value].Count; i++)
            {
                if (i == (int)itemTiersPicked[0]) newTierValues.Add(0);
                else newTierValues.Add(tierValues[currentTileScript.townLevel.Value][i]);
            }
            itemTiersPicked.Clear();
            int maxRandomNumber = 0;
            for (int i = 0; i < newTierValues.Count; i++)
            {
                maxRandomNumber += newTierValues[i];
            }
            int randomNumber = Random.Range(0, maxRandomNumber);
            int currentValue = 0;
            Debug.Log(randomNumber + " " + maxRandomNumber);
            for (int i = 0; i < newTierValues.Count; i++)
            {
                currentValue += newTierValues[i];
                if (randomNumber < currentValue)
                {
                    Debug.Log("i:" + i);
                    itemTiersPicked.Add((ItemTier)i);
                    break;
                }
            }

            List<Item> temporaryListForItems = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, new ItemType[] { ItemType.Alcohol }, itemTiersPicked.ToArray());
            
            
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
            
            
            options[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[0].GetDescription();
            options[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[0].GetName();
             
            currentTileScript.Pay(PlayerScript.LocalInstance.amountOfMoney.Value, currentTileScript.specialTileScript.GetPayAmount() / 10, true, false);
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
                else
                {
                    if(selectedAlcoholIndex == optionIndex) take.interactable = false;
                }
            });
        }

    }

    private void OnOptionSelected(int index)
    {
        selectedAlcoholIndex = index;
        if(selectedAlcoholIndex !=-1) take.interactable = true;
        for (int i = 0; i < (isCurrentPlayerOwner?options.Count:0); i++)
        {
            if (i == index) continue;
            Debug.Log(i + " " +  options[i].name);
            options[i].GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void Show(bool isPlayerOwner,TileScript tile)
    {
        currentTileScript = tile;
        isCurrentPlayerOwner = isPlayerOwner;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }
    List<ItemTier> itemTiersPicked = new List<ItemTier>();

    private void Show()
    {
        int length = isCurrentPlayerOwner ? transform.childCount : transform.childCount-2;
        int optionsAmount = isCurrentPlayerOwner ? 3 : 1;
        if (isCurrentPlayerOwner)
        {
            options[0].GetComponentInChildren<Toggle>().enabled = true;
            options[0].GetComponentInChildren<Toggle>().GetComponentInChildren<Image>().enabled = true;
            getOtherDrink.GetComponent<Image>().enabled = false;
            getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
        else
        {
            options[0].GetComponentInChildren<Toggle>().isOn = true;
            selectedAlcoholIndex = 0;
            options[0].GetComponentInChildren<Toggle>().isOn = false;
            options[0].GetComponentInChildren<Toggle>().enabled = false;
            options[0].GetComponentInChildren<Toggle>().GetComponentInChildren<Image>().enabled =false;
            getOtherDrink.GetComponent<Image>().enabled = true;
            getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            if (PlayerScript.LocalInstance.amountOfMoney.Value < currentTileScript.specialTileScript.GetPayAmount() / 10)
            {
                getOtherDrink.interactable = false;
            }
            else getOtherDrink.interactable = true;

        }
        if (isCurrentPlayerOwner) take.interactable = false;
        else take.interactable = true;

        List<int> alcoholTierValues = tierValues[currentTileScript.townLevel.Value];
        int currentValue = 0;
        int randomNumber = Random.Range(0,100);
        Debug.Log(randomNumber + "random number");
        itemTiersPicked.Clear();
        for(int i=0;i<alcoholTierValues.Count;i++)
        {
            if(isCurrentPlayerOwner)
            {
                if(alcoholTierValues[i] > 0)
                {
                    itemTiersPicked.Add((ItemTier)i);
                }
                continue;
            }
            currentValue += alcoholTierValues[i];
            if (randomNumber < currentValue)
            {
                Debug.Log("i:"+i);
                itemTiersPicked.Add((ItemTier)i);
                break;
            }
        }
        Debug.Log(itemTiersPicked[0]);
        listOfPickedAlcohols = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(optionsAmount, new ItemType[] { ItemType.Alcohol },itemTiersPicked.ToArray() );
        for (int i = 0; i < optionsAmount; i++)
        {
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetDescription();
            options[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = listOfPickedAlcohols[i].GetName();
            //options[i].GetComponent<RawImage>().texture = listOfPickedAlcohols[i]?.GetIcon().texture;
        }

        getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(getOtherDrink.GetComponentInChildren<TextMeshProUGUI>().text,currentTileScript.specialTileScript.GetPayAmount()/10);
        
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
        PlayerScript.LocalInstance.GoToNextAction();
    }
    public void ResumeAction()
    {
        Show();
    }
}
