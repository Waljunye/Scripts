using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public OpponentAI opponent { get; set; }

    public bool IsPlayerTurn { get; set; }
    public bool IsCombatPhase { get; set; }

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(GameSequence());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator GameSequence()
    {
        if (GameStats.sceneProgress > 0)
        {
            AudioController.Instance.SetLoop("main_loop");
            AudioController.Instance.SetLoopVolume(0f);
            AudioController.Instance.FadeInLoop(0.2f, 0.75f);
        }

        yield return new WaitForEndOfFrame();
        if (SceneSequencer.instance == null)
        {
            yield break;
        }
        PlayerHand.instance.PlayingLocked = true;

        yield return StartCoroutine(SceneSequencer.instance.Intro());

        yield return new WaitForSeconds(0.5f);

        int startingHandSize = 3;
        for (int i = 0; i < startingHandSize; i++)
        {
            PlayerHand.instance.Draw();
            yield return new WaitForSeconds(0.2f);
        }

        while (!GameHasEnded())
        {
            yield return StartCoroutine(PlayerTurn());

            if (GameHasEnded())
            {
                break;
            }

            yield return StartCoroutine(OpponentTurn());
        }

        AudioController.Instance.FadeOutLoop(0.5f);
        yield return StartCoroutine(SceneSequencer.instance.End(PlayerIsWinner()));
    }

    private IEnumerator PlayerTurn()
    {
        IsPlayerTurn = true;
        ViewManager.instance.SwitchToView(View.Default);
        ViewManager.instance.ViewLocked = true;

        yield return StartCoroutine(SceneSequencer.instance.PlayerTurnStart());

        StartCoroutine(SceneSequencer.instance.PlayerDuringTurn());

        yield return new WaitForSeconds(0.5f);

        if (PlayerHand.instance != null)
        {
            PlayerHand.instance.Draw();
        }

        yield return new WaitForSeconds(0.25f);
        CombatBell.instance.Enabled = true;

        if (PlayerHand.instance != null)
        {
            PlayerHand.instance.PlayingLocked = false;
        }
        ViewManager.instance.ViewLocked = false;

        while (!CombatBell.instance.Rang)
        {
            if (GameHasEnded())
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        CombatBell.instance.Rang = false;

        yield return StartCoroutine(DoCombatPhase(playerIsAttacker: true));
    }

    private IEnumerator OpponentTurn()
    {
        IsPlayerTurn = false;

        if (PlayerHand.instance != null)
        {
            PlayerHand.instance.PlayingLocked = true;
        }

        ViewManager.instance.SwitchToView(View.Default);
        ViewManager.instance.ViewLocked = true;

        yield return StartCoroutine(SceneSequencer.instance.OpponentTurnStart());

        yield return new WaitForSeconds(1f);

        opponent.DoPlayPhase();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(DoCombatPhase(playerIsAttacker: false));
    }

    private IEnumerator DoCombatPhase(bool playerIsAttacker)
    {
        CombatBell.instance.Enabled = false;
        IsCombatPhase = true;
        var attackingSlots = playerIsAttacker ? BoardManager.instance.PlayerSlots : BoardManager.instance.OpponentSlots;
        attackingSlots.RemoveAll(x => x.Card == null || x.Card.Attack == 0);

        if (attackingSlots.Count > 0)
        {
            ViewManager.instance.SwitchToView(View.Board);
            ViewManager.instance.ViewLocked = true;
        }

        if (playerIsAttacker)
        {
            yield return StartCoroutine(SceneSequencer.instance.PlayerCombatStart());
        }
        else
        {
            yield return StartCoroutine(SceneSequencer.instance.OpponentCombatStart());
        }

        foreach (CardSlot slot in attackingSlots)
        {
            ViewManager.instance.SwitchToView(View.Board);
            ViewManager.instance.ViewLocked = true;
            yield return new WaitForSeconds(0.5f);

            if (slot.opposingSlot.Card != null && slot.Card.Info.ability != SpecialAbility.Flying)
            {
                slot.Card.DoAttackAnimation(false);

                yield return new WaitForSeconds(0.6f);

                slot.opposingSlot.Card.TakeDamage(slot.Card.Attack);
            }
            else
            {
                slot.Card.DoAttackAnimation(true);

                yield return new WaitForSeconds(0.35f);

                AudioController.Instance.PlaySound("die");
                yield return StartCoroutine(LifeManager.instance.ShowDamageSequence(slot.Card.Attack, !playerIsAttacker));

                if (GameHasEnded())
                {
                    break;
                }
            }
        }

        if (playerIsAttacker)
        {
            yield return StartCoroutine(SceneSequencer.instance.PlayerCombatEnd());
        }
        else
        {
            yield return StartCoroutine(SceneSequencer.instance.OpponentCombatEnd());
        }

        IsCombatPhase = false;
        ViewManager.instance.ViewLocked = false;
        yield return new WaitForSeconds(0.5f);
    }

    private bool PlayerIsWinner()
    {
        return LifeManager.instance.Balance >= 5;
    }

    private bool GameHasEnded()
    {
        return Mathf.Abs(LifeManager.instance.Balance) >= 5;
    }
}
