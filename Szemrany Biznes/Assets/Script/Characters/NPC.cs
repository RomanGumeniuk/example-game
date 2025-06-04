using UnityEngine;

public class NPC : Character
{
    public override void Greetings()
    {
        Debug.Log("NPC");
        name = "NPC";
        isWoman = false;
    }
}
