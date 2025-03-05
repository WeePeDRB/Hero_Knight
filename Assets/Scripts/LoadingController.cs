using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public Animator sceneAnimator;
    void Update(){
        if(Input.GetKey(KeyCode.Space)){
            PlayGame();
        }
    }
    public void PlayGame() {
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene(){
        sceneAnimator.SetTrigger("Level End");
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("Dungeon");

    }
}
