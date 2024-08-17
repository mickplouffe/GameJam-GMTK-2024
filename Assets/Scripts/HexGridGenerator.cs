using System;using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    [OnValueChanged("GenerateHexGrid")]
    public int width = 10;
    [OnValueChanged("GenerateHexGrid")]
    public int height = 10;
    [OnValueChanged("GenerateHexGrid")]
    public float hexTileSize = 1.75f;
    public GridShape gridShape;
    [OnValueChanged("GenerateHexGrid")]
    public float heightVariation = 0.1f;
    
    public Texture2D noiseTexture;
    [OnValueChanged("GenerateHexGrid")]
    public float noiseScale = 1f;
    
    public List<GameObject> hexTiles = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [Button("Generate")]
    private void GenerateHexGrid()
    {
        DisableHexGrid();
        for (int z = 0; z < width; z++)
        {
            float xOffset = 0;
            if (z % 2 == 1)
            {
                xOffset = hexTileSize / 2;
            }
            else
            {
                xOffset = 0;
            }
            for (int x = 0; x < height; x++)
            {

                GameObject hexTile = PoolGetTile();
                Vector3 position = CalculateHexTilePosition(x, z, xOffset);
                
                position = this.transform.position + position;
                hexTile.transform.position = position;
                hexTile.SetActive(true);
            }
        }
        
        
    }
    
    public Vector3 CalculateHexTilePosition(int x, int z, float xOffset)
    {
        // Randomize the position of the height of the hexagons
        // float randomHeight = Random.Range(-heightVariation, heightVariation);
        float randomHeight = GetHeightFromNoiseTexture(new Vector2(x / (float)width, z / (float)height)) * heightVariation;
        switch (gridShape)
        {
            case GridShape.Hexagon:
                return new Vector3(x * hexTileSize * 1.5f + xOffset, randomHeight, z * hexTileSize * 1.75f);
            case GridShape.Rectangle:
                return new Vector3(x * hexTileSize + xOffset, randomHeight, z * hexTileSize);
            case GridShape.Circle:
                // Generate the Hexagons in circle
                float angle = x * 60;
                return new Vector3(x * hexTileSize + xOffset, randomHeight, z * hexTileSize);
                
        }

        return Vector3.zero;
    }
    
    [Button("Disable")]
    private void DisableHexGrid()
    {
        int childCount = this.transform.childCount;
        for (int i = childCount; i > 0; i--)
        {
            (this.transform.GetChild(i-1).transform).gameObject.SetActive(false);
        }
    }
    
    [Button("Clear")]
    private void ClearHexGrid()
    {
        int childCount = this.transform.childCount;
        for (int i = childCount; i > 0; i--)
        {
            hexTiles.Remove(this.transform.GetChild(i-1).gameObject);
            DestroyImmediate(this.transform.GetChild(i-1).gameObject);
        }
    }
    
    private GameObject PoolGetTile()
    {
        foreach (var t in hexTiles)
        {
            if (!t.activeInHierarchy)
            {
                return t;
            }
        }
        GameObject hexTile = Instantiate(hexTilePrefab, this.transform, true);
        hexTiles.Add(hexTile);

        return hexTile;
    }
    
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

        return height;
    }

}
// Enum GridShape
    
public enum GridShape
{
    Rectangle,
    Hexagon,
    Circle
}





