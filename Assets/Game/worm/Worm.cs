using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wriggle());
    }

    IEnumerator Wriggle() {
        bool down = false;
        var topLeft = new Vector2(transform.position.x -3, transform.position.y);
        var bottomRight = new Vector2(transform.position.x, transform.position.y - 10);
        while (true) {
            down = !down;

            var dest = new Vector3(
                Random.Range(topLeft.x, bottomRight.x),
                down ? Random.Range(bottomRight.y, bottomRight.y + 1) : Random.Range(topLeft.y - 1, topLeft.y),
            transform.position.z);

            while (transform.position != dest) {
                transform.position = Vector3.MoveTowards(transform.position, dest, 0.1f);
                yield return null;
            }
        }
    }
}
