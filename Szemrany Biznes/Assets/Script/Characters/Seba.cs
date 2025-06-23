using System;
using UnityEngine;
[Serializable]
public class Seba : Character
{
    public override void Greetings()
    {
        Debug.Log("Seba");
        name = "Seba";
        isWoman = false;
    }
}
