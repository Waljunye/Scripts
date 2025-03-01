using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene4 : SceneSequencer
{
    int turnNumber = 0;

    [SerializeField]
    private Knife knife;

    [SerializeField]
    private GameObject eyeCover;

    [SerializeField]
    private GameObject choices;

    bool showedKnife = false;

    private static bool seenDialogue = false;

    public override IEnumerator Intro()
    {
        if (GameStats.lostEye)
        {
            eyeCover.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        ViewManager.instance.FadeIn();

        if (!seenDialogue)
        {
            yield return new WaitForSeconds(1f);
            TextDisplayer.instance.ShowMessage("I see your child there behind you.");
            yield return new WaitForSeconds(3f);
            TextDisplayer.instance.ShowMessage("It also looks hungry.");
            yield return new WaitForSeconds(3f);
            TextDisplayer.instance.ShowMessage("If you beat me now, we will eat.");
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
            TextDisplayer.instance.ShowMessage("I will offer my knife to you again.", Emotion.Neutral);
            yield return new WaitForSeconds(2f);

            knife.gameObject.SetActive(true);
            AudioController.Instance.PlaySound("sacrifice");
            yield return new WaitForSeconds(1f);

            TextDisplayer.instance.ShowMessage("Be careful when you use it.", Emotion.Neutral);
            yield return new WaitForSeconds(3f);

            TextDisplayer.instance.ShowMessage("You will not have a great hand after.", Emotion.Laughter);
            yield return new WaitForSeconds(3f);
            TextDisplayer.instance.Clear();
        }
    }

    public override void OnKnifeUsed()
    {
        knife.GetComponent<Collider>().enabled = false;
        StartCoroutine(KnifeSequence());
    }

    private IEnumerator KnifeSequence()
    {
        ViewManager.instance.ViewLocked = true;
        PlayerHand.instance.PlayingLocked = true;

        AudioController.Instance.PlaySound("suspense_1");

        ViewManager.instance.FadeInRed();
        yield return new WaitForSeconds(0.05f);

        ViewManager.instance.FadeOutRed();
        knife.gameObject.SetActive(false);
        Destroy(PlayerHand.instance.gameObject);

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(LifeManager.instance.ShowDamageSequence(4, false, false, true));

        yield return new WaitForSeconds(1f);
        if (LifeManager.instance.Balance < 5)
        {
            TextDisplayer.instance.ShowMessage("A heavy hand.", Emotion.Laughter);
            CustomCoroutine.WaitThenExecute(2.5f, TextDisplayer.instance.Clear);
            yield return new WaitForSeconds(1f);

            ViewManager.instance.SwitchToView(View.Default);

            ViewManager.instance.ViewLocked = false;
        }
    }

    public override IEnumerator End(bool playerWon)
    {
        if (playerWon)
        {
            GameStats.sceneProgress = 4;
            yield return new WaitForSeconds(1f);
            TextDisplayer.instance.ShowMessage("You've done it.");
        }
        else
        {
            TextDisplayer.instance.ShowMessage("You lose.");
        }

        yield return new WaitForSeconds(3f);

        ViewManager.instance.FadeOut();

        if (playerWon)
        {
            TextDisplayer.instance.ShowMessage("It's time to eat.");
            yield return new WaitForSeconds(3f);

            TextDisplayer.instance.ShowMessage("Please, choose our menu.");
            yield return new WaitForSeconds(1f);

            choices.SetActive(true);
            ViewManager.instance.FadeIn();
            ViewManager.instance.SwitchToView(View.Choices, immediate: true);
            ViewManager.instance.ViewLocked = true;

            yield return new WaitUntil(() => madeChoice);
            ViewManager.instance.FadeOut();

            if (choseChild)
            {
                TextDisplayer.instance.ShowMessage("Very well.");
                yield return new WaitForSeconds(3f);

                TextDisplayer.instance.ShowMessage("Let's eat.");
                yield return new WaitForSeconds(3f);
                Application.Quit();
            }
            else
            {
                TextDisplayer.instance.ShowMessage("How very noble.");
                yield return new WaitForSeconds(3f);

                TextDisplayer.instance.ShowMessage("Much like the stoat.");
                yield return new WaitForSeconds(3f);

                AudioController.Instance.PlaySound("sacrifice");
                yield return new WaitForSeconds(0.4f);
                Application.Quit();
            }

        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private bool madeChoice = false;
    private bool choseChild = false;

    public void MakeChoice (bool child)
    {
        madeChoice = true;
        choseChild = child;
    }
}
