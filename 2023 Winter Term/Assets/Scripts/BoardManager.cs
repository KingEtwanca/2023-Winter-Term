using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random; 

public class BoardManager : MonoBehaviour
{
    [Serializable] //represents object's state as a byte stream
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int Columns = 8;
    public int Rows = 8;
    public Count wallCount = new Count(5, 9);
    //public Count item = new Count(5, 9);  potential health supplements for player in game
    public GameObject exit; //made this an array because we could potentially have more than one exit 
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] enemyTiles; //not sure if this means that the enemies will stand still, could serve as a spawn location for enemies
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>(); //use vector three for three dimensional position

    void initializeList ()
    {
        gridPositions.Clear();

        for (int x = 1; x < Columns - 1; x++) //we don't want to create completely impossible levels, so the outer floor tiles will be normal floortiles with no obstruction 
        {
            for (int y = 1; y < Rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); //creating a list of possible positions to place walls, enemies, or pickups 
            }
        }
    }

    void boardSetUp() //going to set up the board with an outer wall and outer floor tiles set up
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < Columns + 1; x++)
        {
            for (int y = -1; y < Rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; //choose floor tile at random from array
                if (x == -1 || x == Columns || y == -1 || y == Rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; //if it is at position of outer wall tiles, we want to choose an outer wall from the array

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //quaternion.identity eliminates rotation

                instance.transform.SetParent(boardHolder);
                
            }
        }
    }


    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int ObjectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < ObjectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate  (tileChoice, randomPosition, Quaternion.identity);
        }
    }


    public void SetupScene (int level)
    {
        boardSetUp();
        initializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        //LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f); //increases difficulty as the player ascends in level
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(Columns - 1, Rows - 1, 0F), Quaternion.identity);
    }


}
