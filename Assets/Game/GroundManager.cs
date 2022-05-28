using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    WormFoodManager wormFoodManager;
    [SerializeField]
    GameObject dirtPrefab;

    List<List<GameObject>> gameGrid;
    int feedingRow;
    bool pauseWhileClearing;

    public IEnumerable<GameObject> GetFeedingBlocks() {
        return gameGrid[feedingRow].Where(x => x != null && !x.GetComponent<Dirt>().isFinished);
    }

    public int GetRowsToClear() {
        return feedingRow;
    }

    public IEnumerable<GameObject> GetBlocks(int row) {
        return gameGrid[row].Where(x => x != null);
    }

    public void ClearBlock(int row, GameObject block) {
        var blockIndex = gameGrid[row].FindIndex(x => x == block);
        Debug.Log($"ClearBlock index {blockIndex}");
        gameGrid[row][blockIndex] = null;
        StartCoroutine(ClearBlockAnim(block));
    }

    public void PauseWhileClearing(bool pause) {
        this.pauseWhileClearing = pause;
    }

    void Start() {
        pauseWhileClearing = false;
        feedingRow = 0;
        gameGrid = new List<List<GameObject>>(23);
        for (int i = 0; i < 23; ++i) {
            var row = new List<GameObject>(13);
            for (int j = 0; j < 13; ++j) {
                row.Add(null);
            }
            gameGrid.Add(row);

        }
    }

    void Update() {
        if (pauseWhileClearing) {
            return;
        }

        for (int row = 0; row < 23; ++row) {
            var hasAny = false;
            var allFinished = true;
            for (int col = 0; col < 13; ++col) {
                var foodBlock = wormFoodManager.GetBlock(row, col);
                if (gameGrid[row][col] == null && foodBlock != null) {
                    StartCoroutine(AddDirt(row, col, wormFoodManager.GetBlock(row, col)));
                }
                hasAny = hasAny || foodBlock != null;
                allFinished = allFinished && 
                    (gameGrid[row][col] == null || gameGrid[row][col].GetComponent<Dirt>().isFinished);
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
        dirt.GetComponent<Dirt>().foodBlock = foodBlock;
        
        gameGrid[row][col] = dirt;

        while (scale < 1.0f) {
            if (pauseWhileClearing) {
                yield return null;
                continue;
            }
            if (row != feedingRow) {
                yield return null;
                continue;
            }
            scale += Time.deltaTime * 0.25f;
            dirt.transform.localScale = new Vector3(1, scale, 1);
            dirt.transform.position = dirtPos - new Vector3(0, (1 - scale) * 2, 0);
            yield return null;
        }
        dirt.transform.localScale = new Vector3(1, 1, 1);
        dirt.transform.position = dirtPos;
        dirt.GetComponent<Dirt>().isFinished = true;
    }

    IEnumerator ClearBlockAnim(GameObject block) {
        var target = new Vector3(-20, 55, 0);
        while (block.transform.position != target) {
            block.transform.position = Vector3.MoveTowards(block.transform.position, target, 100 * Time.deltaTime);
            yield return null;
        }
    }
}
