using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("Controller")]
    public Entity entity;
    public GameManager manager;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Patrol")]
    public List<Transform> wayPointList;
    public float arrivalDistance = 0.5f;
    public float waitTime = 5;
    public int waypointID;

    Transform targetWaypoint; // ponto alvo
    int currentWaypoint = 0; // ponto atual
    float lastDistanceToTarget = 0f; // distancia do ultimo ponto
    float currentWaitTime = 0f;  // Tempo de espera

    [Header("Experience Reward")]
    public int rewardExperience = 10;
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;

    [Header("Respawn")]
    public GameObject prefab;
    public bool respawn = true;
    public float respawnTime = 10f;

    [Header("UI")]
    public Slider healthSlider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);   

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            int ID = obj.GetComponent<WaypointID>().ID;
            if(ID == waypointID)
            {
                wayPointList.Add(obj.transform);
            }
        }

        currentWaitTime = waitTime;
        if (wayPointList.Count > 0)
        {
            targetWaypoint = wayPointList[currentWaypoint];
            lastDistanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);
        }
    }

    void Update()
    {
        if(entity.dead)
            return;

        if(entity.currentHealth <= 0)
        {
            entity.currentHealth = 0;
            // Lógica de morte do monstro
            Die();
        }

        healthSlider.value = entity.currentHealth;

        if (!entity.inCombat)
        {
            if (wayPointList.Count > 0)
            {
                // Lógica de patrulha ou comportamento não em combate
                Patrol();
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }
        else
        {
            if (entity.attackTimer > 0)
                entity.attackTimer -= Time.deltaTime;

            if (entity.attackTimer < 0)
                entity.attackTimer = 0;

            if (entity.target != null && entity.inCombat)
            {
                if (!entity.combatCoroutine)
                {
                    // Corrotina de combate
                    StartCoroutine(Attack());
                }
            }
            else
            {
                entity.combatCoroutine = false;
                // Para a corrotina de combate
                StopCoroutine(Attack());
            }
        }

    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.tag == "Player" && !entity.dead)
        {
            entity.inCombat = true;
            entity.target = collider.gameObject;
            entity.target.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            entity.inCombat = false;

            if (entity.target)
            {
                entity.target.GetComponent<BoxCollider2D>().isTrigger = false;
                entity.target = null;
            }
        }
    }

    void Patrol()
    {
        if(entity.dead)
            return;

        float distanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);

        if(distanceToTarget <= arrivalDistance || distanceToTarget >= lastDistanceToTarget)
        {
            anim.SetBool("isWalking", false);

            if(currentWaitTime <= 0)
            {
                currentWaypoint++;

                if(currentWaypoint >= wayPointList.Count)
                    currentWaypoint = 0;

                targetWaypoint = wayPointList[currentWaypoint];
                lastDistanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);

                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            anim.SetBool("isWalking", true);
            lastDistanceToTarget = distanceToTarget;
        }

        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        anim.SetFloat("Input_X", direction.x);
        anim.SetFloat("Input_Y", direction.y);

        rb.MovePosition(rb.position + direction * (entity.speed * Time.fixedDeltaTime));
    }

    IEnumerator Attack()
    {
        entity.combatCoroutine = true;

        while (true)
        {
            yield return new WaitForSeconds(entity.cooldown);

            if (entity.target != null && !entity.target.GetComponent<Player>().entity.dead)
            {
                anim.SetBool("Attack", true);

                float distance = Vector2.Distance(entity.target.transform.position, transform.position);

                if(distance <= entity.attackDistance)
                {
                    int dmg = manager.CalculateDamage(entity, entity.damage);
                    int targetDefense = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense);
                    int dmgResult = dmg - targetDefense;

                    if(dmgResult < 0)
                        dmgResult = 0;

                    Debug.Log("Dano causado: " + dmgResult);
                    entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult;
                }
            }
        }
    }

    void Die()
    {
        entity.dead = true;
        entity.inCombat = false;
        entity.target = null;

        anim.SetBool("isWalking", false);

        // Adiciona a experiência ao jogador
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.GainExp(rewardExperience);

        Debug.Log("O monstro morreu!" + entity.name);

        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        
        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation, null);
        newMonster.name = prefab.name;
        newMonster.GetComponent<Monster>().entity.dead = false;
        newMonster.GetComponent<Monster>().entity.combatCoroutine = false;

        Destroy(this.gameObject);
    }
}
