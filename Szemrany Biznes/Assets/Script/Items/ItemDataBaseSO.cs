using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.ComponentModel;

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
                    Alcohol newAlcohol = new Alcohol(itemCreations[i].name, itemCreations[i].description, itemCreations[i].amountOfUses, itemCreations[i].cost,itemCreations[i].icon, (TypeOfAlcohol)itemCreations[i].value1);

                    allItems.Add(newAlcohol);
                    break;
            
            }

        }
    }



    public List<Item> GetRandomNumberOfItems(int numberOfItems)
    {
        List<Item> allItemsCopy = new List<Item>(allItems);
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

