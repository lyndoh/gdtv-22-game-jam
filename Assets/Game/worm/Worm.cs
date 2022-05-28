using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    public int wormId;
    public GroundManager groundManager;
    public Vector3 feedingTarget;

    public void SetTarget(Vector3 target) {
        feedingTarget = target;
    }

    void Start()
    {
        StartCoroutine(Wriggle());
    }

    IEnumerator Wriggle() {
        // Randomise animation speed so worms aren't all in sync
        var animator = GetComponent<Animator>();
        animator.speed = Random.Range(0.8f, 1.0f);

        // Movement loop
        while (true) {
            var localTarget = new Vector3(Random.Range(feedingTarget.x - 2, feedingTarget.x + 2), feedingTarget.y, 0);
            var toTarget = localTarget - transform.position;
            if (toTarget.magnitude > 0.01f) {          
                // Rotate towards target   
                var toRotation = Quaternion.FromToRotation(Vector3.up, toTarget);
                while (transform.rotation != toRotation) {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 200 * Time.deltaTime);
                    yield return null;
                }
                // Move towards target
                while (transform.position != localTarget) {
                    transform.position = Vector3.MoveTowards(transform.position, localTarget, 8 * Time.deltaTime);
                    yield return null;
                }
                // Rotate towards origin
                var originRotation = Quaternion.Euler(0, 0, 0);
                while (transform.rotation != originRotation) {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, originRotation, 200 * Time.deltaTime);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(Random.Range(0.8f, 1.0f)); 
        }

        // bool down = false;
        // var topLeft = new Vector2(transform.position.x -3, transform.position.y);
        // var bottomRight = new Vector2(transform.position.x, transform.position.y - 10);
        // while (true) {
        //     down = !down;

        //     var dest = new Vector3(
        //         Random.Range(topLeft.x, bottomRight.x),
        //         down ? Random.Range(bottomRight.y, bottomRight.y + 1) : Random.Range(topLeft.y - 1, topLeft.y),
        //     transform.position.z);

        //     while (transform.position != dest) {
        //         transform.position = Vector3.MoveTowards(transform.position, dest, 0.1f);
        //         yield return null;
        //     }
        // }
    }
}
