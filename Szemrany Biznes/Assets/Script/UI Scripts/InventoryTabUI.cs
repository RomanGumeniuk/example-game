using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabUI : MonoBehaviour
{
    [SerializeField]
    GameObject content;
    [SerializeField]
    Button close;
    [SerializeField]
    GameObject inventoryItemPrefab;

    bool isShown = false;

    private void Start()
    {
        close.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (isShown) Hide();
            else Show();
        }
    }

    public void Show()
    {
        isShown = true;
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        for(int i=0;i<PlayerScript.LocalInstance.inventory.Count;i++)
        {
            GameObject itemPrefab = Instantiate(inventoryItemPrefab, content.transform);
            int index = i;
            itemPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerScript.LocalInstance.inventory[index].GetName();
            itemPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = PlayerScript.LocalInstance.inventory[index].GetDescription();
            if (GameLogic.Instance.index == PlayerScript.LocalInstance.playerIndex) itemPrefab.GetComponentInChildren<Button>().interactable = false;
            else
            {
                itemPrefab.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    PlayerScript.LocalInstance.inventory[index].BeforeOnItemUse();
                    Hide();
                });
            }
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        isShown = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
