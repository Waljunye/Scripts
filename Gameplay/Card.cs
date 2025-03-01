using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : Interactable
{
    public int Attack { get { return attack; } }
    private int attack;
    private int health;

    public CardInfo Info { get { return info; } }
    private CardInfo info;

    public CardSlot Slot { get; set; }

    public bool InHand { get; set; }
    public Vector2 InHandPos { get; set; }

    [SerializeField]
    private TextMesh healthText;

    [SerializeField]
    private TextMesh attackText;

    [SerializeField]
    private Renderer cardRenderer;

    [SerializeField]
    private GameObject sacrificeMarker;

    [SerializeField]
    private GameObject deathParticles;

    public void SetInfo(CardInfo info)
    {
        this.info = info;

        attack = info.baseAttack;
        health = info.baseHealth;

        cardRenderer.material.mainTexture = info.texture;

        UpdateText();
    }

    public void SetIsOpponentCard()
    {
        cardRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
    }

    public void TakeDamage(int damage)
    {
        AudioController.Instance.PlaySound("die");
        health = Mathf.Max(health - damage, 0);
        
        if (health <= 0)
        {
            Die();
        }

        UpdateText();
        GetComponent<Animator>().SetTrigger("take_hit");
    }

    public void SetMarkedForSacrifice(bool marked)
    {
        sacrificeMarker.SetActive(marked);
        GetComponent<Animator>().SetTrigger("sacrifice_selected");
        AudioController.Instance.PlaySoundWithPitch("card", 0.9f + (Random.value * 0.2f), 0.2f);
    }

    public void Sacrifice()
    {
        if (info.ability == SpecialAbility.Sacrificial)
        {
            // TODO cat sound
            SetMarkedForSacrifice(false);
            var particles = Instantiate(deathParticles);
            particles.SetActive(true);
            particles.transform.SetParent(transform);
            particles.transform.position = deathParticles.transform.position;
            particles.transform.localScale = deathParticles.transform.localScale;
            particles.transform.rotation = deathParticles.transform.rotation;
            Destroy(particles, 6f);
        }
        else
        {
            Die();
        }
    }

    public void DoAttackAnimation(bool attackPlayer)
    {
        AudioController.Instance.PlaySound("growl");
        GetComponent<Animator>().SetTrigger(attackPlayer ? "attack_player" : "attack_creature");
    }

    private void Die()
    {
        if (Slot != null)
        {
            Slot.Card = null;
        }

        deathParticles.transform.parent = null;
        deathParticles.gameObject.SetActive(true);
        Destroy(deathParticles, 6f);

        GetComponent<Animator>().SetTrigger("death");
        Destroy(gameObject, 2f);
    }

    private void UpdateText()
    {
        healthText.text = health.ToString();
        attackText.text = Attack.ToString();
    }

    private void Update()
    {
        if (InHand)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(InHandPos.x, InHandPos.y, transform.localPosition.z), Time.deltaTime * 8f);
        }
    }

    public override void OnCursorSelectStart()
    {
        if (PlayerHand.instance != null)
        {
            PlayerHand.instance.OnCardSelected(this);
        }
    }

    public override void OnCursorEnter()
    {
        if (PlayerHand.instance != null)
        {
            PlayerHand.instance.OnCardInspected(this);
        }
    }
}
