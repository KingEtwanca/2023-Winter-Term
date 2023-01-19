using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Rigidbody2D player;
    public Transform playerGFX;
    public Animator playerAnim;

    Vector2 movement;


    // Update is called once per frame
    //generally not a good idea to do physics in update due to frame rates
    void Update()
    {
        //inputs here
        movement.x=Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

    }

    //do physics in here
    private void FixedUpdate()
    {
        //physics here
        
        player.MovePosition(player.position+movement*movementSpeed*Time.fixedDeltaTime);



        if (movement == Vector2.zero)
        {
            startIdle();
        }
        else if (movement.x >= 0.01f)
        {
            playerGFX.localScale = new Vector3(math.abs(playerGFX.localScale.x), playerGFX.localScale.y, 1f);
            startRun();
        }
        else if (movement.x <= 0.01f)
        {
            playerGFX.localScale = new Vector3(math.abs(playerGFX.localScale.x) * -1f, playerGFX.localScale.y, 1f);
            startRun();
        }
    }

    public void startRun() {
        playerAnim.SetTrigger("Run");
    }

    public void startIdle() {
        playerAnim.SetTrigger("Idle");
    }
}
