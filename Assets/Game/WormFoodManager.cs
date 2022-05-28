using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WormFoodManager : MonoBehaviour
{
    [SerializeField]
    List<Sprite> foodSprites;
    [SerializeField]
    List<GameObject> foodPrefabs;

    List<GameObject> foodQueue;
    GameObject activeFood;

    List<List<GameObject>> gameGrid;

    int currentRow;
    int currentCol;
    bool pauseWhileClearing;

    public void PauseWhileClearing(bool pause) {
        this.pauseWhileClearing = pause;
    }

    void Start()
    {
        gameGrid = new List<List<GameObject>>(23);
        for (int i = 0; i < 23; ++i) {
            var row = new List<GameObject>(13);
            for (int j = 0; j < 13; ++j) {
                row.Add(null);
            }
            gameGrid.Add(row);

        }
        activeFood = null;
        currentRow = 22;
        currentCol = 6;
        pauseWhileClearing = false;
        
        foodQueue = new List<GameObject>();
        for (int i = 0; i < 10; ++i) {
            foodQueue.Add(foodPrefabs[Random.Range(0, foodPrefabs.Count)]);
        }
        StartCoroutine(PlayFood());
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseWhileClearing) {
            return;
        }
        
        if (activeFood != null) {
            var wormFood = activeFood.GetComponent<WormFood>();

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
                if (currentCol > 0 && 
                    wormFood.IsBlockGridColEmpty(-currentCol + 1) &&
                    !WouldCollide(currentRow, currentCol - 1)) {
                    --currentCol;
                    activeFood.transform.position += new Vector3(-4, 0, 0);
                }
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
                if (currentCol < 12 && 
                    wormFood.IsBlockGridColEmpty(13 - currentCol) &&
                    !WouldCollide(currentRow, currentCol + 1)) {
                    ++currentCol;
                    activeFood.transform.position += new Vector3(4, 0, 0);
                }
            }
            
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
                if (wormFood.IsBlockGridColEmpty(-currentCol, true) &&
                    wormFood.IsBlockGridColEmpty(14 - currentCol, true) &&
                    !WouldCollide(currentRow, currentCol, true)) {
                    activeFood.GetComponent<WormFood>().Rotate();
                }
            }
        }
        
    }

    IEnumerator PlayFood() {
        // foreach (var food in foodQueue) {
        //     var instance = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //     instance.GetComponent<SpriteRenderer>().sprite = food;
        //     yield return new WaitForSeconds(2);
        // }

        foreach (var foodPrefab in foodQueue) {
            activeFood = Instantiate(foodPrefab, new Vector3(0, GetGroundLevel() - 4 + (gameGrid.Count * 4), 0), Quaternion.identity);
            currentRow = 22;
            currentCol = 6;
            //activeFood = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            //activeFood.GetComponent<SpriteRenderer>().sprite = food;
            //var activeFoodHelfHeight = activeFood.GetComponent<WormFood>().blocks.GetComponent<SpriteRenderer>().localBounds.size.y / 2;

            var accumulatedTime = 0f;
            var timePerDrop = 1.0f;
            while (activeFood != null) {
                if (pauseWhileClearing) {
                    yield return null;
                    continue;
                }
                var dropped = false;
                accumulatedTime += Time.deltaTime;
                if (accumulatedTime >= timePerDrop) {
                    accumulatedTime -= timePerDrop;
                    dropped = true;
                }
                if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
                    accumulatedTime = 0f;
                    dropped = true;
                }

                if (dropped) {
                    if (currentRow == 0 || WouldCollide(currentRow - 1, currentCol)) {
                        var wormFood = activeFood.GetComponent<WormFood>();
                        var offsets = wormFood.GetBlockOffsets();
                        foreach (var offset in offsets) {
                            var offsetRow = currentRow + offset.y;
                            var offsetCol = currentCol + offset.x;
                            gameGrid[offsetRow][offsetCol] = activeFood;
                        }
                        activeFood = null;
                    } else {
                        --currentRow;
                        activeFood.transform.position += new Vector3(0, -4, 0);
                    }
                }
                    
                yield return null;
            }
        }
    }

    bool WouldCollide(int row, int col, bool rotate = false) {
        var wormFood = activeFood.GetComponent<WormFood>();
        var offsets = wormFood.GetBlockOffsets(rotate);
        foreach (var offset in offsets) {
            var offsetRow = row + offset.y;
            var offsetCol = col + offset.x;
            if (offsetRow >= 0 && offsetRow < gameGrid.Count &&
                offsetCol >= 0 && offsetCol < gameGrid[offsetRow].Count &&
                gameGrid[offsetRow][offsetCol] != null) {
                return true;
            }
        }
        return false;
    }

    public GameObject GetBlock(int row, int col) {
        return gameGrid[row][col];
    }

    public float GetGroundLevel() {
        return -46;
    }
}
