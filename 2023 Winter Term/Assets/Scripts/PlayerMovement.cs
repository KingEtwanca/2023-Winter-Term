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

    public Transform AttackPoint;
    public float AttackRange = 0.8f;
    public LayerMask EnemyLayers;

    //variables for attacking
    public bool isAttacking = false;

    Vector2 movement;

    public bool OnExit = false;


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

        // detect attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        //if the player is attacking 
        if (isAttacking) {
            //detect enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, EnemyLayers);


            //deal damage
            foreach (Collider2D enemy in hitenemies)
            {
                if (enemy.GetComponent<Skeleton_AI>().GotHitThisAttack == false)
                {
                    enemy.GetComponent<Skeleton_AI>().TakeDamage(4);
                }
            }
        }


        if (movement == Vector2.zero)
        {
            startIdle();
        }

        else if (movement.x >= 0.01f && alive)
        {
            player.transform.localScale = new Vector3(math.abs(player.transform.localScale.x), player.transform.localScale.y, 1f);
            startRun();
        }
        else if (movement.x <= 0.01f && alive)
        {
            player.transform.localScale = new Vector3(math.abs(player.transform.localScale.x) * -1f, player.transform.localScale.y, 1f);
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
            OnExit = true;
        }
    }

    //attack function
    public void Attack() {
        //play animation
        playerAnim.SetTrigger("Attack");

    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    public void activateHitbox() {
        isAttacking = true;
    }

    public void deactivateHitbox() {
        isAttacking = false;
    }

    public void ResetPlayer() {
        player.transform.position = Vector2.zero;
        OnExit = false;
    }
}
