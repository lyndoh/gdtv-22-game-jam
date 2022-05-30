using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject titlePanel;
    [SerializeField]
    GameObject introPanel;

    void Start() {
        titlePanel.SetActive(true);
        introPanel.SetActive(false);
    }

    public void OnPlay() {
        titlePanel.SetActive(false);
        introPanel.SetActive(true);
    }

    public void OnStart() {
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuit() {
        Application.Quit();
    }
}
