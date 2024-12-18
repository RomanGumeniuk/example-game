using Unity.Netcode;
using UnityEngine;

public class ThickWoman : Character
{
    public override int OnDiceRolled(int diceValue)
    {
        Debug.Log("Roll decresed by one");
        return diceValue-1;
    }

    public override void Greetings()
    {
        Debug.Log("ThickWoman");
        name = "Baba Grzmot";
    }

}
