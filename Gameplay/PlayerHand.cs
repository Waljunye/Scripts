using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand instance;

    public bool PlayingLocked { get; set; }
    public List<Card> cardsInHand;

    public Deck deck { get; set; }

    [SerializeField]
    private Transform cardsParent;

    private bool choosingSlot;

    private const float PLACEMENT_X_RANGE = 1.75f;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (cardsInHand.Count > 0)
        {
            SetCardPositions();
        }
    }

    public void Draw(bool rabbit = false)
    {
        Card card = null;
        if (rabbit)
        {
            card = deck.DrawRabbit();
        }
        else
        {
            card = deck.DrawCard();
        }

        if (card != null)
        {
            card.transform.parent = cardsParent;
            card.transform.position = transform.position + (Vector3.up * 8f);
            cardsInHand.Add(card);
            OnCardInspected(card);
            SetCardPositions();
        }
        else
        {
            TextDisplayer.instance.ShowMessage("You're out of cards. Good luck.");
            CustomCoroutine.WaitThenExecute(3.5f, TextDisplayer.instance.Clear);
        }
    }

    public void OnCardInspected(Card card)
    {
        AudioController.Instance.PlaySoundWithPitch("card", 0.9f + (Random.value * 0.2f), 0.15f);

        int inspectedIndex = cardsInHand.IndexOf(card);
        foreach (Card c in cardsInHand)
        {
            if (c != card)
            {
                int index = cardsInHand.IndexOf(c);
                int indexDist = cardsInHand.Count - 1 - Mathf.Abs(index - inspectedIndex);
                c.transform.localPosition = new Vector3(c.transform.localPosition.x, c.transform.localPosition.y, -0.02f * indexDist);
            }
        }
        card.transform.localPosition = new Vector3(card.transform.localPosition.x, card.transform.localPosition.y, -0.25f);
    }

    public void OnCardSelected(Card card)
    {
        if (!choosingSlot && CanPlay(card))
        {
            StartCoroutine(SelectSlotForCard(card));
        }
    }

    private bool CanPlay(Card card)
    {
        return card.Info.cost <= BoardManager.instance.PlayerAvailableSacrifices && !PlayingLocked;
    }

    private IEnumerator SelectSlotForCard(Card card)
    {
        BoardManager.instance.CancelledSacrifice = false;
        CombatBell.instance.Enabled = false;
        choosingSlot = true;

        bool hasCost = card.Info.cost > 0;
        if (hasCost)
        {
            var occupiedSlots = BoardManager.instance.PlayerSlots.FindAll(x => x.Card != null);
            yield return StartCoroutine(BoardManager.instance.ChooseSacrificesForCards(occupiedSlots, card));
        }

        if (!BoardManager.instance.CancelledSacrifice)
        {
            var emptySlots = BoardManager.instance.PlayerSlots.FindAll(x => x.Card == null);
            yield return StartCoroutine(BoardManager.instance.ChooseSlot(emptySlots, !hasCost));

            var slot = BoardManager.instance.LastSelectedSlot;
            if (slot != null)
            {
                card.InHand = false;
                cardsInHand.Remove(card);
                BoardManager.instance.AssignCardToSlot(card, slot);

                if (cardsInHand.Count > 0)
                {
                    OnCardInspected(cardsInHand[0]);
                }
            }
        }

        if (card.Info.ability == SpecialAbility.DrawRabbits)
        {
            ViewManager.instance.SwitchToView(View.Hand);
            yield return new WaitForSeconds(0.5f);
            Draw(rabbit: true);
            yield return new WaitForSeconds(0.2f);
            Draw(rabbit: true);
        }

        choosingSlot = false;
        CombatBell.instance.Enabled = true;
    }

    private void SetCardPositions()
    {
        float spacingX = PLACEMENT_X_RANGE / cardsInHand.Count;
        float leftAnchorX = -1.9f;

        foreach (Card c in cardsInHand)
        {
            if (c != null)
            {
                c.InHand = true;

                int index = cardsInHand.IndexOf(c);
                float xPos = leftAnchorX + (spacingX * index);

                float normalizedX = Mathf.Abs(xPos - leftAnchorX) / PLACEMENT_X_RANGE;
                normalizedX = (normalizedX * 2f) - 1f;

                float rotation = normalizedX * -11f;
                float yPos = 1.1f + (0.1f * -Mathf.Abs(normalizedX));

                if (Cursor3D.instance.CurrentInteractable == c)
                {
                    yPos += 0.1f;
                }

                c.InHandPos = new Vector3(xPos, yPos, c.transform.localPosition.z);
                c.transform.localRotation = Quaternion.Euler(5.5f, 0f, rotation);
            }
        }
    }
}
