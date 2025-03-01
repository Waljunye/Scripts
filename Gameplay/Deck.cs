using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    [SerializeField]
    private List<CardInfo> fixedDraws = new List<CardInfo>();

    [SerializeField]
    private List<CardInfo> cards = new List<CardInfo>();

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private CardInfo rabbit;

    public Card DrawRabbit()
    {
        var cardGO = SpawnCard();
        var card = cardGO.GetComponent<Card>();

        card.SetInfo(rabbit);

        return card;
    }

	public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        CardInfo info = null;
        if (fixedDraws.Count > 0)
        {
            info = fixedDraws[0];
            fixedDraws.RemoveAt(0);
        }
        else
        {
            info = cards[Random.Range(0, cards.Count)];
        }
        cards.Remove(info);

        var cardGO = SpawnCard();
        var card = cardGO.GetComponent<Card>();

        card.SetInfo(info);

        return card;
    }

    private GameObject SpawnCard()
    {
        var card = Instantiate(cardPrefab);
        return card;
    }
}
