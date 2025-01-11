using System;
using UnityEngine;
using UnityEngine.UI;
public abstract class Item
{
    protected string name;
    protected string description;
    protected int amountOfUses;
    protected int cost;
    public PlayerScript playerScriptThatOwnsItem;
    protected RawImage icon;


    public void BeforeOnItemUse()
    {
        OnItemUse();
        if (amountOfUses == 0)
            playerScriptThatOwnsItem.inventory.Remove(this);
    }


    public virtual void OnItemUse()
    {

    }

    public RawImage GetIcon()
    {
        return icon;
    }

    public int GetCost()
    {
        return cost;
    }
    public string GetName()
    {
        return name;
    }

    public int GetAmountOfUses()
    {
        return amountOfUses;
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetName(string name)
    {
        this.name = name;
    }
    public  void SetAmountOfUses(int amountOfUses)
    {
        this.amountOfUses = amountOfUses;
    }
    public void SetDescription(string description)
    {
        this.description = description;
    }
    public void PlayerScriptThatOwnsItem(PlayerScript playerScriptThatOwnsItem)
    {
        this.playerScriptThatOwnsItem = playerScriptThatOwnsItem;
    }
}
