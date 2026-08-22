using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public Animator anim;
    private Rigidbody2D rb;
    private Player player;

    [Header("Movement Settings")]
    public float inputX = 0f;
    public float inputY = 0f;
    private Vector2 movement = Vector2.zero;
    bool isMoving = false;

    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;
    bool canTeleport = false;
    Region tmpRegion;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        isMoving = (inputX != 0 || inputY != 0);
        movement = new Vector2(inputX, inputY);

        if (isMoving)
        {
            anim.SetFloat("Input_X", inputX);
            anim.SetFloat("Input_Y", inputY);
        }

        anim.SetBool("isWalking", isMoving);

        if(player.entity.attackTimer < 0)
            player.entity.attackTimer = 0;
        else
            player.entity.attackTimer -= Time.deltaTime;

        if (player.entity.attackTimer == 0 && !isMoving)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                anim.SetTrigger("Attack");
                player.entity.attackTimer = player.entity.cooldown;

                Attack();
            }
        }

        if(canTeleport && tmpRegion != null && Input.GetKeyDown(interactKey))
        {
            this.transform.position = tmpRegion.warpLocation.position;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * player.entity.speed * Time.fixedDeltaTime);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Enemy")
        {
            player.entity.target = collider.transform.gameObject;
        }

        if(collider.transform.tag == "Teleport")
        {
            canTeleport = true;
            tmpRegion = collider.GetComponent<Teleport>().region;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.tag == "Enemy")
        {
            player.entity.target = null;
        }

        if (collider.transform.tag == "Teleport")
        {
            canTeleport = false;
            tmpRegion = null;
        }
    }

    void Attack()
    {
        if(player.entity.target == null)
            return;

        Monster monster = player.entity.target.GetComponent<Monster>();

        if (monster.entity.dead)
        {
            player.entity.target = null;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.entity.target.transform.position);

        if (distance <= player.entity.attackDistance)
        {
            int dmg = player.gameManager.CalculateDamage(player.entity, player.entity.damage);
            int enemyDef = player.gameManager.CalculateDefense(monster.entity, monster.entity.defense);

            int result = dmg - enemyDef;

            if(result < 0)
                result = 0;

            Debug.Log("Seu ataque causou " + result + " de dano!");
            monster.entity.currentHealth -= result;
            monster.entity.target = this.gameObject;
        }
    }
}
