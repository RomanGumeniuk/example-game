using UnityEngine;

public class ThickWoman : PlayerScript
{
    public override async void Move(int diceValue)
    {
        diceValue--;
        navMeshAgent.isStopped = false;
        for (int i = 0; i < diceValue; i++)
        {
            await ChangeCurrentTileIndex(diceValue, i);
        }
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter(currentAvailableTownUpgrade);
        navMeshAgent.isStopped = true;
    }
}
