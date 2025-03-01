using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager instance;

    public int PlayerDamage { get; set; }
    public int OpponentDamage { get; set; }

    public int Balance { get { return OpponentDamage - PlayerDamage; } }

    [SerializeField]
    private Scales scales;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        OpponentDamage = 0;
        PlayerDamage = 0;
    }

    public IEnumerator ShowDamageSequence(int damage, bool toPlayer, bool isEye = false, bool isHand = false)
    {
        ViewManager.instance.SwitchToView(View.Scales);
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(scales.AddDamage(damage, toPlayer, isEye, isHand));

        if (toPlayer)
        {
            PlayerDamage += damage;
        }
        else
        {
            OpponentDamage += damage;
        }

        yield return new WaitForSeconds(0.25f);
    }
}
