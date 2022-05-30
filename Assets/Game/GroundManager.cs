using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    WormFoodManager wormFoodManager;
    [SerializeField]
    GameGrid gameGrid;
    [SerializeField]
    GameObject dirtPrefab;

    
    int feedingRow;
    bool pauseWhileClearing;
    float eatingSpeed;

    public IEnumerable<GameObject> GetFeedingBlocks() {
        return gameGrid.GetRowDirt(feedingRow).Where(x => x != null && !x.GetComponent<Dirt>().isFinished);
    }

    public int GetRowsToClear() {
        return feedingRow;
    }

    public IEnumerable<GameObject> GetBlocks(int row) {
        return gameGrid.GetRowDirt(row).Where(x => x != null);
    }

    public void PauseWhileClearing(bool pause) {
        this.pauseWhileClearing = pause;
    }

    public void SetEatingSpeed(float speed) {
        eatingSpeed = speed;
    }

    void Start() {
        pauseWhileClearing = false;
        feedingRow = 0;
        eatingSpeed = 0.25f; // 0.25 blocks per second
    }

    void Update() {
        if (pauseWhileClearing) {
            return;
        }

        for (int row = 0; row < 23; ++row) {
            var hasAny = false;
            var allFinished = true;
            for (int col = 0; col < 13; ++col) {
                var foodBlock = gameGrid.GetFood(row, col);
                if (!gameGrid.HasDirt(row, col) && gameGrid.HasFood(row, col)) {
                    StartCoroutine(AddDirt(row, col, gameGrid.GetFood(row, col)));
                }
                hasAny = hasAny || gameGrid.HasFood(row, col);
                allFinished = allFinished && 
                    (!gameGrid.HasDirt(row, col) || gameGrid.GetDirt(row, col).GetComponent<Dirt>().isFinished);
            }
            if (!allFinished || !hasAny) {
                feedingRow = row;
                break;
            }
        }
    }

    IEnumerator AddDirt(int row, int col, GameObject foodBlock) {
        var scale = 0.01f;
        var dirtPos = new Vector3((col * 4) - 24, row * 4 - 44, 0);
        var dirt = Instantiate(dirtPrefab, dirtPos, Quaternion.identity);
        dirt.transform.localScale = new Vector3(1, scale, 1);
        dirt.GetComponent<Dirt>().foodBlock = foodBlock;
        
        gameGrid.AddDirt(row, col, dirt);

        while (scale < 1.0f) {
            if (pauseWhileClearing) {
                yield return null;
                continue;
            }
            // Check if block is in the feeding row. Need to get this dynamically as
            // The blocks row may change after line clears. "row" is not reliable after we yield
            // Same with dirtpos, needs to be recalculated in case row changes
            if (gameGrid.GetDirt(feedingRow, col) != dirt) {
                yield return null;
                continue;
            }
            dirtPos = new Vector3((col * 4) - 24, feedingRow * 4 - 44, 0);
            scale += Time.deltaTime * eatingSpeed;
            if (scale >= 1.0f) {
                dirt.transform.localScale = new Vector3(1, 1, 1);
                dirt.transform.position = dirtPos;
            } else {
                dirt.transform.localScale = new Vector3(1, scale, 1);
                dirt.transform.position = dirtPos - new Vector3(0, (1 - scale) * 2, 0);
                yield return null;
            }
        }
        dirt.GetComponent<Dirt>().isFinished = true;
    }

}
