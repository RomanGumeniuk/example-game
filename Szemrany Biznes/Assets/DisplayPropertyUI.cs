using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayPropertyUI : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI townCostToPay;
    public TextMeshProUGUI ownerId;
    public TextMeshProUGUI townLevel;
    public Image backgroundImage;
    public GameObject backgroundOfToggle;
    public TileScript tileScript;

    private void Start()
    {
        tileScript = transform.parent.GetComponent<TileScript>();
        toggle.onValueChanged.AddListener((value) =>
        {
            SellingTabUI.Instance.UpdateSelectedTileList(tileScript, value);
            if (value)
            {
                backgroundImage.color = Color.green;
                return;
            }
            backgroundImage.color = Color.red;
            
        });
    }

    public bool ReturnToggleValue()
    {
        return toggle.isOn;
    }


    public void ShowSellingView(int totalPropertyValue)
    {
        backgroundImage.gameObject.SetActive(true);
        backgroundOfToggle.SetActive(true);
        toggle.enabled = true;
        toggle.isOn = false;
        townCostToPay.text = totalPropertyValue + "PLN";
    }

    public void ShowNormalView(int ownerId, int townLevel, int townCostToPay)
    {
        if(ownerId==-1)
        backgroundImage.gameObject.SetActive(false);
        backgroundOfToggle.SetActive(false);
        toggle.enabled = false;
        this.ownerId.text = ownerId.ToString();
        this.townLevel.text = "Lvl:" + townLevel;
        this.townCostToPay.text = townCostToPay + "PLN";
    }


}
