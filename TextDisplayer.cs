using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Emotion
{
    Neutral,
    Laughter,
    Anger,
    None,
}

public class TextDisplayer : MonoBehaviour
{
    public static TextDisplayer instance;

    [SerializeField]
    private TextMesh textMesh;

    private void Awake()
    {
        instance = this;
    }

    public void ShowMessage(string message, Emotion emotion = Emotion.Neutral)
    {
        textMesh.gameObject.SetActive(true);
        textMesh.text = message;

        switch (emotion)
        {
            case Emotion.Neutral:
                AudioController.Instance.PlayRandomSound("spiritvoice", true, true);
                break;
            case Emotion.Anger:
                AudioController.Instance.PlaySound("spiritvoice_impatient");
                break;
            case Emotion.Laughter:
                AudioController.Instance.PlaySound("spiritvoice_laugh");
                break;
        }
    }

    public void Clear()
    {
        textMesh.gameObject.SetActive(false);
    }
}
