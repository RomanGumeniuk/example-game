using System;
using UnityEngine;
[Serializable]
public class NPC : Character
{
    public override void Greetings()
    {
        Debug.Log("NPC");
        name = "NPC";
        isWoman = false;
    }
}
