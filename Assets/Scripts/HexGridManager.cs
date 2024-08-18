using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class HexGridManager : MonoBehaviourSingleton<HexGridManager>
{
    public GameObject hexPrefab;
    private HexGrid hexGrid;
    private readonly float _hexTileSize = 1;

    /* [OnValueChanged("GenerateHexGrid")] */ public GridShape gridShape;
    [OnValueChanged("GenerateHexGrid"), Label("Width/Diameter"), Range(0, 40)] public int width = 10;
    [OnValueChanged("GenerateHexGrid"), ShowIf("gridShape", GridShape.Rectangle), Range(0, 40)] public int height = 10;
    /* [OnValueChanged("GenerateHexGrid")] */ private float heightVariation = 0.1f; // Extra

    private Texture2D noiseTexture; // Extra
    [OnValueChanged("GenerateHexGrid")] private float noiseScale = 1f; // Extra
    
    public int amountBlobToAdd = 3;

    [Button]
    void Start()
    {
        hexGrid = new HexGrid(_hexTileSize, this.transform);
        GenerateInitialGrid();
    }

    void GenerateInitialGrid()
    {
        Vector2 size = new Vector2(width, height);
        hexGrid.GenerateGrid(size, hexPrefab, 0, gridShape);

        // for (int q = -gridRadius; q <= gridRadius; q++)
        // {
        //     int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
        //     int r2 = Mathf.Min(gridRadius, -q + gridRadius);
        //     for (int r = r1; r <= r2; r++)
        //     {
        //         hexGrid.AddTile(q, r, hexPrefab, this.transform, GetHeightFromNoiseTexture(new Vector2(q, r)), gridShape);
        //     }
        // }
        HighlightTrueEdgeTiles();
    }

    [Button("Generate")]
    private void GenerateHexGrid()
    {
        DisableHexGrid();
        ClearHexGrid();
        Vector2 size = new Vector2(width, height);
        hexGrid.GenerateGrid(size, hexPrefab, 0, gridShape);
        
        HighlightTrueEdgeTiles();
        
        
    }
    
    public void GenerateHexGrid(int width, int height, GridShape gridShape)
    {
        this.width = width;
        this.height = height;
        this.gridShape = gridShape;
        GenerateHexGrid();
    }
    
    
    
    
    //[Button]
    void HighlightTrueEdgeTiles()
    {
        List<HexTile> edgeTiles = hexGrid.GetTrueEdgeTiles();

        foreach (var tile in edgeTiles)
        {
            //tile.TileObject.transform.position -= Vector3.up/2;
        }
    }
    
    public HexTile GetRandomTrueEdgeTile()
    {
        List<HexTile> edgeTiles = hexGrid.GetTrueEdgeTiles();
        return edgeTiles[Random.Range(0, edgeTiles.Count)];
    }
    
    public List<HexTile> GetEdgeTiles()
    {
        return hexGrid.GetTrueEdgeTiles();
    }
    
    public HexTile GetTile(int q, int r)
    {
        return hexGrid.GetTile(q, r);
    }
    
    public List<HexTile> GetNeighbors(int q, int r)
    {
        return hexGrid.GetNeighbors(q, r);
    }
    
    public void AddTile(int q, int r)
    {
        hexGrid.AddTile(q, r, hexPrefab, this.transform);
    }
    
    public void AddTile(int q, int r, GameObject hexPrefabToUse)
    {
        hexGrid.AddTile(q, r, hexPrefabToUse, this.transform);
    }
    
    public void RemoveTile(int q, int r)
    {
        hexGrid.RemoveTile(q, r);
    }
    
    public void AddCircularBlob(int q, int r, int amount, GameObject hexPrefabToUse)
    {
        hexGrid.AddCircularBlob(q, r, amount, hexPrefabToUse);
    }
    
    public void AddCircularBlob(int q, int r, int amount)
    {
        AddCircularBlob(q, r, amount, hexPrefab);
    }
    
    public HexTile GetTileAtPosition(Vector3 worldPosition)
    {
        return hexGrid.GetTileAtPosition(worldPosition);
    }


    // List of all the Manager API methods
    // AddTile(int q, int r) - Adds a tile at the specified coordinates
    // AddTile(int q, int r, GameObject hexPrefabToUse) - Adds a tile at the specified coordinates using the specified prefab
    // RemoveTile(int q, int r) - Removes the tile at the specified coordinates
    // AddCircularBlob(int q, int r, int amount) - Adds a circular blob of tiles around the specified coordinates
    // AddCircularBlob(int q, int r, int amount, GameObject hexPrefabToUse) - Adds a circular blob of tiles around the specified coordinates using the specified prefab
    // GetTileAtPosition(Vector3 worldPosition) - Gets the tile at the specified world position
    // GetTile(int q, int r) - Gets the tile at the specified coordinates
    // GetNeighbors(int q, int r) - Gets the neighbors of the tile at the specified coordinates
    // GetEdgeTiles() - Gets all the edge tiles
    // GetRandomTrueEdgeTile() - Gets a random true edge tile (Currently not perfect)
    // GenerateHexGrid(int width, int height, GridShape gridShape) - Generates the hex grid based on the specified parameters
    // GenerateHexGrid() - Generates the hex grid based on the current parameters

    // Grid Shapes are:
    // Rectangle
    // Hexagon
    // Circle
    
    


    
    
    
    
    
    void Update()
    {
        // Add 1 random tile where there is none
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hexGrid.AddTile(Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()), Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()), hexPrefab, this.transform);
            HighlightTrueEdgeTiles();
    
        }
        
        // Remove 1 random tile
        if (Input.GetKeyDown(KeyCode.R))
        {
            hexGrid.RemoveTile(Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()), Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()));
            HighlightTrueEdgeTiles();
    
        }
        
        // Add group of X tiles to Selected true edge tile
        if (Input.GetKeyDown(KeyCode.G))
        {
            List<HexTile> edgeTiles = hexGrid.GetTrueEdgeTiles();
            HexTile selectedTile = edgeTiles[Random.Range(0, edgeTiles.Count)];
            hexGrid.AddCircularBlob(selectedTile.Q, selectedTile.R, amountBlobToAdd, hexPrefab);
    
            HighlightTrueEdgeTiles();
        }
    
        // Example of getting and printing neighbors of a tile
        if (Input.GetKeyDown(KeyCode.N))
        {
            HexTile tile = hexGrid.GetTile(0, 0);
            if (tile != null)
            {
                List<HexTile> neighbors = hexGrid.GetNeighbors(tile.Q, tile.R);
                Debug.Log("Neighbors: " + neighbors.Count);
            }
        }
    }

    //[Button("Disable")]
    private void DisableHexGrid()
    {
        hexGrid.Disable();

    }

    //[Button("Clear")]
    private void ClearHexGrid()
    {
        hexGrid.Clear();
    }

    // private GameObject PoolGetTile()
    // {
    //     foreach (var t in hexTiles)
    //     {
    //         if (!t.activeInHierarchy)
    //         {
    //             return t;
    //         }
    //     }
    //
    //     GameObject hexTile = Instantiate(hexPrefab, this.transform, true);
    //     hexTiles.Add(hexTile);
    //
    //     return hexTile;
    // }

    // Method to get height from noise texture
    public float GetHeightFromNoiseTexture(Vector2 uv)
    {
        // If the noise texture is not set, return 0
        if (noiseTexture == null)
        {
            return 0;
        }

        // Scale the UV coordinates by the noise scale
        uv *= noiseScale;

        // Ensure UV coordinates are within the texture bounds
        int x = Mathf.Clamp((int)(uv.x * noiseTexture.width), 0, noiseTexture.width - 1);
        int y = Mathf.Clamp((int)(uv.y * noiseTexture.height), 0, noiseTexture.height - 1);

        // Get the pixel color at the specified UV coordinates
        Color pixelColor = noiseTexture.GetPixel(x, y);

        // Convert the color to grayscale to get the height
        float height = pixelColor.grayscale;

        return height * heightVariation;
    }
}


// Enum GridShape
public enum GridShape
{
    Rectangle,
    Hexagon,
    Circle
}
