using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceSelector : MonoBehaviour
{
    [SerializeField]
    private List<SceneSequencer> sequencers = new List<SceneSequencer>();

    [SerializeField]
    private int debugProgress = 0;

	void Start ()
    {
#if UNITY_EDITOR
        GameStats.sceneProgress = debugProgress;
#endif

        SceneSequencer.instance = null;
        ViewManager.instance.SetBlack();
        if (GameStats.sceneProgress < sequencers.Count)
        {
            sequencers[GameStats.sceneProgress].gameObject.SetActive(true);
            PlayerHand.instance.deck = sequencers[GameStats.sceneProgress].transform.Find("PlayerDeck").GetComponent<Deck>();
            TurnManager.instance.opponent = sequencers[GameStats.sceneProgress].GetComponentInChildren<OpponentAI>();
        }

	}
}
