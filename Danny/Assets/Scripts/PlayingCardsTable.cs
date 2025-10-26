using System.Collections.Generic;
using UnityEngine;

public class PlayingCardsTable : MonoBehaviour
{
    private const float HAND_CARDS_DOWN_OFFSET = 20f;
    private const float HAND_CARDS_BETWEEN_OFFSET = 10f;

    private List<Card> cardsInHand = new();
    private List<Card> cardsOnTable = new();
    private int handCardsLayer;

    public void Start()
    {
        Canvas[] allCanvases = GetComponentsInChildren<Canvas>();
        handCardsLayer = allCanvases.Length;
        foreach (Canvas canv in allCanvases) { 
            Card card = canv.GetComponentInChildren<Card>();
            if (card.InHand) {
                cardsInHand.Add(card);
                card.ChangeLayer(handCardsLayer);
            }
            else {
                cardsOnTable.Add(card);
            }
        }
        ReorderCardsLayers();
        PlaceHandCards();
    }

    public void PlaceCardFromHandOnTable(Card card)
    {
        cardsInHand.Remove(card);
        cardsOnTable.Add(card);
        card.InHand = false;
        ReorderCardsLayers();
        PlaceHandCards();
    }

    public void ReturnCardToHand(Card card)
    {
        cardsInHand.Add(card);
        cardsOnTable.Remove(card);
        card.InHand = true;
        card.ChangeLayer(handCardsLayer);
        ReorderCardsLayers();
        PlaceHandCards();
    }

    public void ChangeCardInOrder(Card card, int offset)
    {
        int index = cardsOnTable.IndexOf(card);
        int nexIndex = index + offset;
        if (nexIndex >= cardsOnTable.Count || nexIndex < 0)
            return;
        cardsOnTable.Remove(card);
        cardsOnTable.Insert(index + offset, card);
        ReorderCardsLayers();
    }

    private void PlaceHandCards()
    {
        if (cardsInHand.Count == 0) return;
        float width = cardsInHand[0].Width;
        float height = cardsInHand[0].Height;
        float count = cardsInHand.Count;
        float xPos = - (count - 1) * (HAND_CARDS_BETWEEN_OFFSET + width) / 2;
        float yPos = HAND_CARDS_DOWN_OFFSET + (height - Screen.height) / 2;
        foreach (Card card in cardsInHand)
        {
            card.ReturnToHand(new Vector2(xPos, yPos));
            xPos += HAND_CARDS_BETWEEN_OFFSET + width;
        }
    }

    private void ReorderCardsLayers()
    {
        for (int i = 0; i < cardsOnTable.Count; ++i)
        {
            cardsOnTable[i].ChangeLayer(i);
        }
    }
}
