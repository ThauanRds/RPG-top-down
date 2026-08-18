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

        if(Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * player.entity.speed * Time.fixedDeltaTime);
    }
}
