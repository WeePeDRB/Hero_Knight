using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonController : EnemyController
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

    // Update is called once per frame
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
    protected override void SpawnItems()
    {
        base.SpawnItems();
        int randomPotion = UnityEngine.Random.Range(0,10);
        if(randomPotion == 1){
            Instantiate(itemPrefabs[1],this.transform.position,itemPrefabs[1].transform.rotation );
        }
    }
}
