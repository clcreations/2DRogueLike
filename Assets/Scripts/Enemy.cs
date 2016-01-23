using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public bool alive = true;

    Animator animator;
    Transform target;
    bool skipMove;

    protected override void Start(){
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove(int xDir, int yDir){
        if (skipMove){
            skipMove = false;
            return;
        }else if (!alive){
            return;
        }

        base.AttemptMove(xDir, yDir);
        skipMove = true;
    }

    public void Die(){
        alive = false;
        animator.SetTrigger("enemyDie");
    }

    public void MoveEnemy(){
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon){
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }else{
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove(xDir, yDir);
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
