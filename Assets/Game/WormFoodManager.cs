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
    [SerializeField]
    Bulldozer bulldozer;
    [SerializeField]
    GameGrid gameGrid;

    List<GameObject> foodQueue;
    GameObject activeFood;

    

    float dropTimeLimit = 5.0f;
    int blocksDropped = 0;
    int currentRow;
    int currentCol;
    int foodQueued;
    
    void Start()
    {
        dropTimeLimit = 5.0f;
        blocksDropped = 0;
        activeFood = null;
        foodQueued = 0;
        
        foodQueue = new List<GameObject>();
        for (int i = 0; i < 4; ++i) {
            QueueFood();
        }
        StartCoroutine(PlayFood());
    }

    // Update is called once per frame
    void Update()
    {
              
    }

    IEnumerator PlayFood() {
        // foreach (var food in foodQueue) {
        //     var instance = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //     instance.GetComponent<SpriteRenderer>().sprite = food;
        //     yield return new WaitForSeconds(2);
        // }
        var dropSource = new Vector3(0, gameGrid.GetGroundLevel() - 4 + (gameGrid.GetRowCount() * 4), 0);

        while (true) {
            activeFood = foodQueue[0];
            activeFood.SetActive(true);
            var wormFood = activeFood.GetComponent<WormFood>();
            // Should rotation be set based on food type?
            for (int i = 0; i < Random.Range(0, 4); ++i) {
                wormFood.Rotate();
            }
            wormFood.SetGhost(true);
            

            currentRow = gameGrid.GetRowCount() - 1;
            currentCol = 6;

            SetActiveFoodPositionForColumn(currentCol);
    
            var dropped = false;
            var accumulatedTime = 0f;
            //var timePerDrop = 1.0f;
            while (!dropped) {
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
                    if (currentCol > 0 && 
                        wormFood.IsBlockGridColEmpty(-currentCol + 1)/* &&
                        !WouldCollide(currentRow, currentCol - 1)*/) {
                        --currentCol;
                        SetActiveFoodPositionForColumn(currentCol);
                    }
                }
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
                    if (currentCol < 12 && 
                        wormFood.IsBlockGridColEmpty(13 - currentCol)/* &&
                        !WouldCollide(currentRow, currentCol + 1)*/) {
                        ++currentCol;
                        SetActiveFoodPositionForColumn(currentCol);
                    }
                }

                accumulatedTime += Time.deltaTime;
                dropped = (accumulatedTime >= dropTimeLimit) || (Keyboard.current.downArrowKey.wasPressedThisFrame);
                if (!dropped) {
                    yield return null;
                }
            }

            // TODO check if drop spot puts food above bounds of game area. if so game over?


            // Drop!
            var dropTarget = activeFood.transform.position;
            activeFood.transform.position = dropSource + new Vector3(activeFood.transform.position.x, 0, 0);
            wormFood.SetGhost(false);

            while (activeFood != null) {
                activeFood.transform.position = Vector3.MoveTowards(activeFood.transform.position, dropTarget, 100 * Time.deltaTime);
                
                if (activeFood.transform.position == dropTarget) {
                    
                    var offsets = wormFood.GetBlockOffsets();
                    foreach (var offset in offsets) {
                        var offsetRow = currentRow + offset.y;
                        var offsetCol = currentCol + offset.x;
                        gameGrid.AddFood(offsetRow, offsetCol, activeFood);
                    }
                    Destroy(activeFood);
                    activeFood = null;
                }
                    
                yield return null;
            }

            bulldozer.AllowClear(true);
            while (bulldozer.IsClearing()) {
                yield return null;
            }
            bulldozer.AllowClear(false);

            foodQueue.RemoveAt(0);
            QueueFood();

            ++blocksDropped;
            if (blocksDropped % 10 == 0 && dropTimeLimit > 0.9f) {
                dropTimeLimit -= 0.2f;
            }
        }
    }

    bool WouldCollide(int row, int col, bool rotate = false) {
        var wormFood = activeFood.GetComponent<WormFood>();
        var offsets = wormFood.GetBlockOffsets(rotate);
        foreach (var offset in offsets) {
            var offsetRow = row + offset.y;
            var offsetCol = col + offset.x;
            if (offsetRow >= 0 && offsetRow < gameGrid.GetRowCount() &&
                offsetCol >= 0 && offsetCol < gameGrid.GetColumnCount() &&
                gameGrid.HasFood(offsetRow, offsetCol)) {
                return true;
            }
        }
        return false;
    }

    void SetActiveFoodPositionForColumn(int col) {
        currentRow = gameGrid.GetRowCount();
        for (int row = gameGrid.GetRowCount() - 1; row >= 0; --row) {
            if (WouldCollide(row, col)) {
                break;
            }
            currentRow = row;
        }
        activeFood.transform.position = new Vector3(-24 + (col * 4), gameGrid.GetGroundLevel() + (currentRow * 4));
    }

    void QueueFood() {
        ++foodQueued;

        var isPlusOne = (foodQueued > 0 && foodQueued % 10 == 0);
        var foodPrefab = isPlusOne ? foodPrefabs[0] : foodPrefabs[Random.Range(0, foodPrefabs.Count)];
        
        var queuedFood = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        queuedFood.SetActive(false);
        var wormFood = queuedFood.GetComponent<WormFood>();
        if (isPlusOne) {
            wormFood.SetFoodType(WormFood.FoodType.PlusOneWorm);
        }
        foodQueue.Add(queuedFood);
    }
}
