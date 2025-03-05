using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.gameObject.CompareTag("Player")){
            collider2D.gameObject.GetComponent<PlayerController>().Potion++;
            Destroy(this.gameObject);
        }
    }
}
