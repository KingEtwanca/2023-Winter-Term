using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Rigidbody2D player;

    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
    }
}
