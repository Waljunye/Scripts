using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public int PlayerAvailableSacrifices { get { return playerSlots.FindAll(x => x.Card != null).Count; } }
    public List<CardSlot> PlayerSlots { get { return new List<CardSlot>(playerSlots); } }
    [SerializeField]
    private List<CardSlot> playerSlots = new List<CardSlot>();

    public int OpponentAvailableSacrifices { get { return opponentSlots.FindAll(x => x.Card != null).Count; } }
    public List<CardSlot> OpponentSlots { get { return new List<CardSlot>(opponentSlots); } }
    [SerializeField]
    private List<CardSlot> opponentSlots = new List<CardSlot>();

    public CardSlot LastSelectedSlot { get; set; }
    public bool ChoosingSlot { get; set; }
    private List<CardSlot> currentValidSlots;

    public bool ChoosingSacrifices { get; set; }
    private List<CardSlot> currentSacrifices = new List<CardSlot>();

    public bool CancelledSacrifice { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator ChooseSacrificesForCards(List<CardSlot> validSlots, Card card)
    {
        ViewManager.instance.ViewLocked = true;
        ViewManager.instance.SwitchToView(View.Board);
        Cursor3D.instance.SetCursorType(CursorType.Sacrifice);

        currentValidSlots = validSlots;
        ChoosingSacrifices = true;
        CancelledSacrifice = false;

        yield return new WaitUntil(() => currentSacrifices.Count == card.Info.cost || Input.GetButton("View Down"));

        if (Input.GetButton("View Down"))
        {
            foreach (CardSlot s in currentSacrifices)
            {
                s.Card.SetMarkedForSacrifice(false);
            }
            ViewManager.instance.SwitchToView(View.Default);
            Cursor3D.instance.SetCursorType(CursorType.Default);
            CancelledSacrifice = true;
        }
        else
        {
            ChoosingSacrifices = false;
            yield return new WaitForSeconds(0.5f);

            foreach (CardSlot s in currentSacrifices)
            {
                s.Card.Sacrifice();
            }
            AudioController.Instance.PlaySound("sacrifice");
        }

        // Handle case: no room after sacrifice (Cat)
        if (PlayerSlots.Find(x => x.Card == null) == null)
        {
            ViewManager.instance.SwitchToView(View.Default);
            Cursor3D.instance.SetCursorType(CursorType.Default);
            CancelledSacrifice = true;
        }

        ChoosingSacrifices = false;
        currentSacrifices.Clear();

        ViewManager.instance.ViewLocked = false;
    }

    public IEnumerator ChooseSlot(List<CardSlot> validSlots, bool canCancel)
    {
        ChoosingSlot = true;
        Cursor3D.instance.SetCursorType(CursorType.Place);
        ViewManager.instance.ViewLocked = true;
        ViewManager.instance.SwitchToView(View.Board);

        currentValidSlots = validSlots;
        LastSelectedSlot = null;

        foreach (CardSlot slot in validSlots)
        {
            slot.Chooseable = true;
        }

        yield return new WaitUntil(() => LastSelectedSlot != null || (canCancel && Input.GetButton("View Down")));

        if (canCancel && Input.GetButton("View Down"))
        {
            ViewManager.instance.SwitchToView(View.Default);
        }

        foreach (CardSlot slot in validSlots)
        {
            slot.Chooseable = false;
        }

        ViewManager.instance.ViewLocked = false;
        ChoosingSlot = false;
        Cursor3D.instance.SetCursorType(CursorType.Default);
    }

    public void AssignCardToSlot(Card card, CardSlot slot, float speed = 1f)
    {
        card.GetComponent<Collider>().enabled = false;
        slot.Card = card;
        card.Slot = slot;

        card.transform.parent = null;
        Tween.Position(card.transform, slot.transform.position + (Vector3.up * 0.025f), 0.1f * speed, 0f, Tween.EaseInOut);
        Tween.Rotation(card.transform, slot.transform.GetChild(0).rotation, 0.1f * speed, 0f, Tween.EaseInOut);
    }

    public void OnSlotSelected(CardSlot slot)
    {
        if (currentValidSlots != null)
        {
            if (currentValidSlots.Contains(slot))
            {
                if (ChoosingSacrifices && slot.Card != null && !currentSacrifices.Contains(slot))
                {
                    slot.Card.SetMarkedForSacrifice(true);
                    currentSacrifices.Add(slot);
                }
                else
                {
                    LastSelectedSlot = slot;
                }
            }
        }
    }
}
