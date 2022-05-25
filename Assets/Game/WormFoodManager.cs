using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WormFoodManager : MonoBehaviour
{
    [SerializeField]
    List<Sprite> foodSprites;
    [SerializeField]
    GameObject foodPrefab;

    List<Sprite> foodQueue;
    GameObject activeFood;
    GameObject groundBase;

    void Start()
    {
        activeFood = null;
        groundBase = GameObject.FindGameObjectWithTag("GroundBase");
        
        foodQueue = new List<Sprite>();
        for (int i = 0; i < 10; ++i) {
            foodQueue.Add(foodSprites[Random.Range(0, foodSprites.Count)]);
        }
        StartCoroutine(PlayFood());
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFood != null) {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
                activeFood.transform.position += new Vector3(-4, 0, 0);
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
                activeFood.transform.position += new Vector3(4, 0, 0);
            }
        }
        
    }

    IEnumerator PlayFood() {
        // foreach (var food in foodQueue) {
        //     var instance = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //     instance.GetComponent<SpriteRenderer>().sprite = food;
        //     yield return new WaitForSeconds(2);
        // }

        foreach (var food in foodQueue) {
            activeFood = Instantiate(foodPrefab, new Vector3(0, 50, 0), Quaternion.identity);
            activeFood.GetComponent<SpriteRenderer>().sprite = food;
            var activeFoodHelfHeight = activeFood.GetComponent<SpriteRenderer>().localBounds.size.y / 2;

            var accumulatedTime = 0f;
            var timePerDrop = 0.25f;
            while (activeFood != null) {
                var groundLevel = groundBase.transform.position.y + 50;
                accumulatedTime += Time.deltaTime;
                if (accumulatedTime >= timePerDrop) {
                    activeFood.transform.position += new Vector3(0, -1, 0);
                    accumulatedTime -= timePerDrop;
                    if (activeFood.transform.position.y - activeFoodHelfHeight <= groundLevel) {
                        activeFood.transform.position = new Vector3(activeFood.transform.position.x, groundLevel + activeFoodHelfHeight, 0);
                        activeFood = null;
                    
                    }
                }
                yield return null;
            }
        }
    }
}
