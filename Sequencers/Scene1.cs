using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1 : SceneSequencer
{

    int turnNumber = 0;

    [SerializeField]
    private LerpAlpha controlsHint;

    public override IEnumerator Intro()
    {
        yield return new WaitForSeconds(1f);
        CombatBell.instance.GetComponent<Collider>().enabled = false;
        ViewManager.instance.FadeIn();

        yield return new WaitForSeconds(1f);
        TextDisplayer.instance.ShowMessage("First, draw your hand. 3 cards.");
        yield return new WaitForSeconds(1f);
    }

    public override IEnumerator PlayerTurnStart()
    {
        turnNumber++;

        switch (turnNumber)
        {
            case 1:
                yield return new WaitForSeconds(1.5f);
                TextDisplayer.instance.ShowMessage("You go first. Draw one more.");
                yield return new WaitForSeconds(1f);
                break;
        }
    }

    public override IEnumerator PlayerDuringTurn()
    {
        switch (turnNumber)
        {
            case 1:

                // TODO: show controls
                yield return new WaitForSeconds(2f);
                TextDisplayer.instance.ShowMessage("Play that Squirrel card.");

                yield return new WaitForSeconds(1f);
                controlsHint.intendedAlpha = 0.4f;
                yield return new WaitUntil(() => ViewManager.instance.CurrentView != View.Default);
                controlsHint.intendedAlpha = 0f;

                yield return new WaitUntil(() => BoardManager.instance.PlayerSlots.Find(x => x.Card != null));

                yield return new WaitForSeconds(0.5f);
                TextDisplayer.instance.ShowMessage("The squirrel is a shrewd creature, but weak.");

                yield return new WaitForSeconds(1f);
                controlsHint.intendedAlpha = 0.4f;

                yield return new WaitUntil(() => ViewManager.instance.CurrentView == View.Default);
                controlsHint.intendedAlpha = 0f;
                TextDisplayer.instance.ShowMessage("Now grab the noble Stoat.");

                yield return new WaitUntil(() => BoardManager.instance.ChoosingSacrifices);
                TextDisplayer.instance.ShowMessage("Stoats cost 1 blood. Sacrifices must be made.");

                yield return new WaitUntil(() => BoardManager.instance.ChoosingSlot);
                TextDisplayer.instance.ShowMessage("An honorable death. Play the Stoat.");

                yield return new WaitUntil(() => BoardManager.instance.PlayerSlots.Find(x => x.Card != null && x.Card.Info.name == "Stoat"));
                yield return new WaitForSeconds(0.5f);

                TextDisplayer.instance.ShowMessage("Wolves require 2 sacrifices. You don't have enough.");
                ViewManager.instance.SwitchToView(View.Hand);
                yield return new WaitForSeconds(5f);

                TextDisplayer.instance.ShowMessage("Hit the bell to start COMBAT and end turn.");

                CombatBell.instance.GetComponent<Collider>().enabled = true;
                break;

            case 2:
                TextDisplayer.instance.ShowMessage("Now defeat me.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.Clear();
                break;
        }
    }

    public override IEnumerator PlayerCombatStart()
    {
        switch (turnNumber)
        {
            case 1:
                yield return new WaitForSeconds(0.5f);
                TextDisplayer.instance.ShowMessage("Your Stoat has nothing to oppose it.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.ShowMessage("The left number on the bottom is its Attack Power: 1.");
                yield return new WaitForSeconds(3f);
                break;
        }
    }

    public override IEnumerator PlayerCombatEnd()
    {
        switch (turnNumber)
        {
            case 1:
                yield return new WaitForSeconds(1f);

                TextDisplayer.instance.ShowMessage("The Stoat dealt me 1 damage. I added it to the scale.");
                yield return new WaitForSeconds(5f);

                TextDisplayer.instance.ShowMessage("You win if you tip my side all the way down.");
                yield return new WaitForSeconds(4f);
                break;
        }
    }

    public override IEnumerator OpponentTurnStart()
    {
        OpponentAI.instance.Mode = AIPlayMode.PickStrongest;

        switch (turnNumber)
        {
            case 1:
                yield return new WaitForSeconds(1f);

                TextDisplayer.instance.ShowMessage("My turn.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.Clear();
                break;
            case 3:
                yield return new WaitForSeconds(1f);

                TextDisplayer.instance.ShowMessage("Because you are learning I will pass.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.Clear();
                break;
            case 4:
                yield return new WaitForSeconds(1f);

                TextDisplayer.instance.ShowMessage("Pass.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.Clear();
                break;
        }
    }

    public override IEnumerator OpponentCombatStart()
    {
        switch (turnNumber)
        {
            case 1:
                TextDisplayer.instance.ShowMessage("Now my Starving Man faces your stoat.");
                yield return new WaitForSeconds(4f);
                TextDisplayer.instance.ShowMessage("He will fight it, as men do.");
                yield return new WaitForSeconds(2f);
                break;
        }
    }

    public override IEnumerator OpponentCombatEnd()
    {
        switch (turnNumber)
        {
            case 1:
                TextDisplayer.instance.ShowMessage("He dealt it 1 damage.");
                yield return new WaitForSeconds(3f);
                TextDisplayer.instance.ShowMessage("This is subtracted from the Stoat's health.");
                yield return new WaitForSeconds(4f);
                TextDisplayer.instance.ShowMessage("If a creature's health reaches 0, it dies.");
                yield return new WaitForSeconds(3f);
                break;
        }
    }

    public override IEnumerator End (bool playerWon)
    {
        if (playerWon)
        {
            GameStats.sceneProgress = 1;
        }

        TextDisplayer.instance.ShowMessage("Very good.");
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(FadeOutAndReload());
    }
}
