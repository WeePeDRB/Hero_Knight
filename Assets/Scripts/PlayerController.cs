using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //Components
    protected Rigidbody2D playerRb;
    protected BoxCollider2D playerCollider;
    protected SpriteRenderer playerSpr;
    protected Animator playerAnimator;

    //Audio
    [SerializeField] protected AudioSource attackSoundEffect;
    [SerializeField] protected AudioSource hitSoundEffect;
    [SerializeField] protected AudioSource blockSoundEffect;
    [SerializeField] protected AudioSource jumpSoundEffect;
    [SerializeField] protected AudioSource landingSoundEffect;
    [SerializeField] protected AudioSource healUpSoundEffect;
    [SerializeField] protected AudioSource attackBuffSoundEffect;
    [SerializeField] protected AudioSource speedBuffSoundEffect;




    //Player Stats
    [SerializeField] protected float playerMaxHealth;
    protected float playerHealth;
    [SerializeField] protected HealthBar healthBar;

    //Player Movement
    protected float horizontalInput; //Player input 
    [SerializeField] protected float playerSpeed; //Player speed
    [SerializeField] protected float playerJumpForce; //Jump force
    protected string Walk_Animation = "Walk"; //Condition for walking in animation
    protected string Is_Ground = "Grounded"; //Condition on ground for player
    protected bool isGrounded; //Bool value to check if player on ground (For jump function)
    
    //Items
    protected int gold = 0;
    public int Gold{
        get {return gold;}
        set {gold = value;}
    }
    [SerializeField] protected Text coinText;
    protected int potion = 1;
    public int Potion{
        get {return potion;}
        set {potion = value;}
    }
    [SerializeField] protected Text potionText;

    protected float itemRate;
    

    //Player Attack
    [SerializeField] protected Transform attackPointRight; //Right point 
    [SerializeField] protected Transform attackPointLeft; //Left point
    protected Vector3 attackPoint; //Attack point base on player direction
    [SerializeField] protected float attackRange;
    protected float attackRate;
    [SerializeField] protected float attackMaxRate;
    [SerializeField] protected float playerDamge;
    protected int attackCounter = 1; //Help with the attack animation
    [SerializeField] protected LayerMask enemyLayer; 
    protected string Attack_Animation = "Attack";
    protected string Attack_Counter = "AttackCounter";

    //Player Defence
    protected bool isBlocking;

    //Purchase
    protected bool isPurchase;
    public bool IsPurchase{
        get {return isPurchase;}
        set {isPurchase = value;}
    }

    public bool isOnMenu;
    public bool isDead;

    void Awake(){
        playerRb        =   GetComponent<Rigidbody2D>();
        playerSpr       =   GetComponent<SpriteRenderer>();
        playerAnimator  =   GetComponent<Animator>();
        playerCollider  =   GetComponent<BoxCollider2D>();
        playerHealth = playerMaxHealth;
        healthBar.SetMaxHealth(playerMaxHealth);
    }

    void Start()
    {
        
    }

    void Update()
    {
        attackRate += Time.deltaTime;
        itemRate += Time.deltaTime;

        PlayerBehavior();

    }

    
    protected void PlayerBehavior(){
        if(!isDead){
            GetPotion();
            GetCoin();
            PlayerAttack();
            PlayerMove();
            PlayerJump();
            PlayerDefence();
            UsePotion();
            Purchase();
        }
    }

    //Player Movement
    protected void PlayerMove(){
        //Get player's input
        horizontalInput = Input.GetAxisRaw("Horizontal");        
        //Check if player is moving
        if(horizontalInput != 0 && !isBlocking){
            //Player move
            transform.Translate(Vector3.right * horizontalInput * playerSpeed * Time.deltaTime);
            //Set walk animation for player
            playerAnimator.SetBool(Walk_Animation,true);
            //Flip player sprite according to their direction 
            if(horizontalInput > 0) playerSpr.flipX = false;
            else if (horizontalInput < 0) playerSpr.flipX = true;
        }
        //Turn to idle animation when player stay still
        else if (horizontalInput == 0) playerAnimator.SetBool(Walk_Animation,false);
  
    }

   
        
    

    protected void PlayerJump(){
        //Condtion to jump
        if(Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !isBlocking){
            isGrounded = false;
            //Add a force with rigidbody
            playerRb.AddForce(new Vector2(0f,playerJumpForce),ForceMode2D.Impulse);
            jumpSoundEffect.Play();
            //Set the ground condition to false
            playerAnimator.SetBool(Is_Ground,false);
        }

    }


    //Player Attack
    protected void PlayerAttack(){
        if(attackRate >= attackMaxRate){
            if(Input.GetKeyDown(KeyCode.A) && isGrounded && horizontalInput == 0){
                if (attackCounter == 3){
                    attackCounter = 1;
                }
                else attackCounter ++;
                attackRate = 0;

                //Run the attack animation
                playerAnimator.SetTrigger(Attack_Animation); 
                attackSoundEffect.Play();
                playerAnimator.SetInteger(Attack_Counter,attackCounter);
                //Check which direction player is facing
                if (playerSpr.flipX == false){
                    //Facing right
                    attackPoint = attackPointRight.position;
                }else {
                    //Facing left
                    attackPoint = attackPointLeft.position;
                }

                //Create a list of enemy that get hit by player
                Collider2D [] hitEnemes = Physics2D.OverlapCircleAll(attackPoint,attackRange,enemyLayer);
                //For each enemeis in the list call the Hurt() function 
                foreach(Collider2D enemy in hitEnemes){ 
                    enemy.GetComponent<EnemyController>().Hurt(playerDamge);
                }
            }
        }
    }


    //Player Defence
    protected void PlayerDefence(){
        if(Input.GetKey(KeyCode.S) && isGrounded && horizontalInput==0){
            if(playerSpr.flipX == true){
                this.gameObject.transform.GetChild(3).gameObject.SetActive(true);
            }else{
                this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
            }
            playerAnimator.SetBool("Block",true); 
            isBlocking = true;
        }
        else if (Input.GetKeyUp(KeyCode.S)){
            if(playerSpr.flipX == true){
                this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
            }else{
                this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
            }
            isBlocking = false;
            playerAnimator.SetBool("Block",false);
        }
    }

    public void Hurt(float damage){
        playerHealth -= damage;
        if(playerHealth <= 0f){
            Die();
        }
        healthBar.SetHealth(playerHealth);
        hitSoundEffect.Play();
        playerAnimator.SetTrigger("Hurt");
    }

    public void Die(){
        playerAnimator.SetBool("Die",true);
        playerRb.isKinematic = true;
        playerCollider.enabled = false;
        isDead = true;
    }
    
    public void HitTheShield(){
        blockSoundEffect.Play();
        playerAnimator.SetTrigger("HitTheShield");
    }

    void GetCoin(){
        coinText.text = Gold.ToString();
    }

    void GetPotion(){
        potionText.text = Potion.ToString();
    }

    void UsePotion(){
        if(isPurchase == false && isOnMenu == false){
            if(Input.GetKey(KeyCode.W) && isGrounded && horizontalInput == 0 && itemRate >= 4f){
                playerHealth += 50f;
                if(playerHealth >= playerMaxHealth){
                    playerHealth = playerMaxHealth;
                }
                healthBar.SetHealth(playerHealth);
                healUpSoundEffect.Play();
                Potion --;
                itemRate = 0;
            }
        }
    }

    void Purchase(){
        if(isPurchase == true && isOnMenu == false){
            if(Input.GetKeyDown(KeyCode.Q)){
                if(Gold >= 10){
                    itemRate = 0;
                    //Increase potion quantity
                    Potion ++;
                    healUpSoundEffect.Play();

                    //Decrease money
                    Gold -= 10;
                }else return;
            }
            else if(Input.GetKeyDown(KeyCode.W) && itemRate >= 3){
                if (Gold >= 25){
                    itemRate = 0;
                    //Increase max health
                    playerMaxHealth += 50;
                    playerHealth += 50;
                    healthBar.SetMaxHealth(playerMaxHealth);
                    healthBar.SetHealth(playerHealth);

                    //Increase player damage
                    playerDamge += 5;
                    
                    attackBuffSoundEffect.Play();
                    Gold -= 25;
                }else return;
            }
            else if(Input.GetKeyDown(KeyCode.E) && itemRate >= 3){
                if (Gold >= 25){
                    itemRate = 0;
                    //Increase attack speed
                    if(attackMaxRate > 0.3){
                        attackMaxRate -= 0.05f;
                    }
                    if (playerSpeed < 14){
                        playerSpeed += 0.5f;
                    }
                    //Increase speed

                    speedBuffSoundEffect.Play();
                    Gold -= 25;
                } else return;
            }
        }
    }

 
    private void OnCollisionEnter2D(Collision2D collision){
        //Check if player is on ground
        if(collision.gameObject.CompareTag("Ground")){
            if(isGrounded == false)landingSoundEffect.Play();
            isGrounded = true;
            playerAnimator.SetBool(Is_Ground,true);
        }
    }

    void OnDrawGizmosSelected () {
        if (attackPointRight == null) return;
        Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
        if (attackPointLeft == null) return;
        Gizmos.DrawWireSphere(attackPointLeft.position,attackRange);
    }
}
