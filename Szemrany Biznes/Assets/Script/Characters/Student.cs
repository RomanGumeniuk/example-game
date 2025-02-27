using Unity.Netcode;
using UnityEngine;

public class Student : Character
{
    public override void Greetings()
    {
        Debug.Log("Student");
        name = "Student";
    }
    const float START_MONEY_MULTIPLIER = 0.8f;

    


    public override void OnCharacterCreated()
    {
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(Mathf.CeilToInt(playerScript.amountOfMoney.Value * START_MONEY_MULTIPLIER), playerScript.playerIndex, 0, true, true);
    }
    const float DEAD_DROP_MULTIPLIER = 1.5f;
    public override void ClaimDeadDropBox(DeadDropBox deadDropBoxScript)
    {
        deadDropBoxScript.ChangeAmountOfMoney(DEAD_DROP_MULTIPLIER);
        base.ClaimDeadDropBox(deadDropBoxScript);
    }

    public override int ApplyAllModifiersToSpecifiedTypeOfModificator(int value, TypeOfModificator typeOfModificator, PropertyType propertyType = PropertyType.None)
    {
        return base.ApplyAllModifiersToSpecifiedTypeOfModificator(value, typeOfModificator, propertyType);
    }
}
