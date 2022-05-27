using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormFoodBlocks : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision entered");
    }
}
