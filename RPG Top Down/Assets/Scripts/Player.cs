using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("Você precisa atribuir o GameManager no inspetor!");
            return;
        }

        entity.maxHealth = gameManager.CalculateHealth(this);
        entity.maxMana = gameManager.CalculateMana(this);
        entity.maxStamina = gameManager.CalculateStamina(this);

        int dmg = gameManager.CalculateDamage(this, 10);  // Usado no jogador
        int def = gameManager.CalculateDefense(this, 5);  // Usado no inimigo

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        manaSlider.maxValue = entity.maxMana;
        manaSlider.value = manaSlider.maxValue;

        staminaSlider.maxValue = entity.maxStamina;
        staminaSlider.value = staminaSlider.maxValue;

        expSlider.value = 0;

        // Testando a regeneração de vida do jogador
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
    }

    void Update()
    {
        healthSlider.value = entity.currentHealth;
        manaSlider.value = entity.currentMana;
        staminaSlider.value = entity.currentStamina;

        // Teste de vida e mana
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.currentHealth -= 10;
            entity.currentMana -= 5;
        }
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

}
