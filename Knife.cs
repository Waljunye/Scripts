using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Interactable
{
    public bool Enabled
    {
        get
        {
            return isEnabled;
        }
        set
        {
            isEnabled = value;
            if (Cursor3D.instance.CurrentInteractable != null && Cursor3D.instance.CurrentInteractable == this)
            {
                if (isEnabled)
                {
                    OnCursorEnter();
                }
                else
                {
                    OnCursorExit();
                }
            }
        }
    }
    private bool isEnabled;

    private void Update()
    {
        Enabled = TurnManager.instance.IsPlayerTurn && !TurnManager.instance.IsCombatPhase;
    }

    public override void OnCursorSelectStart()
    {
        if (Enabled)
        {
            SceneSequencer.instance.OnKnifeUsed();
            Cursor3D.instance.SetCursorType(CursorType.Default);
            AudioController.Instance.PlaySound("sacrifice");
        }
    }

    public override void OnCursorEnter()
    {
        if (Enabled)
        {
            Cursor3D.instance.SetCursorType(CursorType.Sacrifice);
        }
    }

    public override void OnCursorExit()
    {
        if (Enabled)
        {
            Cursor3D.instance.SetCursorType(CursorType.Default);
        }
    }
}
