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
                    Alcohol newAlcohol = new Alcohol(itemCreations[i].name, itemCreations[i].description, itemCreations[i].amountOfUses, itemCreations[i].cost,itemCreations[i].icon, itemCreations[i].itemType, itemCreations[i].itemTier);
                    Debug.Log(newAlcohol.itemTier + " " + itemCreations[i].itemTier + " " + itemCreations[i].name);
                    allItems.Add(newAlcohol);
                    break;
            
            }

        }
    }



    public List<Item> GetRandomNumberOfItems(int numberOfItems, ItemType[] itemTypes = null, ItemTier[] itemTiers = null)
    {
        List<Item> allItemsCopy = GetCopyOfAllItems(itemTypes,itemTiers);
        List<Item> pickedItems = new List<Item>();
        Debug.Log(allItemsCopy.Count + " copy items");
        for(int i=0;i< allItemsCopy.Count;i++)
        {
            Debug.Log(allItems[i].itemTier);
            Debug.Log(allItemsCopy[i].itemTier);
        }
        for (int i = 0; i < numberOfItems; i++)
        {
            
            int pickedItem = UnityEngine.Random.Range(0,allItemsCopy.Count);
            Debug.Log(pickedItem + " picked item" );
            pickedItems.Add(allItemsCopy[pickedItem]);
            allItemsCopy.RemoveAt(pickedItem);
        }
        return pickedItems;
    }

    private List<Item> GetCopyOfAllItems(ItemType[] itemTypes = null, ItemTier[] itemTiers = null)
    {
        if(itemTypes == null) return new List<Item>(allItems);
        List<Item> copyList = new List<Item>();
        for (int i = 0; i < allItems.Count; i++)
        {
            if (itemTypes!=null&&!itemTypes.Contains(allItems[i].itemType)) continue;
            Debug.Log(itemTiers[0] + " " + allItems[i].itemTier);
            if (itemTiers != null && !itemTiers.Contains(allItems[i].itemTier)) continue;
            Debug.Log(allItems[i].GetName() + " " + allItems[i].itemTier);
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
    public ItemTier itemTier;

    public int value1;
    public int value2;
    public float value3;


}

public enum ItemType
{ 
    Alcohol,
    Drug,



}


public enum ItemTier
{
    Junk,
    Normal,
    Decent,
    Exclusive,
    Relic

}

