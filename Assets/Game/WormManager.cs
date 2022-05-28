using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    [SerializeField]
    WormFoodManager wormFoodManager;
    [SerializeField]
    GroundManager groundManager;
    [SerializeField]
    GameObject wormPrefab;

    List<GameObject> worms;
    List<GameObject> feedingBlocks;

    void Start() {
        feedingBlocks = new List<GameObject>();
        worms = new List<GameObject>();

        var numberOfWorms = 3;

        for (int i = 0; i < numberOfWorms; ++i) {
            var startPos = new Vector3(Random.Range(-27f, 27f), wormFoodManager.GetGroundLevel(), 0);
            var worm = Instantiate(wormPrefab, startPos, Quaternion.identity);
            var wormComponent = worm.GetComponent<Worm>();
            wormComponent.wormId = i;
            wormComponent.groundManager = groundManager;
            wormComponent.SetTarget(startPos);
            worms.Add(worm);
        }
    }

    void Update() {
        var groundFeedingBlocks = groundManager.GetFeedingBlocks().ToList();
        
        if (!groundFeedingBlocks.SequenceEqual(feedingBlocks)) {
            feedingBlocks = groundFeedingBlocks;
            TargetWorms();
            
        }

    }

    void TargetWorms() {
        if (feedingBlocks.Count == 0) {
            return;
        }

        var offset = new Vector3(0, -2, 0);
        for (int i = 0; i < worms.Count; ++i) {
            worms[i].GetComponent<Worm>().SetTarget(feedingBlocks[i % feedingBlocks.Count].transform.position + offset);
        }
    }
}
