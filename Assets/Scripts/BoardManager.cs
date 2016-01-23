using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count{
        public int minimum;
        public int maximum;

        public Count(int min, int max){
            minimum = min;
            maximum = max;
        }

    }

    public int columns = 14;
    public int rows = 8;

    public Count wallCount = new Count(12,20);
    public Count foodCount = new Count(1,5);
    public Count knifeCount = new Count(0,1);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] knifeTiles;

    // Organizational tool to child the many gameobjects we create
    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList(){
        gridPositions.Clear();
        // columns and rows - 1 is done to make sure the outer edges are always
        // passable. TODO making sure the level is passable could be done with
        // better logic and would no longer need columns and rows - 1
        for(int x = 1; x < columns - 1; x++){
            for(int y = 1; y < rows - 1; y++){
                gridPositions.Add(new Vector3(x,y,0f));
            }
        }
    }

    void BoardSetup(){
        boardHolder = new GameObject("Board").transform;
        // We build an edge around the gameboard of impassable terrain, which
        // is why we are using -1 to columns/rows + 1
        for(int x = -1; x < columns + 1; x++){
            for(int y = -1; y < rows + 1; y++){
                GameObject toInstatiate;
                if (x == -1 || x == columns || y == -1 || y == rows){
                    toInstatiate =
                        outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }else{
                    toInstatiate =
                        floorTiles[Random.Range(0, floorTiles.Length)];
                }
                GameObject instance = Instantiate(toInstatiate,
                    new Vector3(x,y,0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition(){
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++){
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice =
                tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level){
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        LayoutObjectAtRandom(
            knifeTiles, knifeCount.minimum, knifeCount.maximum);

        // Logarithmic difficulty curve
        // 1 enemy at 2, 2 at level 4, 4 at level 8, etc.
        int enemyCount = (int)Mathf.Log(level, 2f);

        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit,
            new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

    }

} // public class BoardManager : MonoBehaviour {
