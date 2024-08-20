using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexGridManager : MonoBehaviour
{
    
    public static HexGridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional: if you want the Singleton to persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
        
        if (!hexTileParent)
        {
            // Create new GameObject to hold the hex tiles that is a child of the HexGridManager
            hexTileParent = new GameObject("HexTiles").transform;
            hexTileParent.SetParent(transform);
        }
    }
    
    [HideInInspector] public float gridSpan = 5; // Getting the furthest distance from the center of the grid
    public GameObject hexPrefab;
    private HexGrid hexGrid;
    public readonly float _hexTileSize = 1;
    [SerializeField] private Transform hexTileParent;

    /* [OnValueChanged("GenerateHexGrid")] */ public GridShape gridShape;
    [OnValueChanged("GenerateHexGrid"), Label("Width/Diameter"), Range(0, 40)] public int width = 10;
    [SerializeField] public Transform hexGridTilt;
    [OnValueChanged("GenerateHexGrid"), ShowIf("gridShape", GridShape.Rectangle), Range(0, 40)] public int height = 10;
    /* [OnValueChanged("GenerateHexGrid")] */ private float heightVariation = 0.1f; // Extra

    private Texture2D noiseTexture; // Extra
    [OnValueChanged("GenerateHexGrid")] private float noiseScale = 1f; // Extra
    
    public int amountBlobToAdd = 3;
    
    [SerializeField] private TiltObject tiltObject;

    public Transform mainUnit;

    [SerializeField] private int mainUnityStartHealth;
    private int _currentMainUnitHealth = 100;

    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;


    private void OnEnable()
    {
        enemyEventChannel.OnEnemyAttack += HandleEnemyAttack;
        enemyEventChannel.OnWaveCompleted += HandleWaveCompleted;
    }


    private void OnDisable()
    {
        enemyEventChannel.OnEnemyAttack -= HandleEnemyAttack;
        enemyEventChannel.OnWaveCompleted += HandleWaveCompleted;

    }
    private void HandleWaveCompleted()
    {
        HexTile tile = GetRandomEdgeTiles(1)[0];
        AddCircularBlob(tile.Q, tile.R, amountBlobToAdd);
    }

    [Button]
    void Start()
    {
        hexGrid = new HexGrid(_hexTileSize, hexTileParent);
        GenerateInitialGrid();
        List<HexTile> edgeTiles = hexGrid.GetTrueEdgeTiles();
        HexTile selectedTile = edgeTiles[Random.Range(0, edgeTiles.Count)];
        //hexGrid.AddCircularBlob(selectedTile.Q, selectedTile.R, amountBlobToAdd, hexPrefab);
        mainUnit = GameObject.FindGameObjectWithTag("MainUnit").transform;
        _currentMainUnitHealth = mainUnityStartHealth;
    }

    [Button]
    private void NukeTower()
    {
        HandleEnemyAttack(_currentMainUnitHealth);
    }
    
    public void TakeDamage(int damage)
    {
        HandleEnemyAttack(damage);
    }

    private void HandleEnemyAttack(int damage)
    {
        _currentMainUnitHealth -= damage;
        if (_currentMainUnitHealth > 0)
        {
            return;
        }

        _currentMainUnitHealth = mainUnityStartHealth;
        // TODO: Play tower animation
        
        gameManagerEventChannel.RaiseGameOver();
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
        //         hexGrid.AddTile(q, r, hexPrefab, hexTileParent, GetHeightFromNoiseTexture(new Vector2(q, r)), gridShape);
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
    
    public string SayHello()
    {
        return "Hello from HexGridManager!";
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
    
    public HexTile GetNextTile(HexTile currentTile, Vector3 direction)
    {
        Vector2Int nextGridPos = new Vector2Int(Mathf.RoundToInt(currentTile.Q + direction.x), Mathf.RoundToInt(currentTile.R + direction.z));
        return hexGrid.GetTile(nextGridPos.x, nextGridPos.y);
    }
    
    public List<HexTile> GetRandomEdgeTiles(int numberOfTiles)
    {
        List<HexTile> edgeTiles = hexGrid.GetTrueEdgeTiles();
        // Shuffle the list to ensure randomness
        for (int i = 0; i < edgeTiles.Count; i++)
        {
            HexTile temp = edgeTiles[i];
            int randomIndex = Random.Range(i, edgeTiles.Count);
            edgeTiles[i] = edgeTiles[randomIndex];
            edgeTiles[randomIndex] = temp;
        }
        
        // Return the specified number of random edge tiles
        numberOfTiles = Mathf.Clamp(numberOfTiles, 0, edgeTiles.Count);
        return edgeTiles.GetRange(0, numberOfTiles);
    }
    
    public HexTile GetTile(int q, int r)
    {
        return hexGrid.GetTile(q, r);
    }
    
    public GameObject GetTileObject(int q, int r)
    {
        return hexGrid.GetTile(q, r).TileObject;
    }
    
    public GameObject GetTileObject(HexTile hexTile)
    {
        return hexTile.TileObject;
    }
    
    public List<HexTile> GetNeighbors(int q, int r)
    {
        return hexGrid.GetNeighbors(q, r);
    }
    
    public void AddTile(int q, int r)
    {
        hexGrid.AddTile(q, r, hexPrefab, hexTileParent);
    }
    
    public void AddTile(int q, int r, GameObject hexPrefabToUse)
    {
        hexGrid.AddTile(q, r, hexPrefabToUse, hexTileParent);
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
    
    public HexTile GetTileAtPoint(Vector3 position)
    {
        return GetTile(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
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
            hexGrid.AddTile(Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()), Random.Range(hexGrid.MinQ(), hexGrid.MaxQ()), hexPrefab, hexTileParent);
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

        tiltObject.tiles = hexGrid.GetTilesObjects();
        CalculateGridSpan();

    }
    
    private void CalculateGridSpan()
    {
        // Get the center point then find the hex tile that is the furthest away from the center
        Vector3 center = Vector3.zero;
        foreach (var tile in hexGrid.GetTiles())
        {
            center += tile.TileObject.transform.position;
        }
        center /= hexGrid.GetTiles().Count;
        
        foreach (var tile in hexGrid.GetTiles())
        {
            float distance = Vector3.Distance(center, tile.TileObject.transform.position);
            if (distance > gridSpan)
            {
                gridSpan = distance;
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
    //     GameObject hexTile = Instantiate(hexPrefab, hexTileParent, true);
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
    
    private (int q, int r) WorldToHex(Vector3 worldPosition)
    {
        Vector3 localPosition = Instance.transform.InverseTransformPoint(worldPosition);
        float q = (localPosition.x * Mathf.Sqrt(3f) / 3f - localPosition.z / 3f) / _hexTileSize;
        float r = localPosition.z * 2f / 3f / _hexTileSize;
        return HexRound(q, r);
    }

    private (int q, int r) HexRound(float q, float r)
    {
        float s = -q - r;
        int q_round = Mathf.RoundToInt(q);
        int r_round = Mathf.RoundToInt(r);
        int s_round = Mathf.RoundToInt(s);

        float q_diff = Mathf.Abs(q_round - q);
        float r_diff = Mathf.Abs(r_round - r);
        float s_diff = Mathf.Abs(s_round - s);

        if (q_diff > r_diff && q_diff > s_diff)
        {
            q_round = -r_round - s_round;
        }
        else if (r_diff > s_diff)
        {
            r_round = -q_round - s_round;
        }

        return (q_round, r_round);
    }

    public HexTile GetTileAtWorldPosition(Vector3 worldPosition)
    {
        var (q, r) = WorldToHex(worldPosition);
    
        // Check if the tile is within the circular boundary and exists in the grid
        HexTile tile = GetTile(q, r);
    
        if (tile != null && tile.TileObject.activeInHierarchy)
            return tile;

        return null;
    }
}


// Enum GridShape
public enum GridShape
{
    Rectangle,
    Hexagon,
    Circle
}
