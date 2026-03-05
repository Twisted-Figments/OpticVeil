using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingCells : MonoBehaviour
{
    [SerializeField] private int gridWidth = 0; //x
    [SerializeField] private int gridHeight = 0; //y
    [SerializeField] private Vector2 gridOffset; //y

    [SerializeField] private float cellHeight = 1f;
    [SerializeField] private float cellWidth = 1f;

    [SerializeField] private bool generatePath;
    [SerializeField] private bool visualiseGrid;

    public Tilemap groundMap;
    public Tilemap WallMap;

    private bool pathsGenerated;

    private Dictionary<Vector2, Cell> cells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridWidth = WallMap.cellBounds.size.x;
        gridHeight = WallMap.cellBounds.size.y;
        gridOffset = WallMap.cellSize / 2;

        for(int x = 0;  x < WallMap.cellBounds.size.x; x++)
        {
            for(int y = 0; y < WallMap.cellBounds.size.y; y++)
            {
                Vector3Int WallTile = WallMap.WorldToCell(new Vector2(x, y));
                if(WallMap.HasTile(WallTile))
                {
                    Debug.Log(new Vector2(x, y));
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(!visualiseGrid || cells == null) return;

        foreach(KeyValuePair<Vector2, Cell> kvp in cells)
        {
            if(!kvp.Value.isWall)
            {
                Gizmos.color = Color.green; // enemies can walk on this
            }
            else
            {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawCube(kvp.Key + (Vector2)transform.position, new Vector3(cellWidth, cellHeight));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(generatePath && !pathsGenerated)
        {
            GenerateGrid();
            pathsGenerated = true;
        }
        else if(!generatePath)
        {
            pathsGenerated = false;
        }
    }

    private void GenerateGrid()
    {
        cells = new Dictionary<Vector2, Cell>();

        for(float x = 0; x < gridWidth; x+= cellWidth)
        {
            for(float y = 0; y < gridHeight; y += cellHeight)
            {
                Vector2 pos = new Vector2(x, y) + gridOffset;
                if (groundMap.HasTile(groundMap.WorldToCell(pos)) == true)
                {
                    cells.Add(pos, new Cell(pos));
                }

                if (WallMap.HasTile(WallMap.WorldToCell(pos)) == true)
                {
                    cells.Add(pos, new Cell(pos));
                    cells[pos].isWall = true;
                }
            }
        }
    }

    private class Cell
    {
        public Vector2 position;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = int.MaxValue;
        public Vector2 connection;
        public bool isWall;

        public Cell(Vector2 pos)
        {
            position = pos;
        }
    }
}
