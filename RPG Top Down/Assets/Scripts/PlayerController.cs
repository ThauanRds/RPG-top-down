using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public float inputX = 0f;
    public float inputY = 0f;
    public float speed = 2.5f;
    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        isMoving = (inputX != 0 || inputY != 0);

        if(isMoving)
        {
            var move = new Vector3(inputX, inputY, 0).normalized;
            transform.position += move * speed * Time.deltaTime;
            anim.SetFloat("Input_X", inputX);
            anim.SetFloat("Input_Y", inputY);
        }

        anim.SetBool("isWalking", isMoving);

        if(Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Attack");
        }
    }
}
