using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrugTabUI : MonoBehaviour,IQueueWindows
{
    public static DrugTabUI Instance { private set; get; }

    [SerializeField]
    Button exit;
    [SerializeField]
    Button take;

    List<Item> listOfPickedAlcohols = new List<Item>();



    private TileScript currentTileScript;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        exit.onClick.AddListener(() =>
        {
            Hide();
        });

        take.onClick.AddListener(() =>
        {
            List<Item> pickedDrug = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, new ItemType[] { ItemType.Drug }, new ItemTier[] { (ItemTier)currentTileScript.townLevel.Value-1 });
            PlayerScript.LocalInstance.AddItemToInventory(pickedDrug[0]);
            PlayerScript.LocalInstance.inventory[PlayerScript.LocalInstance.inventory.Count - 1].BeforeOnItemUse();
            Hide();
        });
    }

    public void Show(TileScript tile)
    {
        currentTileScript = tile;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    private void Show()
    {
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
        Show();
    }

}
