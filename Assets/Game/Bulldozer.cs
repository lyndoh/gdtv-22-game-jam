using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bulldozer : MonoBehaviour
{
    [SerializeField]
    GroundManager groundManager;
    [SerializeField]
    WormFoodManager wormFoodManager;

    float checkFrequency = 30f;
    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        StartCoroutine(Checker());
    }

    IEnumerator Checker() {
        while (true) {
            yield return new WaitForSeconds(checkFrequency);
            var rowsToClear = groundManager.GetRowsToClear();
            if (rowsToClear > 0) {
                StartCoroutine(ClearRows(rowsToClear));
            } else {
                StartCoroutine(Idle());
            }
        }
    }

    IEnumerator Idle() {
        spriteRenderer.enabled = true;
        transform.position = new Vector3(-35, -42, 0);
        var target = new Vector3(-27, -42, 0);
        while (transform.position != target) {
            transform.position = Vector3.MoveTowards(transform.position, target, 8 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        spriteRenderer.flipX = true;
        target = new Vector3(-35, -42, 0);
        while (transform.position != target) {
            transform.position = Vector3.MoveTowards(transform.position, target, 8 * Time.deltaTime);
            yield return null;
        }
        spriteRenderer.enabled = false;
    }

    IEnumerator ClearRows(int upToRow) {
        groundManager.PauseWhileClearing(true);
        wormFoodManager.PauseWhileClearing(true);

        spriteRenderer.enabled = true;
        transform.position = new Vector3(-35, -42, 0);
        for (int i = 0; i < upToRow; ++i) {
            var blocks = groundManager.GetBlocks(i).ToList();
            var target = new Vector3(-transform.position.x, transform.position.y, 0);
            while (transform.position != target) {
                transform.position = Vector3.MoveTowards(transform.position, target, 35 * Time.deltaTime);
                if (blocks.Count > 0) {
                    if (spriteRenderer.flipX) {
                        var block = blocks.Last();
                        if (transform.position.x < block.transform.position.x) {
                            Debug.Log("ClearBlock");
                            blocks.Remove(block);
                            groundManager.ClearBlock(i, block);
                        }
                    } else {
                        var block = blocks.First();
                        if (transform.position.x > block.transform.position.x) {
                            Debug.Log("ClearBlock");
                            blocks.Remove(block);
                            groundManager.ClearBlock(i, block);
                        }
                    }
                }
                yield return null;
            }
            spriteRenderer.flipX = !spriteRenderer.flipX;
            transform.position += new Vector3(0, 4, 0);
        }
        spriteRenderer.enabled = false;
        spriteRenderer.flipX = false;

        groundManager.PauseWhileClearing(false);
        wormFoodManager.PauseWhileClearing(false);
    }
}
