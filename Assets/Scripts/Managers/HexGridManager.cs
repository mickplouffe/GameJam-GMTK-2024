using System;
using System.Collections.Generic;
using CristiEventSystem.EventChannels;
using GridSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HexGridManager : MonoBehaviourSingleton<HexGridManager>
{

    ///// SETTINGS /////
    
    // PUBLICS

    [SerializeField] public Transform hexGridTilt;

    // PRIVATES
    private HexGrid _hexGrid;
    private Transform _hexTilesContainer;
    private const float HexTileSize = 1;
    [HideInInspector] public float gridSpan = 5; // Getting the furthest distance from the center of the grid


    [SerializeField, Expandable] private GridTileSet gridTileSet;

    private GameObject _hexPrefab;

    private Animator _animator;

    // Health Should be split into a separate class or Interface
    private int mainUnitStartHealth = 100;
    private int _currentMainUnitHealth = 1; 
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    private RectTransform healthBar;
    private bool IsDead { get; set; }
    


    ///// DEBUGGING /////
    [OnValueChanged("GenerateHexGrid"), Label("Width/Diameter"), Range(0, 40)] public int width = 10;

    private int amountBlobToAdd = 3;

    /* [OnValueChanged("GenerateHexGrid")] */ public GridShape gridShape = GridShape.Circle;

    [OnValueChanged("GenerateHexGrid"), ShowIf("gridShape", GridShape.Rectangle), Range(0, 40)] public int height = 10;
    /* [OnValueChanged("GenerateHexGrid")] */ private float heightVariation = 0.1f;

    private Texture2D noiseTexture; // Extra
    /*[OnValueChanged("GenerateHexGrid")]*/ private float noiseScale = 1f;
    
    private void Awake()
    {
        if (!_hexTilesContainer)
        {
            // Create new GameObject to hold the hex tiles that is a child of the HexGridManager
            _hexTilesContainer = new GameObject("hexTilesContainer").transform;
            _hexTilesContainer.SetParent(transform);
        }
        _hexPrefab = gridTileSet.GetTile(0);
        GenerateGrid();
    }

    
    private void OnEnable()
    {
        enemyEventChannel.OnEnemyAttack += HandleEnemyAttack;
        enemyEventChannel.OnWaveCompleted += HandleWaveCompleted;
        gameManagerEventChannel.OnGameRestart += GenerateHexGrid;
        
    }


    private void OnDisable()
    {
        enemyEventChannel.OnEnemyAttack -= HandleEnemyAttack;
        enemyEventChannel.OnWaveCompleted -= HandleWaveCompleted;
        gameManagerEventChannel.OnGameRestart -= GenerateHexGrid;

        
    }
    private void HandleWaveCompleted()
    {
        HexTile tile = GetRandomEdgeTiles(1)[0];
        AddCircularBlob(tile.Q, tile.R, amountBlobToAdd);
    }

    // [Button]
    void GenerateGrid()
    {
        _hexGrid = new HexGrid(HexTileSize, _hexTilesContainer);
        GenerateInitialGrid();
        List<HexTile> edgeTiles = _hexGrid.GetTrueEdgeTiles();
        HexTile selectedTile = edgeTiles[Random.Range(0, edgeTiles.Count)];
        //hexGrid.AddCircularBlob(selectedTile.Q, selectedTile.R, amountBlobToAdd, hexPrefab);
        // mainUnit = GameObject.FindGameObjectWithTag("MainUnit").transform;
        _currentMainUnitHealth = mainUnitStartHealth;

        // if (!_animator)
        // {
        //     _animator = mainUnit.GetComponent<Animator>();
        // }

        if (!healthBar)
        {
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<RectTransform>();
        }
        
        healthBar.localScale = new Vector3((float)_currentMainUnitHealth / mainUnitStartHealth, 1, 1);

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
        healthBar.localScale = new Vector3((float)_currentMainUnitHealth / mainUnitStartHealth, 1, 1);
        if (_currentMainUnitHealth > 0)
            return;

        if(IsDead)
            return;
        
        IsDead = true;
        // TODO: Play tower animation
        _animator.SetBool("IsDead", true);
        GameManager.Instance.gameOverMusic.Post(gameObject);
        Invoke("SetIsDeadTrue", 3.5f);
                
    }

    private void SetIsDeadTrue()
    {
        gameManagerEventChannel.RaiseGameOver();
    }

    void GenerateInitialGrid()
    {
        Vector2 size = new Vector2(width, height);
        _hexGrid.GenerateGrid(size, _hexPrefab, 0, gridShape);

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
        _hexGrid.GenerateGrid(size, _hexPrefab, 0, gridShape);
        _animator.SetBool("IsDead", false);

        HighlightTrueEdgeTiles();
        _currentMainUnitHealth = mainUnitStartHealth;


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
        List<HexTile> edgeTiles = _hexGrid.GetTrueEdgeTiles();

        foreach (var tile in edgeTiles)
        {
            //tile.TileObject.transform.position -= Vector3.up/2;
        }
    }
    
    public HexTile GetRandomTrueEdgeTile()
    {
        List<HexTile> edgeTiles = _hexGrid.GetTrueEdgeTiles();
        return edgeTiles[Random.Range(0, edgeTiles.Count)];
    }
    
    public List<HexTile> GetEdgeTiles()
    {
        return _hexGrid.GetTrueEdgeTiles();
    }
    
    public HexTile GetNextTile(HexTile currentTile, Vector3 direction)
    {
        Vector2Int nextGridPos = new Vector2Int(Mathf.RoundToInt(currentTile.Q + direction.x), Mathf.RoundToInt(currentTile.R + direction.z));
        return _hexGrid.GetTile(nextGridPos.x, nextGridPos.y);
    }
    
    public List<HexTile> GetRandomEdgeTiles(int numberOfTiles)
    {
        List<HexTile> edgeTiles = _hexGrid.GetTrueEdgeTiles();
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
        return _hexGrid.GetTile(q, r);
    }
    
    public GameObject GetTileObject(int q, int r)
    {
        return _hexGrid.GetTile(q, r).TileObject;
    }
    
    public GameObject GetTileObject(HexTile hexTile)
    {
        return hexTile.TileObject;
    }
    
    public List<HexTile> GetNeighbors(int q, int r)
    {
        return _hexGrid.GetNeighbors(q, r);
    }
    
    public void AddTile(int q, int r)
    {
        _hexGrid.AddTile(q, r, _hexPrefab, _hexTilesContainer);
    }
    
    public void AddTile(int q, int r, GameObject hexPrefabToUse)
    {
        _hexGrid.AddTile(q, r, hexPrefabToUse, _hexTilesContainer);
    }
    
    public void RemoveTile(int q, int r)
    {
        _hexGrid.RemoveTile(q, r);
    }
    
    public void AddCircularBlob(int q, int r, int amount, GameObject hexPrefabToUse)
    {
        _hexGrid.AddCircularBlob(q, r, amount, hexPrefabToUse);
    }
    
    public void AddCircularBlob(int q, int r, int amount)
    {
        AddCircularBlob(q, r, amount, _hexPrefab);
    }
    
    public HexTile GetTileAtPosition(Vector3 worldPosition)
    {
        return _hexGrid.GetTileAtPosition(worldPosition);
    }
    
    public HexTile GetTileAtPoint(Vector3 position)
    {
        return GetTile(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
    }
    
    /// <summary>
    ///  DISABLE ME!
    /// TODO: DISABLE ME!
    /// </summary>
    void Update()
    {
        // Add 1 random tile where there is none
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _hexGrid.AddTile(Random.Range(_hexGrid.MinQ(), _hexGrid.MaxQ()), Random.Range(_hexGrid.MinQ(), _hexGrid.MaxQ()), _hexPrefab, _hexTilesContainer);
            HighlightTrueEdgeTiles();
    
        }
        
        // Remove 1 random tile
        if (Input.GetKeyDown(KeyCode.R))
        {
            _hexGrid.RemoveTile(Random.Range(_hexGrid.MinQ(), _hexGrid.MaxQ()), Random.Range(_hexGrid.MinQ(), _hexGrid.MaxQ()));
            HighlightTrueEdgeTiles();
    
        }
        
        // Add group of X tiles to Selected true edge tile
        if (Input.GetKeyDown(KeyCode.G))
        {
            List<HexTile> edgeTiles = _hexGrid.GetTrueEdgeTiles();
            HexTile selectedTile = edgeTiles[Random.Range(0, edgeTiles.Count)];
            _hexGrid.AddCircularBlob(selectedTile.Q, selectedTile.R, amountBlobToAdd, _hexPrefab);
    
            HighlightTrueEdgeTiles();
        }
    
        // Example of getting and printing neighbors of a tile
        if (Input.GetKeyDown(KeyCode.N))
        {
            HexTile tile = _hexGrid.GetTile(0, 0);
            if (tile != null)
            {
                List<HexTile> neighbors = _hexGrid.GetNeighbors(tile.Q, tile.R);
                Debug.Log("Neighbors: " + neighbors.Count);
            }
        }
        CalculateGridSpan();

    }
    
    private void CalculateGridSpan()
    {
        // Get the center point then find the hex tile that is the furthest away from the center
        Vector3 center = Vector3.zero;
        foreach (var tile in _hexGrid.GetTiles())
        {
            center += tile.TileObject.transform.position;
        }
        center /= _hexGrid.GetTiles().Count;
        
        foreach (var tile in _hexGrid.GetTiles())
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
        _hexGrid.Disable();

    }

    //[Button("Clear")]
    private void ClearHexGrid()
    {
        _hexGrid.Clear();
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
        float q = (localPosition.x * Mathf.Sqrt(3f) / 3f - localPosition.z / 3f) / HexTileSize;
        float r = localPosition.z * 2f / 3f / HexTileSize;
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

    public Dictionary<(int q, int r), HexTile> GetAllTiles()
    {
        return _hexGrid.GetAllTiles();
    }
}


// Enum GridShape
public enum GridShape
{
    Rectangle,
    Hexagon,
    Circle
}
