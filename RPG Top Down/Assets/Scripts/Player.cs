using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Entity entity;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Player Regen System")]
    public bool regenHPEnabled = true;
    public float regenHPTime = 5f;
    public int regenHPValue = 5;
    public bool regenMPEnabled = true;
    public float regenMPTime = 10f;
    public int regenMPValue = 5;

    [Header("Player UI")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;
    public Slider expSlider;
    public TMP_Text expText;
    public TMP_Text levelText;

    [Header("EXP")]
    public int currentExp;
    public int expLeft;
    public int expBase;
    public float expMod;
    public GameObject levelUpEffect;
    public AudioClip levelUpSound;

    [Header("Respawn")]
    public float respawnTime = 5f;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("Você precisa atribuir o GameManager no inspetor!");
            return;
        }

        entity.maxHealth = gameManager.CalculateHealth(entity);
        entity.maxMana = gameManager.CalculateMana(entity);
        entity.maxStamina = gameManager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        manaSlider.maxValue = entity.maxMana;
        manaSlider.value = manaSlider.maxValue;

        staminaSlider.maxValue = entity.maxStamina;
        staminaSlider.value = staminaSlider.maxValue;

        expSlider.value = currentExp;
        expSlider.maxValue = expLeft;

        expText.text = string.Format("Exp: (0)/(1)", currentExp, expLeft);
        levelText.text = entity.level.ToString();

        // Testando a regeneração de vida do jogador
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
    }

    void Update()
    {
        if (entity.dead)
            return;

        if(entity.currentHealth <= 0)
        {
            Die();
        }

        healthSlider.value = entity.currentHealth;
        manaSlider.value = entity.currentMana;
        staminaSlider.value = entity.currentStamina;

        expSlider.value = currentExp;
        expSlider.maxValue = expLeft;
        expText.text = string.Format("Exp: (0)/(1)", currentExp, expLeft);
        levelText.text = entity.level.ToString();
    }

    IEnumerator RegenHealth()
    {
        while (true)
        {
            if (regenHPEnabled)
            {
                if (entity.currentHealth < entity.maxHealth)
                {
                    Debug.LogFormat("Recuperando o HP do jogador");
                    entity.currentHealth += regenHPValue;
                    yield return new WaitForSeconds(regenHPTime);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator RegenMana()
    {
        while (true)
        {
            if (regenMPEnabled)
            {
                if (entity.currentMana < entity.maxMana)
                {
                    Debug.LogFormat("Recuperando a mana do jogador");
                    entity.currentMana += regenMPValue;
                    yield return new WaitForSeconds(regenMPTime);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    void Die()
    {
        entity.currentHealth = 0;
        entity.dead = true;
        entity.target = null;
        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        GetComponent<PlayerController>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        GameObject newPlayer = Instantiate(prefab, transform.position, transform.rotation, null);
        newPlayer.name = prefab.name;
        newPlayer.GetComponent<Player>().entity.dead = false;
        newPlayer.GetComponent<Player>().entity.combatCoroutine = false;
        newPlayer.GetComponent<PlayerController>().enabled = true;


        Destroy(this.gameObject);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expLeft)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentExp -= expLeft;
        entity.level++;

        entity.currentHealth = entity.maxHealth;

        float newExp = Mathf.Pow((float)expMod, entity.level);
        expLeft = (int)Mathf.FloorToInt((float)expBase * newExp);

        entity.entityAudio.PlayOneShot(levelUpSound);
        Instantiate(levelUpEffect, this.gameObject.transform);
    }

}
