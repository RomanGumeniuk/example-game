using UnityEngine;

public class Alcohol : Item
{
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
