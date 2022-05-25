using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    WormFoodManager wormFoodManager;

    // Start is called before the first frame update
    void Start()
    {
        wormFoodManager = FindObjectOfType<WormFoodManager>();
    }

    void OnMouseDown() {
        // wormFoodManager.Launch();
    }
}
