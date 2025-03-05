using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SalesMan : MonoBehaviour
{
    protected bool inRange;
    protected bool inRepareTime;
    protected bool checkRepareTime;

    //Dialouge
    [SerializeField] protected TextMeshProUGUI instruct;
    [SerializeField] protected GameObject panel;
    [SerializeField] protected GameObject playerRef;
    [SerializeField] protected GameObject spawnManager;
    void Start()
    {
        
    }

    void Update()
    {
        CheckRepareTime();
        Interact();
    }

    public void Interact(){
        if(inRange){
            if(Input.GetKey(KeyCode.I)){
                panel.SetActive(true);
                playerRef.GetComponent<PlayerController>().IsPurchase = true;
            }
        }
    }

    public void CheckRepareTime(){
        checkRepareTime = spawnManager.GetComponent<SpawnManager>().inRepareTime;
        if( checkRepareTime == true){
            inRepareTime = true;
        }else inRepareTime = false;
    }
    public void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.CompareTag("Player") && inRepareTime == true){
            inRange = true;
            instruct.gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collider){
        if(collider.gameObject.CompareTag("Player")){
            inRange = false;
            instruct.gameObject.SetActive(false);
            panel.SetActive(false);
            playerRef.GetComponent<PlayerController>().IsPurchase = false;
        }
    }
}
