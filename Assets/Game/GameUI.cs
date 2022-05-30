using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    ScoreBoard scoreBoard;

    [SerializeField]
    WormFoodManager wormFoodManager;
    [SerializeField]
    GameObject finishedPanel;
    [SerializeField]
    TextMeshProUGUI scoreText;

    public void OnQuit() {
        SceneManager.LoadScene("MenuScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        finishedPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (wormFoodManager.gameOver) {
            finishedPanel.SetActive(true);
            scoreText.text = $"Score: {scoreBoard.GetScore()}";
        }
    }
}
