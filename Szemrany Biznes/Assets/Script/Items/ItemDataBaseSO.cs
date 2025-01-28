using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.ComponentModel;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Scriptable Objects/ItemDataBase")]
public class ItemDataBaseSO : ScriptableObject
{
    [SerializeField]
    List<ItemCreation> itemCreations = new List<ItemCreation>();

    public List<Item> allItems = new List<Item>();
    public void GenerateItems()
    {
        for (int i = 0; i < itemCreations.Count; i++)
        {
            switch (itemCreations[i].itemType)
            {
                case ItemType.Alcohol:
                    Alcohol newAlcohol = new Alcohol(itemCreations[i].name, itemCreations[i].description, itemCreations[i].amountOfUses, itemCreations[i].cost,itemCreations[i].icon, itemCreations[i].itemType, (TypeOfAlcohol)itemCreations[i].value1);

                    allItems.Add(newAlcohol);
                    break;
            
            }

        }
    }



    public List<Item> GetRandomNumberOfItems(int numberOfItems, ItemType[] itemTypes = null)
    {
        List<Item> allItemsCopy = GetCopyOfAllItems(itemTypes);
        List<Item> pickedItems = new List<Item>();
        Debug.Log(allItemsCopy.Count + " copy items");
        for (int i = 0; i < numberOfItems; i++)
        {
            
            int pickedItem = UnityEngine.Random.Range(0,allItemsCopy.Count);
            Debug.Log(pickedItem + " picked item" );
            pickedItems.Add(allItemsCopy[pickedItem]);
            allItemsCopy.RemoveAt(pickedItem);
        }
        return pickedItems;
    }

    private List<Item> GetCopyOfAllItems(ItemType[] itemTypes = null)
    {
        if(itemTypes == null) return new List<Item>(allItems);
        List<Item> copyList = new List<Item>();
        for (int i = 0; i < allItems.Count; i++)
        {
            if (!itemTypes.Contains(allItems[i].itemType)) continue;
            copyList.Add(allItems[i]);
        }
        return copyList;
    }
}

[Serializable]
public class ItemCreation
{
    public string name;
    public string description;
    public int amountOfUses;
    public int cost;
    public RawImage icon;
    public ItemType itemType;

    public int value1;
    public int value2;
    public float value3;


}

public enum ItemType
{ 
    Alcohol,
    Drug,



}

