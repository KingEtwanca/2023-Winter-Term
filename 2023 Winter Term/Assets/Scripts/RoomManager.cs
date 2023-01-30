using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public GameObject doors;
    public bool IsActive = true;
    public GameObject SkeletonEnemy;
    public GameObject[] enemies;
    public int activeEmemies;
    public GameObject player;
    public int round = 0;


    // Start is called before the first frame update
    void Start()
    {
        activeEmemies = enemies.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (activeEmemies == 0) {
            DestroyDoors();
        }

        if (player.GetComponent<PlayerMovement>().OnExit == true) {
            NextRound();
        }

    }

    public void DestroyDoors() {
        doors.GetComponent<doorController>().DisableDoor();
    }

    public void NextRound() {
        round++;
        doors.GetComponent<doorController>().EnableDoor();
        player.GetComponent<PlayerMovement>().ResetPlayer();

        //make new array of enemies
        GameObject[] EnemiesPlusOne = new GameObject[enemies.Length+1];
        for(int i = 0; i < enemies.Length; i++) {
            EnemiesPlusOne[i] = enemies[i];
        }

        //spawn new skeleton
        EnemiesPlusOne[enemies.Length] = Instantiate(SkeletonEnemy, Vector3.zero, Quaternion.identity);

        enemies = EnemiesPlusOne;

        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<Skeleton_AI>().SetPosition(-20,Random.Range(-20,20));
            enemy.GetComponent<Skeleton_AI>().ResetSkeleton(this.gameObject);
            enemy.GetComponent<Skeleton_AI>().SetTarget(player);
        }
        activeEmemies = enemies.Length;
    }
}
