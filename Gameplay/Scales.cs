using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Scales : MonoBehaviour {

    [SerializeField]
    private GameObject weightPrefab;

    [SerializeField]
    private Transform playerSide;

    [SerializeField]
    private Transform opponentSide;

    [SerializeField]
    private Transform rotator;

    [SerializeField]
    private GameObject eye;

    [SerializeField]
    private GameObject hand;

    private int opponentWeight = 0;
    private int playerWeight = 0;

    private const float MAX_ROTATION = 30f;

	public IEnumerator AddDamage(int damage, bool toPlayer, bool isEye, bool isHand)
    {
        if (isEye)
        {
            eye.SetActive(true);
            opponentWeight += damage;
            SetScalesBalance();
            yield return new WaitForSeconds(0.2f);
        }
        else if (isHand)
        {
            hand.SetActive(true);
            opponentWeight += damage;
            SetScalesBalance();
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            for (int i = 0; i < damage; i++)
            {
                var weight = Instantiate(weightPrefab);
                weight.transform.parent = transform;
                weight.transform.position = toPlayer ? playerSide.transform.position : opponentSide.transform.position;
                weight.transform.eulerAngles = Random.onUnitSphere * 360f;
                weight.gameObject.SetActive(true);

                weight.transform.localScale = Vector3.zero;
                Tween.LocalScale(weight.transform, Vector3.one * 0.2f, 0.1f, 0f, Tween.EaseIn);
                weight.GetComponent<Rigidbody>().useGravity = false;
                yield return new WaitForSeconds(0.12f);
                weight.GetComponent<Rigidbody>().useGravity = true;

                weight.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 7f);
                weight.GetComponent<Rigidbody>().AddForce(Vector3.down * 9f);

                yield return new WaitForSeconds(0.12f);

                if (toPlayer)
                {
                    playerWeight++;
                }
                else
                {
                    opponentWeight++;
                }

                SetScalesBalance();
                AudioController.Instance.PlaySoundWithPitch("weight_drop", 0.9f + (Random.value * 0.2f), 0.2f);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private const float INCREMENT = 5.5f;
    private void SetScalesBalance()
    {
        float rotation = (opponentWeight - playerWeight) * INCREMENT; // ASSUMING game over at 5
        rotation = Mathf.Clamp(rotation, INCREMENT * -5, INCREMENT * 5);

        Tween.LocalRotation(rotator, Quaternion.Euler(rotation, 56f, 0f), 0.3f, 0f, Tween.EaseOut);
        //rotator.localRotation = Quaternion.Euler(rotation, 56f, 0f);
    }
}
