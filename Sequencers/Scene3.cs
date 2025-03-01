using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene3 : SceneSequencer
{

    private bool playedWarrenMessage = false;

    int turnNumber = 0;

    [SerializeField]
    private Knife knife;

    [SerializeField]
    private GameObject eyeCover;

    bool showedKnife = false;
    bool knifeSequence = false;
    private static bool seenDialogue = false;

    public override IEnumerator Intro()
    {
        yield return new WaitForSeconds(1f);
        ViewManager.instance.FadeIn();

        if (!seenDialogue)
        {
            yield return new WaitForSeconds(1f);
            TextDisplayer.instance.ShowMessage("You told me you were the only survivor of the crash.");
            yield return new WaitForSeconds(4f);
            TextDisplayer.instance.ShowMessage("If that is true, then who is that behind you?");
            CustomCoroutine.WaitThenExecute(3f, TextDisplayer.instance.Clear);
            seenDialogue = true;
        }
    }

    public override IEnumerator PlayerTurnStart()
    {
        turnNumber++;

        if (LifeManager.instance.Balance < 0 && !showedKnife)
        {
            showedKnife = true;
            TextDisplayer.instance.ShowMessage("You are losing.", Emotion.Neutral);
            yield return new WaitForSeconds(2f);

            knife.gameObject.SetActive(true);
            AudioController.Instance.PlaySound("sacrifice");

            yield return new WaitForSeconds(1f);
            TextDisplayer.instance.ShowMessage("But I will allow you to tip the scales.", Emotion.Neutral);
            yield return new WaitForSeconds(3f);
            TextDisplayer.instance.Clear();
        }
    }

    public override IEnumerator PlayerDuringTurn()
    {
        while (TurnManager.instance.IsPlayerTurn)
        {
            if (!playedWarrenMessage && !knifeSequence)
            {
                if (PlayerHand.instance.cardsInHand.Find(x => x.Info.name == "Warren"))
                {
                    playedWarrenMessage = true;
                    yield return new WaitForSeconds(1f);
                    TextDisplayer.instance.ShowMessage("Ah, a warren.");
                    yield return new WaitForSeconds(3f);
                    TextDisplayer.instance.ShowMessage("You will draw some rabbits when it is played.");
                    yield return new WaitForSeconds(4f);
                    TextDisplayer.instance.Clear();
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public override IEnumerator OpponentTurnStart()
    {
        if (turnNumber == 1)
        {
            yield return new WaitForSeconds(1f);
            TextDisplayer.instance.ShowMessage("You dare lie to me? In my forest?", Emotion.Anger);
            yield return new WaitForSeconds(1f);
            CustomCoroutine.WaitThenExecute(2.5f, TextDisplayer.instance.Clear);
        }
    }

    public override void OnKnifeUsed()
    {
        knife.GetComponent<Collider>().enabled = false;
        StartCoroutine(KnifeSequence());
    }

    private IEnumerator KnifeSequence()
    {
        knifeSequence = true;
        ViewManager.instance.ViewLocked = true;
        PlayerHand.instance.PlayingLocked = true;

        GameStats.lostEye = true;

        AudioController.Instance.PlaySound("suspense_1");

        ViewManager.instance.FadeInRed();
        yield return new WaitForSeconds(0.05f);

        ViewManager.instance.FadeOutRed();
        knife.gameObject.SetActive(false);
        eyeCover.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(LifeManager.instance.ShowDamageSequence(2, false, true));

        yield return new WaitForSeconds(1f);
        TextDisplayer.instance.ShowMessage("That ought to even things out.", Emotion.Neutral);
        CustomCoroutine.WaitThenExecute(2.5f, TextDisplayer.instance.Clear);
        yield return new WaitForSeconds(1f);

        ViewManager.instance.SwitchToView(View.Default);

        ViewManager.instance.ViewLocked = false;
        PlayerHand.instance.PlayingLocked = false;
        knifeSequence = false;
    }

    public override IEnumerator End(bool playerWon)
    {
        if (playerWon)
        {
            GameStats.sceneProgress = 3;
            TextDisplayer.instance.ShowMessage("You win again.");
        }
        else
        {
            TextDisplayer.instance.ShowMessage("You lose.");
        }

        yield return new WaitForSeconds(3f);

        ViewManager.instance.FadeOut();

        if (playerWon)
        {
            TextDisplayer.instance.ShowMessage("One more round, human.");
            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
