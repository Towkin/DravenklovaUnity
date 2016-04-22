using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class LoadingScript : MonoBehaviour {
    void Start () {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}
}
