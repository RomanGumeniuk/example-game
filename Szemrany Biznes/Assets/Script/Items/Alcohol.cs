using UnityEngine;
using UnityEngine.UI;

public class Alcohol : Item
{
    public Alcohol(string name,string description,int amountOfUses,int cost,RawImage icon,TypeOfAlcohol alcoholType)
    {
        this.name = name;
        this.description = description;
        this.amountOfUses = amountOfUses;
        this.cost = cost;
        this.icon = icon;
        this.alcoholType = alcoholType;
    }



    TypeOfAlcohol alcoholType;

    public override void OnItemUse()
    {
        int randomNumber;
        
        switch (alcoholType)
        {
            case TypeOfAlcohol.Awwfull:
                randomNumber = Random.Range(0, (GameLogic.Instance.mapGenerator.GetSize() * 4) - 4);
                playerScriptThatOwnsItem.TeleportToTile(randomNumber);
                break;
        }
    }


}

public enum TypeOfAlcohol
{ 
    Awwfull,
    Normal,
    Decent,
    Good,
    Ambrossia

}
