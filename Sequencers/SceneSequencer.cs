using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequencer : MonoBehaviour
{
    public static SceneSequencer instance;

    private void Start()
    {
        instance = this;
    }

    public virtual IEnumerator Intro()
    {
        yield break;
    }

    public virtual IEnumerator End(bool playerWon)
    {
        yield break;
    }

    public virtual IEnumerator PlayerTurnStart()
    {
        yield break;
    }

    public virtual IEnumerator PlayerDuringTurn()
    {
        yield break;
    }

    public virtual IEnumerator OpponentTurnStart()
    {
        yield break;
    }

    public virtual IEnumerator PlayerCombatStart()
    {
        yield break;
    }

    public virtual IEnumerator PlayerCombatEnd()
    {
        yield break;
    }

    public virtual IEnumerator OpponentCombatStart()
    {
        yield break;
    }

    public virtual IEnumerator OpponentCombatEnd()
    {
        yield break;
    }

    public virtual void OnKnifeUsed()
    {

    }

    protected virtual IEnumerator FadeOutAndReload()
    {
        ViewManager.instance.FadeOut();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
