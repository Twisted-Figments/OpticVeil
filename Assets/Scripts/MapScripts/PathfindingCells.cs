using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingCells : MonoBehaviour
{
    public Tilemap WallMap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
