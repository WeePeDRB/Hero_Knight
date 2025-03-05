using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{

    public GameObject[] enemyPrefabs;
    private int countEnemy; 
    private int enemyQuantity;
    private float repareTimer;
    private int level;
    public bool inRepareTime;

    [SerializeField] private Text levelText;
    [SerializeField] private Text countDownText;
    [SerializeField] private Text countDownTextBorder;
    [SerializeField] private Text enemyCountText;

    // Start is called before the first frame update
    void Start()
    {
        countDownTextBorder.gameObject.SetActive(false);
        level = 0;
        enemyQuantity = 1;
        repareTimer = 30;
    }

    // Update is called once per frame
    void Update()
    {
        countEnemy = GameObject.FindGameObjectsWithTag("Enemy").Length;
        GetEnemyCount();
        NextWave();
        GetLevel();
        SkipRepareTime();
    }

    void Spawn(int enemiesToSpawn){
        //int index = Random.Range(0, enemyPrefab.Length);
        for(int i=0,y; i<enemiesToSpawn; i++){
            y = Random.Range(0,enemyPrefabs.Length);
            Instantiate(enemyPrefabs[y],GenerateSpawn(),enemyPrefabs[y].transform.rotation);
        }
    }
    
    void NextWave(){
        if (countEnemy == 0){
            if(repareTimer >= 0){
                inRepareTime = true;
                GetCountDown();
                repareTimer -= Time.deltaTime;
            }else if(repareTimer <= 0f){
                enemyQuantity ++;
                level++;
                Spawn(enemyQuantity);
                repareTimer = 30f;
                inRepareTime = false;
                countDownTextBorder.gameObject.SetActive(false);
            }
        }
    }
    Vector3 GenerateSpawn(){
        float spawnPosX = Random.Range(40, 90);
        Vector3 randomPos = new Vector3(spawnPosX,2f); 
        return randomPos;
    }
    void GetCountDown(){
        countDownTextBorder.gameObject.SetActive(true);
        countDownText.text = repareTimer.ToString("0");
    }
    void GetLevel(){
        levelText.text = level.ToString();
    }

    void GetEnemyCount(){
        enemyCountText.text = countEnemy.ToString();
    }

    void SkipRepareTime(){
        if(inRepareTime){
            if(Input.GetKey(KeyCode.P)){
                repareTimer = 0;
            }
        }
    }
}
