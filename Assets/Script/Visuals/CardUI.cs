using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI Visual Elements")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI staminaCostText;
    public Image cardArtImage;

    private CardData assignedCardData;

    public void DisplayCard(CardData data)
    {
        assignedCardData = data;

        if (assignedCardData != null)
        {
            cardNameText.text = assignedCardData.cardName;
            descriptionText.text = assignedCardData.description;
            staminaCostText.text = assignedCardData.staminaCost.ToString();

            if (assignedCardData.cardArt != null)
            {
                cardArtImage.sprite = assignedCardData.cardArt;
            }
        }
    }
}