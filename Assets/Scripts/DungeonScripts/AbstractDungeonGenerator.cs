using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    protected virtual void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        if (tilemapVisualizer == null)
        {
            Debug.LogError("TilemapVisualizer is not assigned in the inspector.");
            return;
        }

        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
