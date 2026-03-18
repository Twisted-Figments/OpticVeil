using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingCells : MonoBehaviour
{
    [SerializeField] private int gridWidth = 0; //x
    [SerializeField] private int gridHeight = 0; //y
    [SerializeField] private Vector2 gridOffset;

    [SerializeField] private float cellHeight = 1f;
    [SerializeField] private float cellWidth = 1f;

    [SerializeField] private bool generatePath;
    [SerializeField] private bool visualiseGrid;

    public Tilemap groundMap;
    public Tilemap WallMap;

    public bool pathsGenerated;

    [SerializeField] private Dictionary<Vector2, Cell> cells;

    [SerializeField]  public List<Vector2> cellsToSearch;
    [SerializeField] public List<Vector2> searchedCells;
    [SerializeField]  public List<Vector2> finalPath;

    [SerializeField] private int StartPathLocX = 1;
    [SerializeField] private int StartPathLocY = 1;

    [SerializeField] private int FinalPathLocX = 1;
    [SerializeField] private int FinalPathLocY = 1;

    void Start()
    {
        gridWidth = WallMap.cellBounds.size.x;
        gridHeight = WallMap.cellBounds.size.y;
        //gridOffset = WallMap.cellSize / 2;


        for(int x = 0;  x < WallMap.cellBounds.size.x; x++)
        {
            for(int y = 0; y < WallMap.cellBounds.size.y; y++)
            {
                Vector3Int WallTile = WallMap.WorldToCell(new Vector2(x, y));
                if(WallMap.HasTile(WallTile))
                {
                    //Debug.Log(new Vector2(x, y));
                }
            }
        }

        GenerateGrid();
    }

    
    private void Update()
    {
        
    }

    public void GeneratePath(Vector2 StartPos ,Vector2 Position)
    {
        if (generatePath && !pathsGenerated)
        {
            FindPath(StartPos, Position);
            pathsGenerated = true;
        }
        else if (!generatePath)
        {
            pathsGenerated = false;
        }
    }
    

    public void GenerateGrid()
    {

        cells = new Dictionary<Vector2, Cell>();

        for (float x = 0; x < gridWidth; x += cellWidth)
        {
            for (float y = 0; y < gridHeight; y += cellHeight)
            {
                Vector2 pos = new Vector2(x, y) + gridOffset;

                cells.Add(pos, new Cell(pos));

                if (WallMap.HasTile(WallMap.WorldToCell(pos)) == true)
                {
                    cells[pos].isWall = true;
                }
            }
        }

        /*
        cells = new Dictionary<Vector2, Cell>();

        for (float x = 0; x < gridWidth; x += cellWidth)
        {
            for (float y = 0; y < gridHeight; y += cellHeight)
            {
                Vector2 pos = new Vector2(x, y) + gridOffset;
                if (!WallMap.HasTile(WallMap.WorldToCell(pos)) == true)
                {
                    cells.Add(pos, new Cell(pos));
                    //Debug.Log(cells[x, y]);
                }

                else if (WallMap.HasTile(WallMap.WorldToCell(pos)) == true)
                {
                    cells.Add(pos, new Cell(pos));
                    cells[pos].isWall = true;
                }
            }
        }
        */
    }

    public void FindPath(Vector2 startPos, Vector2 endPos)
    {
        searchedCells = new List<Vector2>();
        cellsToSearch = new List<Vector2> { startPos };
        finalPath = new List<Vector2>();

        Cell startCell = cells[startPos];
        startCell.gCost = 0;
        startCell.hCost = GetDistance(startPos, endPos);
        startCell.fCost = GetDistance(startPos, endPos);

        while (cellsToSearch.Count > 0)
        {
            Vector2 cellToSearch = cellsToSearch[0];

            foreach (Vector2 pos in cellsToSearch)
            {
                Cell c = cells[pos];
                if (c.fCost < cells[cellToSearch].fCost ||
                    c.fCost == cells[cellToSearch].fCost && c.hCost == cells[cellToSearch].hCost)
                {
                    cellToSearch = pos;
                }
            }

            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == endPos)
            {
                Cell pathCell = cells[endPos];

                while (pathCell.position != startPos)
                {
                    finalPath.Add(pathCell.position);
                    pathCell = cells[pathCell.connection];
                }

                finalPath.Add(startPos);
                finalPath.Reverse();
                return;
            }

            SearchCellNeighbors(cellToSearch, endPos);
        }
    }

    private void SearchCellNeighbors(Vector2 cellPos, Vector2 endPos)
    {
        for (float x = cellPos.x - cellWidth; x <= cellWidth + cellPos.x; x += cellWidth)
        {
            for (float y = cellPos.y - cellHeight; y <= cellHeight + cellPos.y; y += cellHeight)
            {
                Vector2 neighborPos = new Vector2(x, y);
                if(cellPos + new Vector2(cellWidth, cellHeight) - new Vector2(Mathf.Abs(neighborPos.x), Mathf.Abs(neighborPos.y)) == Vector2.zero)
                {
                    continue;
                }
                Debug.DrawLine(neighborPos, cellPos);

                if (cells.TryGetValue(neighborPos, out Cell c) && !searchedCells.Contains(neighborPos) && !cells[neighborPos].isWall)
                {
                    int GcostToNeighbour = cells[cellPos].gCost + GetDistance(cellPos, neighborPos);

                    if (GcostToNeighbour < cells[neighborPos].gCost)
                    {
                        Cell neighbourNode = cells[neighborPos];

                        neighbourNode.connection = cellPos;
                        neighbourNode.gCost = GcostToNeighbour;
                        neighbourNode.hCost = GetDistance(neighborPos, endPos);
                        neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;

                        if (!cellsToSearch.Contains(neighborPos))
                        {
                            cellsToSearch.Add(neighborPos);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs((int)pos1.x - (int)pos2.x), Mathf.Abs((int)pos1.y - (int)pos2.y));

        int lowest = Mathf.Min(dist.x, dist.y);
        int highest = Mathf.Max(dist.x, dist.y);

        int horizontalMovesRequired = highest - lowest;

        return lowest * 14 + horizontalMovesRequired * 10;
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

            if(finalPath.Contains(kvp.Key))
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(kvp.Key + (Vector2)transform.position, new Vector3(cellWidth, cellHeight));
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
