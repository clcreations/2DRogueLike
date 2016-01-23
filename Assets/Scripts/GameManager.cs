using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.2f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    public int knifeCount = 1;
    public int level = 1;
    [HideInInspector] public bool playersTurn = true;

    List<Enemy> enemies;
    bool enemiesMoving;
    Text levelText;
    Text knifeText;
    GameObject levelImage;
    CameraManager cam;
    bool doingSetup;
    float shake;

    const float SCREENSHAKE = 0.333f;

    void Awake(){
        if (instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void OnLevelWasLoaded(int index){
        // Complete hack because Unity 5.3.1 has a bug, and this fixes it
        // (bug only appears on build, runs fine in editor)
        if (Time.time > 6f) {

            level++;

            InitGame();
        }
    }

    public void GetKnife(GameObject gameObject){
        if (knifeCount < 3){
            knifeCount++;
            knifeText.text = knifeCount.ToString();
        }
        gameObject.SetActive(false);
    }

    public bool UseKnife(Enemy enemy){
        if (knifeCount > 0 && enemy.alive){
            knifeCount--;
            knifeText.text = knifeCount.ToString();
            return true;
        }
        return false;
    }

    void InitGame(){
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        knifeText = GameObject.Find("KnifeText").GetComponent<Text>();
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cam = camObject.GetComponent<CameraManager>();

        levelText.text = "Day " + level;
        knifeText.text = knifeCount.ToString();
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage(){
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver(){
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    void Update(){
        if (playersTurn || enemiesMoving || doingSetup){
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script){
        enemies.Add(script);
    }

    IEnumerator MoveEnemies(){
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        foreach(Enemy enemy in enemies){
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
    public void ShakeScreen(){
        cam.ShakeScreen(SCREENSHAKE, SCREENSHAKE);
    }

} // public class GameManager : MonoBehaviour {
