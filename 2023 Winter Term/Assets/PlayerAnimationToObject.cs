using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationToObject : MonoBehaviour
{
    public GameObject player;

    public void startAttack() {
        player.GetComponent<PlayerMovement>().activateHitbox();
    }

    public void endAttack() {
        player.GetComponent<PlayerMovement>().deactivateHitbox();
    }
   
}
