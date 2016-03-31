using UnityEngine;
using System.Collections;

public class LoadingScript : MonoBehaviour {
    void Start () {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}
}
