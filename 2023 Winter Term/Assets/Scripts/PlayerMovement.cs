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
    public int health = 100;
    public bool alive = true;
    public bool gettingHit = false;

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
        if(health <= 0 && alive) {
            Die();
        }
        //physics here

        if (!gettingHit) {
            player.MovePosition(player.position + movement * movementSpeed * Time.fixedDeltaTime);
        }



        if (movement == Vector2.zero)
        {
            startIdle();
        }
        else if (movement.x >= 0.01f && alive)
        {
            playerGFX.localScale = new Vector3(math.abs(playerGFX.localScale.x), playerGFX.localScale.y, 1f);
            startRun();
        }
        else if (movement.x <= 0.01f && alive)
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

    public void GetHit(int dmg)
    {
        if (alive)
        {
            gettingHit = true;
            playerAnim.SetTrigger("GetHit");
            health -= dmg;
            Debug.Log(health);
            Invoke("unstuck",0.5f);
        }
    }

    private void unstuck() {
        gettingHit = false;
    }

    public void Die() {
        alive = false;
        playerAnim.SetTrigger("Die");
        movementSpeed = 0f;
    }

    public void OnTriggerEnter2D(Collider2D other)  {
       if(other.tag == "exit") {
            Debug.Log("on Exit");
        }
    }

}
