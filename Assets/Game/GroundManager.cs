using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    WormFoodManager wormFoodManager;
    [SerializeField]
    GameObject dirtPrefab;

    List<List<GameObject>> gameGrid;

    void Start() {
        gameGrid = new List<List<GameObject>>(23);
        for (int i = 0; i < 23; ++i) {
            var row = new List<GameObject>(13);
            for (int j = 0; j < 13; ++j) {
                row.Add(null);
            }
            gameGrid.Add(row);

        }
    }

    void Update() {
        for (int row = 0; row < 23; ++row) {
            for (int col = 0; col < 13; ++col) {
                if (gameGrid[row][col] == null && wormFoodManager.GetBlock(row, col) != null) {
                    gameGrid[row][col] = wormFoodManager.GetBlock(row, col);
                    StartCoroutine(AddDirt(row, col));
                }
            }
        }
    }

    IEnumerator AddDirt(int row, int col) {
        var dirt = Instantiate(dirtPrefab, new Vector3((col * 4) - 46, row * 4, 0), Quaternion.identity);
        //var spriteRenderer = dirt.GetComponent<SpriteRenderer>();
        var scale = 0.01f;
        while (scale < 1.0f) {
            scale += Time.deltaTime * 0.25f;
            dirt.transform.localScale = new Vector3(1, scale, 1);
            dirt.transform.localPosition = new Vector3(0, scale * 2, 0);
            yield return null;
        }
        dirt.transform.localScale = new Vector3(1, 1, 1);
        dirt.transform.localPosition = new Vector3(0, 2, 0);
    }
}
