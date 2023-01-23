using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;

public class Skeleton_AI : MonoBehaviour
{

    public Transform target;


    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public Transform SkeletonGFX;
    public Animator skeletonAnimator;
    public int MaxHealth = 10;
    int currentHealth;

    public Transform AttackPoint;          //to check if player is in range          
    public Vector2 AttackRange= new Vector2(2,1);    
    public LayerMask PlayerLayer;
    public bool IsInRange = false;
    public bool IsAttacking = false;
    public bool canHit = false;
    private bool hitThisAttack = false;
    public bool Alive = true;
    public RoomManager CurrentRoom;


    //variables for getting hit by player
    public bool GotHitThisAttack = false;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        InvokeRepeating("RangeCheck", 0f, 0.01f);

    }

    void UpdatePath() {
        if (Alive) {
            if (seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }

    }

    void RangeCheck() {
        //check if player is in range
        if (Alive) {
            Collider2D hitplayer = Physics2D.OverlapCapsule(SkeletonGFX.position, AttackRange, CapsuleDirection2D.Horizontal, 0, PlayerLayer);
            if (hitplayer == true && !IsAttacking)
            {
                IsInRange = true;
                Attack1();
            }
            else
            {
                IsInRange = false;
            }
        }
    }  
    
    public void startAttack() { //fcn to start attack on animation event
        IsAttacking = true;
        rb.velocity = new Vector2(0,0);
        rb.isKinematic = true;
    }

    public void endAttack() { //fcn to end attack on animation event
        IsAttacking = false;
        rb.isKinematic = false;
    }

    public void startThrust() {
        rb.isKinematic = false;
        Vector2 direction = new Vector2(target.position.x - rb.position.x,target.position.y - rb.position.y);
        rb.AddForce(direction*Time.fixedDeltaTime*speed*5);

    }

    public void endThrust() {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    void Attack1() {

        //play attack animation
        skeletonAnimator.SetTrigger("Attack1");
    }

    public void ActivateHitbox() {
        canHit = true;
    }

    public void DeactivateHitbox() {
        canHit = false;
    }

    public void Die() {
        Alive = false;
        skeletonAnimator.SetTrigger("SkeletonDeath");
        CurrentRoom.activeEmemies--;
    }

    public void TakeDamage(int dmg) {
        if (Alive && !GotHitThisAttack) { 
            currentHealth -= dmg;
            GotHitThisAttack = true;
            Invoke("unGetHit",0.5f);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void GetHit() {
        if (!IsAttacking)  {
            skeletonAnimator.SetTrigger("GetHit");
        }
    }

    public void unGetHit() { 
        GotHitThisAttack = false;
    }
    

    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(AttackPoint.position- new Vector3(AttackRange.x/2,0,0),AttackPoint.position+ new Vector3(AttackRange.x / 2, 0, 0));
        Gizmos.DrawLine(AttackPoint.position - new Vector3(0, AttackRange.y / 2, 0), AttackPoint.position + new Vector3(0, AttackRange.y / 2, 0));
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(Alive) {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            if (canHit)
            {
                Collider2D hitplayer = Physics2D.OverlapCapsule(SkeletonGFX.position, AttackRange, CapsuleDirection2D.Horizontal, 0, PlayerLayer);
                if (hitplayer)
                {
                    canHit = false;
                    target.GetComponent<PlayerMovement>().GetHit(10);
                }
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            if (!IsAttacking)
            {
                rb.AddForce(force);
            }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }


            if (rb.velocity == Vector2.zero)
            {

            }
            else if (rb.velocity.x >= 0.01f)
            {
                SkeletonGFX.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (rb.velocity.x <= 0.01f)
            {
                SkeletonGFX.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}
