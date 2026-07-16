using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck Databases")]
    public List<CardData> startingDeck = new List<CardData>();

    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> handPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    public int DrawPileCount => drawPile.Count;
    public int HandCount => handPile.Count;
    public int DiscardPileCount => discardPile.Count;


    public void SetupDeck()
    {
        drawPile.Clear();
        handPile.Clear();
        discardPile.Clear();

        drawPile.AddRange(startingDeck);

        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            CardData temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    public List<CardData> DrawCards(int amount)
    {
        List<CardData> drawnCards = new List<CardData>();

        for (int i = 0; i < amount; i++)
        {
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0) break;

                drawPile.AddRange(discardPile);
                discardPile.Clear();
                ShuffleDeck();
                Debug.Log("Deck habis! Mengocok ulang dari Discard Pile.");
            }

            CardData card = drawPile[0];
            drawPile.RemoveAt(0);
            handPile.Add(card);
            drawnCards.Add(card);
        }

        return drawnCards;
    }

    public void DiscardCard(CardData card)
    {
        if (handPile.Contains(card))
        {
            handPile.Remove(card);
            discardPile.Add(card);
        }
    }

    public void DiscardHand()
    {
        discardPile.AddRange(handPile);
        handPile.Clear();
    }

    public List<CardData> DiscardHandAndRedraw()
    {
        int currentHandCount = handPile.Count;

        DiscardHand();

        Debug.Log($"[Ganti Judul] Membuang {currentHandCount} kartu. Menarik kartu baru...");

        return DrawCards(currentHandCount);
    }
}