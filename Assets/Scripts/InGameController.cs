using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject playerRef;
    public GameObject gameOverText;
    public Animator sceneAnimator;
    public bool gameOver;
    public bool isOpenMenu;

    void Update()
    {
        OpenMenu();
        MenuControl();
        CheckPlayerStatus();
    }
    public void OpenMenu(){
        if(Input.GetKeyDown(KeyCode.Escape) && !gameOver){
            if(isOpenMenu == false){
                isOpenMenu = true;
                playerRef.gameObject.GetComponent<PlayerController>().isOnMenu = true;
                menuPanel.SetActive(true);
            }else if (isOpenMenu == true){
                isOpenMenu = false;
                playerRef.gameObject.GetComponent<PlayerController>().isOnMenu = false;
                menuPanel.SetActive(false);
            }
        }
    }
    
    public void CheckPlayerStatus(){
        if(playerRef.gameObject.GetComponent<PlayerController>().isDead == true){
            gameOverText.SetActive(true);
            menuPanel.SetActive(true);
            gameOver = true;
        }
    }

    public void MenuControl(){
        if(isOpenMenu == true){
            if(Input.GetKeyDown(KeyCode.Q)){
                ToLoadingScene();
            }
            if(Input.GetKeyDown(KeyCode.W)){
                ToMenuScene();
            }
            if(Input.GetKeyDown(KeyCode.E)){
                RestartScene();
            }
        }
    }

    public void ToMenuScene(){
        SceneManager.LoadScene("Menu");
    }

    public void ToLoadingScene(){
        SceneManager.LoadScene("Loading");
    }

    public void RestartScene(){
        SceneManager.LoadScene("Dungeon");
    }
}
