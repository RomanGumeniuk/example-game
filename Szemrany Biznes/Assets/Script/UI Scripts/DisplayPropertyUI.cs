using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DisplayPropertyUI : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI townCostToPay;
    public TextMeshProUGUI townName;
    public Image backgroundImage;
    public GameObject backgroundOfToggle;
    public TileScript tileScript;
    public Button townButton;
    public Image lableImage;


    private void Awake()
    {
        townButton = GetComponentInChildren<Button>();
    }


    private void Start()
    {
        tileScript = transform.parent.GetComponent<TileScript>();
        toggle.onValueChanged.AddListener((value) =>
        {
            ChooseTownToDestroyTabUI.Instance.UpdateSelectedTile(tileScript, value);
            SellingTabUI.Instance.UpdateSelectedTileList(tileScript, value);
            if (value)
            {
                backgroundImage.color = Color.green;
                return;
            }
            backgroundImage.color = Color.red;
            
        });

        townButton.onClick.AddListener(() =>
        {
            tileScript.RepairTownServerRpc(tileScript.GetRepairCost(), tileScript.ownerId.Value);
            HideButton();
        });

        townButton.gameObject.SetActive(false);
        
    }

    public void UpdateBuilding(int propertyTier)
    {
        foreach(Transform child in toggle.transform)
        {
            if (child.name.StartsWith("Level"))
            {
                Destroy(child.gameObject);
                break;
            }
        }
        if (propertyTier == -1) return;
        Instantiate(GameLogic.Instance.propertyTiers[propertyTier],toggle.transform);
    }

    public void SetUpDisplay(string name, Material material,int cost)
    {
        if(name.Contains(" - "))
        {
            townName.text = name.Split(" - ")[0];
        }
        else townName.text = name;
        lableImage.color = material.color;
        townCostToPay.text = cost.ToString();
    }


    public void ShowButton(string text)
    {
        townButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        townButton.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        townButton.gameObject.SetActive(false);
    }



    public bool ReturnToggleValue()
    {
        return toggle.isOn;
    }


    public void ShowSellingView(int totalPropertyValue)
    {
        backgroundImage.color = Color.red;
        backgroundImage.gameObject.SetActive(true);
        backgroundOfToggle.SetActive(true);
        toggle.enabled = true;
        toggle.isOn = false;
        townCostToPay.text = totalPropertyValue + "PLN";
    }

    public void ShowNormalView(int ownerId, int townLevel, int townCostToPay,bool onlyChangeText = false)
    {
        if(ownerId==-1)
        {
            if (onlyChangeText)
            {
                this.townCostToPay.text = "";
                return;
            }
            backgroundImage.gameObject.SetActive(false);
            backgroundOfToggle.SetActive(false);
            toggle.enabled = false;
            this.townCostToPay.text = "";
            return;
        }
        if (onlyChangeText)
        {
            this.townCostToPay.text = townCostToPay + "PLN";
            return;
        }
        backgroundImage.gameObject.SetActive(false);
        backgroundOfToggle.SetActive(false);
        toggle.enabled = false;
        this.townCostToPay.text = townCostToPay + "PLN";
    }


}
