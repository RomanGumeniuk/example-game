using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ChancesSO", menuName = "Scriptable Objects/ChancesSO")]
public class ChancesSO : ScriptableObject
{
    public List<ChanceCard> chances;
}
[Serializable]
public class ChanceCard
{
    public string name;
    public string description;
    public int amountOfMoney;
    public SpecialType type;
    public int turnsPlayerNeedsToWait;


    public enum SpecialType
    { 
        MoveToPrison,
        MoveToStart,

    }


}

