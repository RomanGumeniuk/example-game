using UnityEngine;

public class ChanceTile : Tile
{

    public ChanceTile(TileScript tile)
    {
        tileScript = tile;
    }

    public override void OnPlayerStepped()
    {
        ShowPickedCard();
    }

    private async void ShowPickedCard()
    {
        int cardIndex = Random.Range(0, GameLogic.Instance.chancesSO.chances.Count);
        ChanceCard card = GameLogic.Instance.chancesSO.chances[cardIndex];
        int moneyAmount = 0;
        switch(card.type)
        {
            case CardType.TakeMoney:
                moneyAmount = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedTypeOfModificator(card.value1, TypeOfModificator.PayingForPenalty);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(moneyAmount, PlayerScript.LocalInstance.playerIndex, 1, true, true);
                await AlertTabForPlayerUI.Instance.ShowTab(string.Format(card.description,moneyAmount),2.5f);
                break;
            case CardType.GiveMoney:
                moneyAmount = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedTypeOfModificator(card.value1, TypeOfModificator.GettingMoney);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(moneyAmount, PlayerScript.LocalInstance.playerIndex, 2, true, true);
                await AlertTabForPlayerUI.Instance.ShowTab(string.Format(card.description, moneyAmount), 2.5f);
                break;
            case CardType.AnotherDiceRoll:
                await AlertTabForPlayerUI.Instance.ShowTab(card.description, 1.5f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                break;
        }
    }

}
