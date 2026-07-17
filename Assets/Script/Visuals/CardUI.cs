using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image cardArtImage;

    private CardData assignedCardData;

    public void DisplayCard(CardData data)
    {
        assignedCardData = data;
        if (cardArtImage != null && data != null)
        {
            cardArtImage.sprite = data.cardArt;
        }
    }

    public void OnCardClicked()
    {
        if (assignedCardData == null) return;

        BattleManager battleManager = Object.FindFirstObjectByType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.PlayCard(assignedCardData);
        }
    }
}