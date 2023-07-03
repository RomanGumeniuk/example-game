using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyingTabUIScript : MonoBehaviour
{

    public static BuyingTabUIScript Instance { get; private set; }
    public Button DontBuyButton;
    public Button BuyButton;

    public List<Toggle> LevelsOfToggle = new List<Toggle>();
    
    public List<Transform> Levels = new List<Transform>();

    public List<int> currentTownCostToBuy = new List<int>();

    public TileScript currentTileScript = null;

    private void Awake()
    {
        Instance = this;

        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();

        });
        
        BuyButton.onClick.AddListener(() =>
        {
            int townLevelsAdded = GetCurrentIndexOfChoosenLevel();
            if (townLevelsAdded != -1)
            {
                PlayerScript.LocalInstance.amountOfMoney -= currentTownCostToBuy[currentTileScript.townLevel+ townLevelsAdded];
                currentTileScript.UpgradeTown(townLevelsAdded, PlayerScript.LocalInstance.playerIndex);
                Hide();
            }
        });
        
        LevelsOfToggle[0].onValueChanged.AddListener((state) =>
        {
            if(state)
            {
                //changed to true
                RefreshLevelsOnToggleOn(0);
                return;
            }
            //changed to false
            RefreshLevelsOnToggleOff(0);
        });

        LevelsOfToggle[1].onValueChanged.AddListener((state) =>
        {
            if (state)
            {
                //changed to true
                RefreshLevelsOnToggleOn(1);
                return;
            }
            //changed to false
            RefreshLevelsOnToggleOff(1);
        });

        LevelsOfToggle[2].onValueChanged.AddListener((state) =>
        {
            if (state)
            {
                //changed to true
                RefreshLevelsOnToggleOn(2);
                return;
            }
            //changed to false
            RefreshLevelsOnToggleOff(2);
        });

        LevelsOfToggle[3].onValueChanged.AddListener((state) =>
        {
            if (state)
            {
                //changed to true
                RefreshLevelsOnToggleOn(3);
                return;
            }
            //changed to false
            RefreshLevelsOnToggleOff(3);
        });


    }
    
    public void RefreshLevelsOnToggleOn(int index)
    {
        for(int i=0;i<index;i++)
        {
            if (LevelsOfToggle[i].interactable)
            {
                LevelsOfToggle[i].isOn = true;
            }
        }
    }
    
    public void RefreshLevelsOnToggleOff(int index)
    {
        for (int i = index; i < 4; i++)
        {
            if (LevelsOfToggle[i].interactable)
            {
                LevelsOfToggle[i].isOn = false;
            }
        }
    }
    
    
    public void ShowBuyingUI(int townLevel, int maxTownLevelThatCanBeBuy, List<int> townAllLevelsCost,TileScript tileScript)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        foreach(int cost in townAllLevelsCost)
        {
            currentTownCostToBuy.Add(cost);

        }
        for(int i=0;i<4;i++)
        {
            Levels[i].GetComponentInChildren<TextMeshProUGUI>().text = currentTownCostToBuy[i].ToString() + "PLN";
        }
        currentTileScript = tileScript;
        for(int i=0;i<townLevel;i++)
        {
            Levels[i].GetComponent<Image>().color = Color.gray;
            LevelsOfToggle[i].isOn = true;
            LevelsOfToggle[i].interactable = false;
        }
        for(int i=maxTownLevelThatCanBeBuy; i<4;i++)
        {
            Levels[i].GetComponent<Image>().color = Color.gray;
            LevelsOfToggle[i].interactable = false;
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

    public int GetCurrentIndexOfChoosenLevel()
    {
        int currentIndex = -1;
        for(int i=0;i<4;i++)
        {
            if (LevelsOfToggle[i].interactable)
            {
                if (LevelsOfToggle[i].isOn) currentIndex++;
            }
        }
        Debug.Log(currentIndex);
        return currentIndex;
    }

}