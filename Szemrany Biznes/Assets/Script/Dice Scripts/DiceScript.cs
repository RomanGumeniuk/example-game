using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiceScript : NetworkBehaviour
{
    [ClientRpc]
    public void OnDiceCreatedClientRpc(DiceType diceType, int playerIndex)
    {
        DiceSpawn diceSpawn = DiceSpawn.Instance;
        switch (diceType)
        {
            case DiceType.SixSide:
                if (playerIndex == 0) gameObject.GetComponent<MeshRenderer>().SetMaterials(new List<Material> { diceSpawn.sixSideDiceMaterials[0], diceSpawn.sixSideDiceMaterials[6] });
                else gameObject.GetComponent<MeshRenderer>().SetMaterials(new List<Material> { diceSpawn.sixSideDiceMaterials[playerIndex], diceSpawn.sixSideDiceMaterials[0] });
                break;
            case DiceType.FourSide:
                gameObject.GetComponent<MeshRenderer>().SetMaterials(new List<Material> { diceSpawn.fourSideDiceMaterials[playerIndex] });
                break;
            case DiceType.EightSide:
                gameObject.GetComponent<MeshRenderer>().SetMaterials(new List<Material> { diceSpawn.eightSideDiceMaterials[playerIndex] });
                break;
        }
    }
}
