using System.Collections.Generic;
using UnityEngine;

public class SlotCard : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform handPanelTransform;
    public GameObject cardPrefab;

    private List<GameObject> spawnedCardVisuals = new List<GameObject>();

    public void RefreshHandVisuals(List<CardData> currentHandCards)
    {
        foreach (GameObject cardVisual in spawnedCardVisuals)
        {
            Destroy(cardVisual);
        }
        spawnedCardVisuals.Clear();

        foreach (CardData cardData in currentHandCards)
        {
            if (cardData != null)
            {
                GameObject newCard = Instantiate(cardPrefab, handPanelTransform);
                spawnedCardVisuals.Add(newCard);

                CardUI cardUIScript = newCard.GetComponent<CardUI>();
                if (cardUIScript != null)
                {
                    cardUIScript.DisplayCard(cardData);
                }
            }
        }
    }
}