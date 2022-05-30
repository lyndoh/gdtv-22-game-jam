using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WormFood : MonoBehaviour
{
    public GameObject blocks;
    
    int rotationIndex = 0;

    [SerializeField]
    BlockType blockType;

    [SerializeField]
    GameObject blockPrefab;

    Vector3[] rotationSet;

    List<int>[] rotationGrid;   
    bool ghost; 
    FoodType foodType;

    static WormFood() {
        var gridSize = rotationGridCols.Length * rotationGridCols[0].Length;
        rotationGridOffsets = new Vector2Int[gridSize];
        for (int i = 0; i < gridSize; ++i) {
            var x = (i % rotationGridCols[0].Length) - 1;
            var y = (i / rotationGridCols[0].Length);
            rotationGridOffsets[i] = new Vector2Int(x, y);
        }
    }

    public bool IsBlockGridColEmpty(int col, bool nextRotation = false) {
        if (col < 0 || col > 3) { 
            return true;
        }
        int rotIndex = rotationIndex;
        if (nextRotation) {
            ++rotIndex;
            if (rotIndex > 3) {
                rotIndex = 0;
            }
        }
        
        return !rotationGrid[rotIndex].Any(x => rotationGridCols[col].Any(y => x == y));
    }

    public List<Vector2Int> GetBlockOffsets(bool nextRotation = false) {
        int rotIndex = rotationIndex;
        if (nextRotation) {
            ++rotIndex;
            if (rotIndex > 3) {
                rotIndex = 0;
            }
        }
        return rotationGrid[rotIndex].Select(x => rotationGridOffsets[x]).ToList();
    }

    public void SetGhost(bool on) {
        ghost = on;
        if (ghost) {
            GetComponentInChildren<WormFoodBlocks>().GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 0.6f);
        } else {
            GetComponentInChildren<WormFoodBlocks>().GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void SetFoodType(FoodType foodType) {
        this.foodType = foodType;
        GetComponentInChildren<WormFoodBlocks>().gameObject.GetComponentsInChildren<SpriteRenderer>(true)[1].enabled = (foodType != FoodType.Nothing);
    }

    public FoodType GetFoodType() {
        return foodType;
    }

    void Awake() {
        foodType = FoodType.Nothing;
        ghost = false;
        rotationSet = rotationSets[(int)blockType];
        rotationGrid = rotationGrids[(int)blockType];
        blocks.transform.localPosition = rotationSet[0];
    }

    public void Rotate() {
        ++rotationIndex;
        if (rotationIndex > 3) {
            rotationIndex = 0;
        }
        blocks.transform.localRotation = blocks.transform.localRotation * Quaternion.Euler(0, 0, 90);
        blocks.transform.localPosition = rotationSet[rotationIndex];
    }

    public enum BlockType {
        One,
        Two,
        ThreeCorner,
        FourTee,
    }

    static Vector3[] oneRotations = new Vector3[] {
        new Vector3(0, 2, 0),
        new Vector3(0, 2, 0),
        new Vector3(0, 2, 0),
        new Vector3(0, 2, 0)
    };

    static Vector3[] twoRotations = new Vector3[] {
        new Vector3(2, 2, 0),
        new Vector3(0, 4, 0),
        new Vector3(2, 2, 0),
        new Vector3(0, 4, 0)
    };

    static Vector3[] threeCornerRotations = new Vector3[] {
        new Vector3(2, 4, 0),
        new Vector3(-2, 4, 0),
        new Vector3(-2, 4, 0),
        new Vector3(2, 4, 0)
    };

    static Vector3[] fourTeeRotations = new Vector3[] {
        new Vector3(0, 4, 0),
        new Vector3(-2, 6, 0),
        new Vector3(0, 4, 0),
        new Vector3(2, 6, 0)
    };

    static Vector3[][] rotationSets = {
        oneRotations,
        twoRotations,
        threeCornerRotations,
        fourTeeRotations
    };

    // Rotation grid = 4x4 grid of boxes that have a block
    // Where grid 1 (bottom row second from left) is "origin"
    // 
    // --------------------------
    // |  12 |  13 |  14 |  15  |
    // --------------------------
    // |  8  |  9  |  10 |  11  |
    // --------------------------
    // |  4  |  5  |  6  |  7   |
    // --------------------------
    // |  0  |  1  |  2  |  3   |
    // --------------------------

    static int[][] rotationGridCols = new int[][] {
            new int[] {
                0, 4, 8, 12
            },
            new int[] {
                1, 5, 9, 13
            },
            new int[] {
                2, 6, 10, 14
            },
            new int[] {
                3, 7, 11, 15
            }
        };

    static Vector2Int[] rotationGridOffsets;

    static List<int>[] oneRotationGrid = {
        new List<int>() { 1 },
        new List<int>() { 1 },
        new List<int>() { 1 },
        new List<int>() { 1 }
    };

    static List<int>[] twoRotationGrid = {
        new List<int>() { 1, 2 },
        new List<int>() { 1, 5 },
        new List<int>() { 1, 2 },
        new List<int>() { 1, 5 }
    };

    static List<int>[] threeCornerRotationGrid = {
        new List<int>() { 1, 2, 5 },
        new List<int>() { 0, 1, 5 },
        new List<int>() { 1, 4, 5 },
        new List<int>() { 1, 5, 6 }
    };

    static List<int>[] fourTeeRotationGrid = {
        new List<int>() { 0, 1, 2, 5 },
        new List<int>() { 1, 4, 5, 9 },
        new List<int>() { 1, 4, 5, 6 },
        new List<int>() { 1, 5, 6, 9 }
    };

    static List<int>[][] rotationGrids = {
        oneRotationGrid,
        twoRotationGrid,
        threeCornerRotationGrid,
        fourTeeRotationGrid
    };

    public enum FoodType {
        Nothing,
        PlusOneWorm,
    };
}
