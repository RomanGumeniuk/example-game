using UnityEngine;

public abstract class Character
{
    public PlayerScript localPlayerScript;
    string name;
    public virtual int OnDiceRolled(int diceRoll)
    {
        //Debug.Log("Nie powinienes tego widziec");
        return diceRoll;
    }

    public virtual void Greetings()
    {
        Debug.Log("Character");
    }
}