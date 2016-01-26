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
    public AudioClip zombieDie1;
    public AudioClip zombieDie2;


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
            AttemptMove(h, v);
        }
    }

    private void OnTriggerEnter2D (Collider2D other){
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
        }else if (other.tag == "Knife"){
            GameManager.instance.GetKnife(other.gameObject);
        }

    }

    protected override void OnCantMove(Transform t){
        Wall wall = t.GetComponent<Wall>();
        Enemy enemy = t.GetComponent<Enemy>();
        if (wall){
            Debug.Log("Running into Wall");
            wall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");
        }else if(enemy){
            Debug.Log("Running into Enemy");
                if (GameManager.instance.UseKnife(enemy)){
                    enemy.Die();
                    animator.SetTrigger("playerStab");
                    SoundManager.instance.PrioritySfx(zombieDie1, zombieDie2);
                }
        }

    }

    private void Restart(){
        SceneManager.LoadScene("Main");
    }

    public void LoseFood(int loss){
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    protected override void AttemptMove(int xDir, int yDir){
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove(xDir, yDir);

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
