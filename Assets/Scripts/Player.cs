using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;


public class Player : MovingObject {
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;

    protected override void Start(){
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;

        base.Start();
    }

    private void OnDisable(){
        GameManager.instance.playerFoodPoints = food;
    }

    void Update() {
        if (!GameManager.instance.playersTurn) return;

        int h = 0;
        int v = 0;
        h = (int) CrossPlatformInputManager.GetAxisRaw("Horizontal");
        v = (int) CrossPlatformInputManager.GetAxisRaw("Vertical");
        if (h != 0) v = 0;
        if (!(h == 0 && v == 0)){
            AttemptMove<Wall>(h, v);
        }

        //horizontal = (int) Input.GetAxisRaw("Horizontal");
        //vertical = (int) Input.GetAxisRaw("Vertical");

        //if (horizontal != 0) vertical = 0;
        //if (horizontal != 0 || vertical != 0){
        //    AttemptMove<Wall>(horizontal, vertical);
        //}
        // DEBUG
        //Debug.Log("Food is " + food);
    }

    private void OnTriggerEnter2D (Collider2D other){
        Debug.Log("Collided with trigger " + other.name);
        if (other.tag == "Exit"){
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if (other.tag == "Food"){
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }else if (other.tag == "Soda"){
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }

    }

    protected override void OnCantMove<T>(T component){
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart(){
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene("Main");
        
    }

    public void LoseFood(int loss){
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir){
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move (xDir, yDir, out hit)){
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void CheckIfGameOver(){
        if (food <= 0){
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }



} // public class Player : MovingObject {
