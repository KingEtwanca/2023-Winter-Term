using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //this script will run the game
    public BoardManager boardScript;

    public int level = 3;

    // Start is called before the first frame update
    void Awake()
    {
        boardScript = GetComponent<BoardManager>();
        initGame(); 
    }

    void initGame()
    {
        boardScript.SetupScene(level);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
