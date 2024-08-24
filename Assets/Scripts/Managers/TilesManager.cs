using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviourSingletonPersistent<TileManager>
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float tileSpacing = 1.0f; // Space between tiles
    [SerializeField] private float yOffsetNoiseTap = 0.2f;
    
    private readonly Dictionary<Vector2Int, HexTileController> _grid = new();
    private GameObject _tilesParent;
    private HexTileController _targetTile;

    private new void Awake()
    {
        GenerateGrid();
    }

    [Button]
    private void GenerateGrid()
    {
        if(_tilesParent)
            DestroyImmediate(_tilesParent.gameObject);
        
        _grid.Clear();

        _tilesParent = new GameObject("TilesContainer");
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * tileSpacing - gridWidth*0.5f, Random.Range(-yOffsetNoiseTap, yOffsetNoiseTap), z * tileSpacing - gridHeight*0.5f);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, _tilesParent.transform);
                HexTileController tile = tileObject.GetComponent<HexTileController>();
                tile.GridPosition = new Vector2Int(x, z);
                _grid[new Vector2Int(x, z)] = tile;
            }
        }

        _grid.TryGetValue(new Vector2Int((int)(gridWidth / 2.0f), (int)(gridHeight / 2.0f)), out _targetTile);
    }

    public HexTileController GetTargetTile()
    {
        return _targetTile;
    }

    public HexTileController GetTileAtPosition(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(position.x / tileSpacing), Mathf.RoundToInt(position.z / tileSpacing));
        _grid.TryGetValue(gridPos, out HexTileController tile);
        return tile;
    }

    public HexTileController GetNextTile(HexTileController currentTile, Vector3 direction)
    {
        Vector2Int nextGridPos = currentTile.GridPosition + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.z));
        _grid.TryGetValue(nextGridPos, out HexTileController nextTile);
        return nextTile;
    }
    
    public List<HexTileController> GetRandomEdgeTiles(int numberOfTiles)
    {
        List<HexTileController> edgeTiles = new List<HexTileController>();

        // TODO: We could potentially cache this info
        // Add left and right edges (x = 0 and x = gridWidth - 1)
        for (int z = 0; z < gridHeight; z++)
        {
            _grid.TryGetValue(new Vector2Int(0, z), out var leftEdgeTile);
            edgeTiles.Add(leftEdgeTile); // Left edge
            
            _grid.TryGetValue(new Vector2Int(gridWidth - 1, z), out var rightEdgeTile);
            edgeTiles.Add(rightEdgeTile); // Right edge
        }

        // Add bottom and top edges (z = 0 and z = gridHeight - 1)
        for (int x = 0; x < gridWidth; x++)
        {
            _grid.TryGetValue(new Vector2Int(x, 0), out var bottomEdgeTile);
            edgeTiles.Add(bottomEdgeTile); // Bottom edge
            _grid.TryGetValue(new Vector2Int(x, gridHeight - 1), out var topEdgeTile);
            edgeTiles.Add(topEdgeTile); // Top edge
        }

        // Shuffle the list to ensure randomness
        for (int i = 0; i < edgeTiles.Count; i++)
        {
            HexTileController temp = edgeTiles[i];
            int randomIndex = Random.Range(i, edgeTiles.Count);
            edgeTiles[i] = edgeTiles[randomIndex];
            edgeTiles[randomIndex] = temp;
        }

        // Return the specified number of random edge tiles
        numberOfTiles = Mathf.Clamp(numberOfTiles, 0, edgeTiles.Count);
        return edgeTiles.GetRange(0, numberOfTiles);
    }
}

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }
}
