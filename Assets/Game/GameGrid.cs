using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField]
    GameObject singlePrefab;
    [SerializeField]
    ScoreBoard scoreBoard;
    [SerializeField]
    WormManager wormManager;

    List<List<GameObject>> gameGridFood;
    List<List<GameObject>> gameGridDirt;

    public int GetRowCount() {
        return gameGridFood.Count;
    }

    public int GetColumnCount() {
        return gameGridFood[0].Count;
    }

    public List<GameObject> GetRowFood(int row) {
        return gameGridFood[row];
    }

    public List<GameObject> GetRowDirt(int row) {
        return gameGridDirt[row];
    }

    public void AddFood(int row, int col, GameObject sourceFood) {
        var single = Instantiate(
            singlePrefab, 
            new Vector3(-24 + (col * 4), GetGroundLevel() + (row * 4), 0),
            Quaternion.identity);
        // Copy food type attributes etc. required for feeding
        var wormFood = sourceFood.GetComponent<WormFood>();
        single.GetComponent<WormFood>().SetFoodType(wormFood.GetFoodType(), wormFood.GetFoodTypeSprite());
        gameGridFood[row][col] = single;
    }

    public void AddDirt(int row, int col, GameObject dirt) {
        gameGridDirt[row][col] = dirt;
    }

    public GameObject GetFood(int row, int col) {
        return gameGridFood[row][col];
    }

    public GameObject GetDirt(int row, int col) {
        return gameGridDirt[row][col];
    }

    public bool HasFood(int row, int col) {
        return gameGridFood[row][col] != null;
    }

    public bool HasDirt(int row, int col) {
        return gameGridDirt[row][col] != null;
    }

    public void ClearBlock(int row, GameObject block) {
        var col = gameGridDirt[row].FindIndex(x => x == block);
        if (col >= 0) {
            Destroy(gameGridFood[row][col]);
            gameGridFood[row][col] = null;
            gameGridDirt[row][col] = null;
            StartCoroutine(ClearBlockAnim(block));
        }
    }

    public float GetGroundLevel() {
        return -46;
    }

    public Coroutine ClearEmptyRows(int upToRow) {
        return StartCoroutine(ClearEmptyRowsAnim(upToRow));
    }

    void Start() {
        gameGridFood = new List<List<GameObject>>(23);
        for (int i = 0; i < 23; ++i) {
            var row = new List<GameObject>(13);
            for (int j = 0; j < 13; ++j) {
                row.Add(null);
            }
            gameGridFood.Add(row);
        }

        gameGridDirt = new List<List<GameObject>>(23);
        for (int i = 0; i < 23; ++i) {
            var row = new List<GameObject>(13);
            for (int j = 0; j < 13; ++j) {
                row.Add(null);
            }
            gameGridDirt.Add(row);
        }
    }

    IEnumerator ClearBlockAnim(GameObject block) {
        var isWorm = block.GetComponent<Dirt>().foodBlock.GetComponent<WormFood>().GetFoodType() == WormFood.FoodType.PlusOneWorm;
        var target = isWorm ? new Vector3(0, GetGroundLevel(), 0) : new Vector3(-20, 55, 0);
        while (block.transform.position != target) {
            block.transform.position = Vector3.MoveTowards(block.transform.position, target, 100 * Time.deltaTime);
            yield return null;
        }
        if (isWorm) {
            wormManager.AddWorm(block.transform.position.x);
        }
        scoreBoard.ScoreBlock();
        Destroy(block);        
    }

    IEnumerator ClearEmptyRowsAnim(int upToRows) {

        for (int fromRow = upToRows, toRow = 0; fromRow < gameGridFood.Count; ++fromRow, ++toRow) {
            for (int col = 0; col < 13; ++col) {
                gameGridFood[toRow][col] = gameGridFood[fromRow][col];
                gameGridDirt[toRow][col] = gameGridDirt[fromRow][col];
                gameGridFood[fromRow][col] = null;
                gameGridDirt[fromRow][col] = null;

                if (gameGridFood[toRow][col] != null) {
                    gameGridFood[toRow][col].transform.position = new Vector3(-24 + (col * 4), GetGroundLevel() + (toRow * 4), 0);
                }
                if (gameGridDirt[toRow][col] != null) {
                    gameGridDirt[toRow][col].transform.position = new Vector3(-24 + (col * 4), GetGroundLevel() + 2 + (toRow * 4), 0);
                }
            }
        }
        
        yield return null;
    }
}
