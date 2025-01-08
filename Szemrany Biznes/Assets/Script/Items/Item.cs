using UnityEngine;

public abstract class Item
{
    public string name;
    public int amountOfUses;
    public PlayerScript playerScriptThatOwnsItem;


    public virtual void OnItemUse()
    {

    }
}
