using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormFoodManager : MonoBehaviour
{
    [SerializeField]
    List<Sprite> foodSprites;
    [SerializeField]
    GameObject foodPrefab;

    List<Sprite> foodQueue;

    // Start is called before the first frame update
    void Start()
    {
        foodQueue = new List<Sprite>();
        for (int i = 0; i < 10; ++i) {
            foodQueue.Add(foodSprites[Random.Range(0, foodSprites.Count)]);
        }
        StartCoroutine(PlayFood());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayFood() {
        foreach (var food in foodQueue) {
            var instance = Instantiate(foodPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            instance.GetComponent<SpriteRenderer>().sprite = food;
            yield return new WaitForSeconds(2);
        }
        
    }
}
