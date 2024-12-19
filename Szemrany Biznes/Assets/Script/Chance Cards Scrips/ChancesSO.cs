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
    public CardType type;
    public int value1;
    public int value2;
    public int value3;
    public float value4;
}

public enum CardType
{
    MoveToPrison,
    MoveToStart,
    GiveMoney,
    TakeMoney,
    AnotherDiceRoll

}