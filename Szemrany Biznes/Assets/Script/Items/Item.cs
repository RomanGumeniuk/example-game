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
}
