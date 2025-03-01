using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour {

    private void Start()
    {
        AudioController.Instance.SetLoop("main_loop");
        AudioController.Instance.SetLoopVolume(0f);
        AudioController.Instance.FadeInLoop(0.2f, 0.75f);
    }

    void Update ()
    {
		if (Input.anyKeyDown)
        {
            GetComponent<LerpAlpha>().intendedAlpha = 0f;

            CustomCoroutine.WaitThenExecute(3f, Next);
            enabled = false;
        }
	}

    void Next()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("scene1");
    }
}
