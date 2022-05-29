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
    [SerializeField]
    GameGrid gameGrid;

    float checkFrequency = 30f;
    SpriteRenderer spriteRenderer;
    bool allowClear = false;
    bool readyToClear = false;


    public void AllowClear(bool allow) {
        allowClear = allow;
    }

    public bool IsClearing() {
        return allowClear && readyToClear;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        allowClear = false;
        readyToClear = false;

        StartCoroutine(Checker());
    }

    IEnumerator Checker() {
        while (true) {
            yield return new WaitForSeconds(checkFrequency);
            readyToClear = true;
            while (!allowClear) {
                yield return null;
            }

            var rowsToClear = groundManager.GetRowsToClear();
            if (rowsToClear > 0) {
                yield return StartCoroutine(ClearRows(rowsToClear));
            } else {
                StartCoroutine(Idle());
            }
            allowClear = false;
            readyToClear = false;
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
                            blocks.Remove(block);
                            gameGrid.ClearBlock(i, block);
                        }
                    } else {
                        var block = blocks.First();
                        if (transform.position.x > block.transform.position.x) {
                            blocks.Remove(block);
                            gameGrid.ClearBlock(i, block);
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

        //groundManager.ClearEmptyRows(upToRow);
        yield return gameGrid.ClearEmptyRows(upToRow);

        groundManager.PauseWhileClearing(false);
    }
}
