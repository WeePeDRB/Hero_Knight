using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Components
    protected Animator enemyAnimator;
    protected SpriteRenderer enemySpr;
    [SerializeField] protected LayerMask player;

    //Audio
    [SerializeField] protected AudioSource deadSoundEffect;


    //Enemy stats
    [SerializeField] protected float enemyDamage;
    [SerializeField] protected float enemyMaxHealth;
    protected float enemyHealth;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected HealthBar attackTimerBar;


    //Enemy Attack
    [SerializeField] protected Transform attackPointRight;
    [SerializeField] protected Transform attackPointLeft;
    protected Vector3 attackPoint;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackRate;
    [SerializeField] protected float attackTimer;
    [SerializeField] protected float combatRange;
    
    //Enemy movement
    [SerializeField] protected float speed;

    //Detect player
    protected GameObject playerRef;
    [SerializeField] protected float detectRange;
    [SerializeField] protected GameObject detectPoint;
    protected float distance; //Distance between player and enemy
    protected float direction;
    protected bool arlert;
    protected bool detect;
    protected float detectTimer;

    //Enemy death
    protected bool isDead;
    [SerializeField] protected GameObject[] itemPrefabs;
    protected bool playerIsDead;

    void Awake() {
        //Get animator from game object
        enemyAnimator = GetComponent<Animator>();
        //Get sprite 
        enemySpr = GetComponent<SpriteRenderer>();
        //Find player 
        playerRef = GameObject.Find("Player");
        
        //Set enemy's current health equals to it's max health
        enemyHealth = enemyMaxHealth;

        //Set the healthbar equals to max health
        healthBar.SetMaxHealth(enemyMaxHealth);

        //Set up the attack timer bar
        attackTimerBar.SetMaxHealth(attackRate);
        attackTimerBar.SetHealth(attackTimer);
    }


    protected virtual void Locate(){
        distance = Vector2.Distance(playerRef.transform.position,transform.position);
        direction = playerRef.transform.position.x - transform.position.x;
    }

    protected virtual void Behavior(){
        if (!detect && arlert){
            detectTimer += Time.deltaTime;
            if(detectTimer >=10){
                arlert = false;
            }else return;
        }
    }

    protected virtual void Detect(){
        RaycastHit2D seePlayer = Physics2D.Raycast(detectPoint.transform.position,Vector2.right*direction,detectRange,player);
        if (seePlayer.collider != null){
            if(seePlayer.collider.name == "Player"){
                arlert = true;
                detect = true;
                detectTimer = 0;
            }
        }else {
            detect = false;
        }
    
    }
    public virtual void Hurt(float damge) {
        enemyAnimator.SetTrigger("Hurt");
        enemyHealth -= damge;
        healthBar.SetHealth(enemyHealth);
        attackTimer += 0.2f;
        if (enemyHealth <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        enemyAnimator.SetBool("Die", true);
        isDead = true;
        deadSoundEffect.Play();
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Collider2D>().enabled = false ;
        SpawnItems();
        foreach(Transform child in transform){
            child.gameObject.SetActive(false);
        }
    }


    protected void Attack(){
        if (enemySpr.flipX == false){
            attackPoint = attackPointRight.position;
        }else {
            attackPoint = attackPointLeft.position;
        }

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint,attackRange,player);
        if (hitPlayer == null) {
            return;
        }else {
            if(direction < 0){
                //Player on the left
                foreach( Collider2D hit in hitPlayer){
                    if(hit.gameObject.name == "Player"){
                        if(hit.gameObject.transform.GetChild(2).gameObject.activeInHierarchy == true){
                            hit.gameObject.GetComponent<PlayerController>().HitTheShield();
                        }else{
                            hit.gameObject.GetComponent<PlayerController>().Hurt(enemyDamage);
                        }
                    }
                }
            }else if (direction > 0){
                //Player on the right
                foreach( Collider2D hit in hitPlayer){
                    if(hit.gameObject.name == "Player"){
                        if(hit.gameObject.transform.GetChild(3).gameObject.activeInHierarchy == true){
                            hit.gameObject.GetComponent<PlayerController>().HitTheShield();
                        }else{
                            hit.gameObject.GetComponent<PlayerController>().Hurt(enemyDamage);
                        }
                    }
                }
            }
        }
    }


    protected virtual void MovingToPlayer(float distance, float direction){
        if(arlert){
            //Change direction for enemy 
            if(direction > 0 && attackTimer < (attackRate - 0.1f)){
                enemySpr.flipX = false;
            }else if(direction < 0 && attackTimer < (attackRate - 0.1f)) {
                enemySpr.flipX = true;
            }

            //Enemy follow player
            if(distance > combatRange) {
                enemyAnimator.SetBool("Walk",true);
                transform.position = Vector2.MoveTowards(transform.position,playerRef.transform.position,speed * Time.deltaTime);
            }else if (distance <= combatRange){
                attackTimer += Time.deltaTime;
                attackTimerBar.SetHealth(attackTimer);
                enemyAnimator.SetBool("Walk",false);
                if (attackTimer >= attackRate){
                    enemyAnimator.SetTrigger("Attack");
                    attackTimer = 0;
                }
            }
        }else if (arlert == false){
            enemyAnimator.SetBool("Walk",false);
        }    
    }

    protected virtual void SpawnItems(){
        //Spawn coin
        int randomCoin = UnityEngine.Random.Range(1,4);
        for(int i = 0; i < randomCoin; i++){
            Instantiate(itemPrefabs[0],this.transform.position,itemPrefabs[0].transform.rotation );
        }
    }

    public virtual void CheckPlayerStatus(){
        if(playerRef.gameObject.GetComponent<PlayerController>().isDead == true){
            playerIsDead = true;
        }
    }
    void OnDrawGizmosSelected () {
        Gizmos.DrawRay(detectPoint.transform.position,Vector2.right * detectRange *Math.Sign(direction));
        if (attackPointRight == null) return;
        Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
        if (attackPointLeft == null) return;
        Gizmos.DrawWireSphere(attackPointLeft.position,attackRange);
    }

}
