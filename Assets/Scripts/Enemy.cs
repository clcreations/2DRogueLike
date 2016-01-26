using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MovingObject {
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public bool alive = true;

    List<Vector2> goodDirs = new List<Vector2>();
    List<Vector2> badDirs = new List<Vector2>();
    Animator animator;
    Transform target;
    bool skipMove;

    protected override void Start(){
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    public void Die(){
        alive = false;
        animator.SetTrigger("enemyDie");
        // No longer prevent movement
        gameObject.layer = 0;
    }

    public void MoveEnemy(){
        if (skipMove){
            skipMove = false;
            return;
        }else if (!alive){
            return;
        }

        // Vector2(newX, newY) is the ideal movement direction
        int newX = Comparison(target.position.x, transform.position.x);
        int newY = Comparison(target.position.y, transform.position.y);
        AttemptMove(newX, newY);

        skipMove = true;
    }

    bool CheckMove(Vector2 checking){
        Vector2 start = transform.position;
        Vector2 end = start + checking;
        RaycastHit2D hit;

        GetComponent<BoxCollider2D>().enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        GetComponent<BoxCollider2D>().enabled = true;

        if (hit.transform == null){
            StartCoroutine(SmoothMovement(end));
            return true;
        }else if (hit.transform.GetComponent<Player>()){
            OnCantMove(hit.transform);
            return true;
        }
        return false;
    }


    void FindBestMove(){
        for(int i = 0; i < goodDirs.Count; i++){
            Vector2 temp = goodDirs[i];
            int randomIndex = Random.Range(i, goodDirs.Count);
            goodDirs[i] = goodDirs[randomIndex];
            goodDirs[randomIndex] = temp;
        }
        for(int i = 0; i < badDirs.Count; i++){
            Vector2 temp = badDirs[i];
            int randomIndex = Random.Range(i, badDirs.Count);
            badDirs[i] = badDirs[randomIndex];
            badDirs[randomIndex] = temp;
        }
        foreach(Vector2 dir in goodDirs){
            if (CheckMove(dir)) return;
        }
        foreach(Vector2 dir in badDirs){
            if (CheckMove(dir)) return;
        }
    }

    protected override void AttemptMove(int xDir, int yDir){
        goodDirs.Clear();
        badDirs.Clear();
        if (xDir == -1 && yDir == -1){
            goodDirs.Add(Vector2.down);
            goodDirs.Add(Vector2.left);
            badDirs.Add(Vector2.up);
            badDirs.Add(Vector2.right);
        }else if (xDir == 0 && yDir == -1){
            goodDirs.Add(Vector2.down);
            badDirs.Add(Vector2.left);
            badDirs.Add(Vector2.right);
        }else if (xDir == -1 && yDir == 0){
            goodDirs.Add(Vector2.left);
            badDirs.Add(Vector2.down);
            badDirs.Add(Vector2.up);
        }else if (xDir == 1 && yDir == 1){
            goodDirs.Add(Vector2.up);
            goodDirs.Add(Vector2.right);
            badDirs.Add(Vector2.left);
            badDirs.Add(Vector2.down);
        }else if (xDir == 1 && yDir == 0){
            goodDirs.Add(Vector2.right);
            badDirs.Add(Vector2.up);
            badDirs.Add(Vector2.down);
        }else if (xDir == 0 && yDir == 1){
            goodDirs.Add(Vector2.up);
            badDirs.Add(Vector2.up);
            badDirs.Add(Vector2.right);
        }else if (xDir == 1 && yDir == -1){
            goodDirs.Add(Vector2.right);
            goodDirs.Add(Vector2.down);
            badDirs.Add(Vector2.up);
            badDirs.Add(Vector2.left);
        }else if (xDir == -1 && yDir == 1){
            goodDirs.Add(Vector2.up);
            goodDirs.Add(Vector2.left);
            badDirs.Add(Vector2.right);
            badDirs.Add(Vector2.down);
        }
        FindBestMove();
    }

    public int Comparison(float a, float b){
        if (a > b) return 1;
        if (a < b) return -1;
        return 0;
        //if (a - b < float.Epsilon) return 1;
        //if (a - b > float.Epsilon) return -1;
        //return 0;
    }

    protected override void OnCantMove(Transform t){
        Player player = t.GetComponent<Player>();
        if (player){
            animator.SetTrigger("enemyAttack");
            SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

            player.LoseFood(playerDamage);
            GameManager.instance.ShakeScreen();
        }
    }

} // public class Enemy : MovingObject {
