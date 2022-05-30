using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    int dirtBlocksCleared = 0;
    int score = 0;

    public void ScoreBlock() {
        ++dirtBlocksCleared;
        score += 10;

        GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public int GetScore() {
        return score;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dirtBlocksCleared = 0;
        score = 0;

        GetComponent<TextMeshProUGUI>().text = "0";
    }

}
