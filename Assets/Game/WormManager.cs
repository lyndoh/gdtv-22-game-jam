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
    GameGrid gameGrid;
    [SerializeField]
    GameObject wormPrefab;

    List<GameObject> worms;
    List<GameObject> feedingBlocks;

    public void AddWorm(float? xpos = null) {
        var startPos = new Vector3(Random.Range(-27f, 27f), gameGrid.GetGroundLevel(), 0);
        if (xpos != null) {
            startPos.x = xpos.Value;
        }
        var worm = Instantiate(wormPrefab, startPos, Quaternion.identity);
        var wormComponent = worm.GetComponent<Worm>();
        wormComponent.groundManager = groundManager;
        wormComponent.SetTarget(startPos);
        worms.Add(worm);
        SetEatingSpeed();
    }

    void Start() {
        feedingBlocks = new List<GameObject>();
        worms = new List<GameObject>();

        AddWorm();
        AddWorm();
        AddWorm();
    }

    void Update() {
        var groundFeedingBlocks = groundManager.GetFeedingBlocks().ToList();
        
        if (!groundFeedingBlocks.SequenceEqual(feedingBlocks)) {
            feedingBlocks = groundFeedingBlocks;
            TargetWorms();
            SetEatingSpeed();
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

    void SetEatingSpeed() {
        var eatingSpeed = 0.1f + (worms.Count * 0.1f / Mathf.Max(feedingBlocks.Count, 1f));
        groundManager.SetEatingSpeed(eatingSpeed);
    }
}
