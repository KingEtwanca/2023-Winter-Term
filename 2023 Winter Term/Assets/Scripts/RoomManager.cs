using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RoomManager : MonoBehaviour
{
    public GameObject doors;
    public bool IsActive = true;
    public GameObject[] enemies;
    public int activeEmemies;

    // Start is called before the first frame update
    void Start()
    {
        activeEmemies = enemies.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(activeEmemies == 0) {
            DestroyDoors();
        }
    }

    public void DestroyDoors() {
        Destroy(doors);
    }
}
