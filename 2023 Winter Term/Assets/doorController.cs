using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class doorController : MonoBehaviour
{
    public TilemapCollider2D door;
    private void Start()
    {
        door = GetComponent<TilemapCollider2D>();
    }
    public void DisableDoor() {
        Debug.Log("door disabled");
        gameObject.layer = LayerMask.NameToLayer("floor");
        door.enabled = false;

    }

   public void EnableDoor() {
        Debug.Log("Door enabled");
        this.gameObject.layer = LayerMask.NameToLayer("obstacles");
        door.enabled = true;
    }
}
