using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI Visual Elements")]
    public Image cardArtImage;

    private CardData assignedCardData;

    public void DisplayCard(CardData data)
    {
        assignedCardData = data;

        if (assignedCardData != null && cardArtImage != null)
        {
            cardArtImage.sprite = assignedCardData.cardArt;
        }
    }
}