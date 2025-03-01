using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIPlayMode
{
    PickEmpty,
    PickWeakest,
    PickStrongest,
}

public class OpponentAI : MonoBehaviour
{
    public static OpponentAI instance;

    private Deck deck;

    public AIPlayMode Mode { get; set; }

    void Start()
    {
        instance = this;
        deck = GetComponentInChildren<Deck>();
    }

    public virtual void DoPlayPhase()
    {
        var validSlots = BoardManager.instance.OpponentSlots.FindAll(x => x.Card == null);
        if (validSlots.Count > 0)
        {
            var card = deck.DrawCard();

            if (card != null)
            {
                card.SetIsOpponentCard();

                var slot = PickSlot(validSlots);
                card.transform.position = slot.transform.position + (Vector3.forward) * 6f + (Vector3.up * 5f);
                card.transform.rotation = Quaternion.Euler(130f, 0f, 180f);
                BoardManager.instance.AssignCardToSlot(card, slot, 10f);
                AudioController.Instance.PlaySoundWithPitch("card", 0.9f + (Random.value * 0.2f), 0.15f);
            }
        }
    }

    private CardSlot PickSlot(List<CardSlot> validSlots)
    {
        CardSlot slot = null;

        List<CardSlot> slots = null;
        switch (Mode)
        {
            case AIPlayMode.PickEmpty:
                slots = validSlots.FindAll(x => x != null && x.opposingSlot.Card == null);
                break;
            case AIPlayMode.PickStrongest:
                slots = validSlots.FindAll(x => x != null && x.opposingSlot.Card != null); //TODO find strongest
                break;
            case AIPlayMode.PickWeakest:
                slots = validSlots.FindAll(x => x != null && x.opposingSlot.Card != null); //TODO find weakest
                break;

        }

        if (slots != null && slots.Count > 0)
        {
            slot = slots[Random.Range(0, slots.Count)];
        }

        if (slot == null)
        {
            slot = validSlots[Random.Range(0, validSlots.Count)];
        }
        return slot;
    }
}
