using UnityEngine;

public class ChanceTile : Tile
{

    public ChanceTile(TileScript tile)
    {
        tileScript = tile;
    }

    public override void OnPlayerStepped(int value1 = 0, float value2 = 0)
    {
        ShowPickedCard();
    }

    private async void ShowPickedCard()
    {
        int cardIndex = Random.Range(0, GameLogic.Instance.chancesSO.chances.Count);
        ChanceCard card = GameLogic.Instance.chancesSO.chances[cardIndex];

        switch(card.type)
        {
            case CardType.TakeMoney:
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(card.value1, PlayerScript.LocalInstance.playerIndex, 1, true, true);
                await AlertTabForPlayerUI.Instance.ShowTab(card.description,2.5f);
                break;
            case CardType.GiveMoney:
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(card.value1, PlayerScript.LocalInstance.playerIndex, 2, true, true);
                await AlertTabForPlayerUI.Instance.ShowTab(card.description, 2.5f);
                break;
            case CardType.AnotherDiceRoll:
                await AlertTabForPlayerUI.Instance.ShowTab(card.description, 1.5f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                break;
        }
    }

}
