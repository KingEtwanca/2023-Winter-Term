using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Skeleton_AI : MonoBehaviour
{

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public Transform SkeletonGFX;
    public Animator skeletonAnimator;

    public Transform AttackPoint;          //to check if play is in range          
    public Vector2 AttackRange= new Vector2(2,1);    
    public LayerMask PlayerLayer;
    public bool IsInRange = false;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        InvokeRepeating("RangeCheck", 0f, 3.2f);

    }

    void UpdatePath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }

    }

    void RangeCheck() {
        Collider2D hitplayer = Physics2D.OverlapCapsule(SkeletonGFX.position, AttackRange, CapsuleDirection2D.Horizontal , 0, PlayerLayer);
        if (hitplayer == true)
        {
            IsInRange = true;
            Attack1();
        }
        else {
            IsInRange = false;
        }
    }    

    void Attack1() {
        //play attack animation
        skeletonAnimator.SetTrigger("Attack1");


        //detect if player is in range

        //deal damage
        Debug.Log("You've been hit!");
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(AttackPoint.position- new Vector3(AttackRange.x/2,0,0),AttackPoint.position+ new Vector3(AttackRange.x / 2, 0, 0));
        Gizmos.DrawLine(AttackPoint.position - new Vector3(0, AttackRange.y / 2, 0), AttackPoint.position + new Vector3(0, AttackRange.y / 2, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        }   
        else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        if(rb.velocity.x >= 0.01f) {
            SkeletonGFX.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x <= 0.01f) {
            SkeletonGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
