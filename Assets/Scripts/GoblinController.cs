using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : EnemyController
{
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

    void Update()
    {
        if(!isDead && !playerIsDead){
            CheckPlayerStatus();
            Locate();
            Detect();
            Behavior();
            MovingToPlayer(distance,direction);
        }else if (isDead){
            Destroy(this.gameObject,1.5f);
        }
    }
}
