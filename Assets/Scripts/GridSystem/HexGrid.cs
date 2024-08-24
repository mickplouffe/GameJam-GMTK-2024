using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class HexGrid
    {
        private readonly Dictionary<(int q, int r), HexTile> _hexTiles;
        private float _hexSize;
        private readonly Transform _gridOrigin;
        // private float _yOffset = 0f;

        public HexGrid(float hexSize, Transform gridOrigin)
        {
            _hexSize = hexSize;
            _gridOrigin = gridOrigin;
            _hexTiles = new Dictionary<(int q, int r), HexTile>();
        }
    
        public void GenerateGrid(Vector2 size, GameObject hexPrefab, float yOffset = 0f, GridShape gridShape = GridShape.Hexagon)
        {
            switch (gridShape)
            {
                case GridShape.Hexagon:
                    GenerateHexagonGrid(size, hexPrefab, yOffset);
                    break;
                case GridShape.Rectangle:
                    GenerateRectangleGrid(size, hexPrefab, yOffset);
                    break;
                case GridShape.Circle:
                    GenerateCircleGrid(size, hexPrefab, yOffset);
                    break;
                default:
                    GenerateHexagonGrid(size, hexPrefab, yOffset);
                    break;
            }
        }
    
        private void GenerateHexagonGrid(Vector2 size, GameObject hexPrefab, float yOffset = 0f)
        {
            // First, Generate all the positions for the hexagons
            // Using size.x as the radius of the hexagon grid
            List<(int q, int r)> positions = new List<(int q, int r)>();
            int gridRadius = Mathf.FloorToInt(size.x / 2);
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                int r2 = Mathf.Min(gridRadius, -q + gridRadius);
                for (int r = r1; r <= r2; r++)
                {
                    positions.Add((q, r));
                }
            }
        

            // Then, add the hexagons to the grid
            foreach (var pos in positions)
            {
                AddTile(pos.q, pos.r, hexPrefab, _gridOrigin, yOffset);
            }
        
        }
    
        private void GenerateRectangleGrid(Vector2 size, GameObject hexPrefab, float yOffset = 0f)
        {
            // First, Generate all the positions for the hexagons
            // Since it is a rectangle, we can use the width and height to determine the positions
            List<(int q, int r)> positions = new List<(int q, int r)>();
            int width = Mathf.FloorToInt(size.x);
            int height = Mathf.FloorToInt(size.y);
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    int q = x - (z >> 1); // Staggered rows
                    positions.Add((q, z));
                }
            }

            // Then, add the hexagons to the grid
            foreach (var pos in positions)
            {
                AddTile(pos.q, pos.r, hexPrefab, _gridOrigin, yOffset);
            }
        }
    
        private void GenerateCircleGrid(Vector2 size, GameObject hexPrefab, float yOffset = 0f)
        {
            List<(int q, int r)> positions = new List<(int q, int r)>();

            // The radius of the circle in world units
            float radius = size.x / 2f;

            // Hexagon size factors
            float hexWidth = Mathf.Sqrt(3) * _hexSize; // Width of a single hexagon
            float hexHeight = 2 * _hexSize * 0.75f;    // Height from one flat side to the opposite flat side

            // Calculate the maximum range of hexes to consider
            int maxRange = Mathf.CeilToInt(radius / _hexSize);

            for (int q = -maxRange; q <= maxRange; q++)
            {
                for (int r = -maxRange; r <= maxRange; r++)
                {
                    // Convert hex coordinates to world space
                    float x = hexWidth * (q + r / 2f);
                    float y = hexHeight * r;

                    // Calculate Euclidean distance from the center
                    float distance = Mathf.Sqrt(x * x + y * y);

                    // Check if the distance is within the radius
                    if (distance <= radius)
                    {
                        positions.Add((q, r));
                    }
                }
            }

            // Add the hexagons to the grid
            foreach (var pos in positions)
            {
                AddTile(pos.q, pos.r, hexPrefab, _gridOrigin, yOffset);
            }
        }
    
        public void AddTile(int q, int r, GameObject hexPrefab, Transform parent = null, float yOffset = 0f)
        {
            if (!IsTile(q, r))
            {
                Vector3 position = HexToWorld(q, r, yOffset);
                GameObject hex = PoolGetTile(q, r, hexPrefab, parent);
                hex.transform.position = position;
                hex.transform.rotation = _gridOrigin.rotation;
                hex.name = "HexTile " + q + "x" + r;
                hex.SetActive(true);
                hex.GetComponent<HexTileController>().GridPosition = new Vector2Int(q, r);
            }
            else
            {
                GameObject hex = _hexTiles[(q, r)].TileObject;
                if (hex.activeInHierarchy == false)
                {
                    Vector3 position = HexToWorld(q, r, yOffset);
                    hex.transform.position = position;
                    hex.transform.rotation = _gridOrigin.rotation;
                    hex.name = "HexTile " + q + "x" + r;
                    hex.SetActive(true);
                    hex.GetComponent<HexTileController>().GridPosition = new Vector2Int(q, r);
                }
            }
        }
    
        public void AddBlob(int q, int r, int radius, GameObject hexPrefab)
        {
            // Generate all positions within the given radius
            List<(int q, int r)> positions = GenerateHexesInRadius(q, r, radius);

            foreach (var pos in positions)
            {
                int pq = pos.q;
                int pr = pos.r;

                // Only add tiles if the position is empty (no existing or active tile)
                if (GetTile(pq, pr) == null)
                {
                    AddTile(pq, pr, hexPrefab, _gridOrigin);
                }
            }
        }

        private List<(int q, int r)> GenerateHexesInRadius(int centerQ, int centerR, int radius)
        {
            List<(int q, int r)> positions = new List<(int q, int r)>();

            for (var dq = -radius; dq <= radius; dq++)
            {
                for (var dr = Mathf.Max(-radius, -dq - radius); dr <= Mathf.Min(radius, -dq + radius); dr++)
                {
                    var dqAbs = Mathf.Abs(dq);
                    var drAbs = Mathf.Abs(dr);
                    var dsAbs = Mathf.Abs(-dq - dr);

                    if (dqAbs + drAbs + dsAbs <= radius * 2)
                    {
                        positions.Add((centerQ + dq, centerR + dr));
                    }
                }
            }

            return positions;
        }
    
        public void AddCircularBlob(int q, int r, float radius, GameObject hexPrefab)
        {
            // Generate all positions within the given radius, applying a circular boundary
            List<(int q, int r)> positions = GenerateCircularHexes(q, r, radius);

            foreach (var pos in positions)
            {
                int pq = pos.q;
                int pr = pos.r;

                // Only add tiles if the position is empty (no existing or active tile)
                if (GetTile(pq, pr) == null)
                {
                    AddTile(pq, pr, hexPrefab, _gridOrigin);
                }
            }
        }

        private List<(int q, int r)> GenerateCircularHexes(int centerQ, int centerR, float radius)
        {
            List<(int q, int r)> positions = new List<(int q, int r)>();

            int maxRange = Mathf.CeilToInt(radius); // Determine the range of hexagons to check

            for (var dq = -maxRange; dq <= maxRange; dq++)
            {
                for (var dr = Mathf.Max(-maxRange, -dq - maxRange); dr <= Mathf.Min(maxRange, -dq + maxRange); dr++)
                {
                    var ds = -dq - dr;
                
                    // Calculate the Euclidean distance from the center
                    Vector3 tileWorldPos = HexToWorld(centerQ + dq, centerR + dr);
                    Vector3 centerWorldPos = HexToWorld(centerQ, centerR);
                    var distance = Vector3.Distance(tileWorldPos, centerWorldPos);

                    if (distance <= radius * _hexSize)
                    {
                        positions.Add((centerQ + dq, centerR + dr));
                    }
                }
            }

            return positions;
        }
    
        public int MinQ()
        {
            var minQ = int.MaxValue;
            foreach (var tile in _hexTiles)
            {
                minQ = Mathf.Min(minQ, tile.Key.q);
            }

            return minQ;
        }
    
        public int MaxQ()
        {
            var maxQ = int.MinValue;
            foreach (var tile in _hexTiles)
            {
                maxQ = Mathf.Max(maxQ, tile.Key.q);
            }

            return maxQ;
        }
    
    
        public int MinR()
        {
            var minR = int.MaxValue;
            foreach (var tile in _hexTiles)
            {
                minR = Mathf.Min(minR, tile.Key.r);
            }

            return minR;
        }
    
        public int MaxR()
        {
            var maxR = int.MinValue;
            foreach (var tile in _hexTiles)
            {
                maxR = Mathf.Max(maxR, tile.Key.r);
            }

            return maxR;
        }
    
        public void RemoveTile(int q, int r)
        {
            if (!IsTile(q, r)) return;
            
            GameObject hex = _hexTiles[(q, r)].TileObject;
            hex.SetActive(false);
        }
    
        private GameObject PoolGetTile(int q, int r, GameObject hexPrefab, Transform parent = null)
        {
            foreach (var t in _hexTiles)
            {
                if (!t.Value.TileObject.activeInHierarchy)
                {
                    return t.Value.TileObject;
                }
            }

            var hex = Object.Instantiate(hexPrefab, Vector3.zero, Quaternion.identity, parent);
            var tile = new HexTile(q, r, hex);
            _hexTiles[(q, r)] = tile;
            // HexGridManager.Instance.hexTiles.Add(hex);

            return hex;
        }

        public HexTile GetTile(int q, int r)
        {
            _hexTiles.TryGetValue((q, r), out var tile);
            if (tile != null && tile.TileObject.activeInHierarchy)
            {
                return tile;
            }
            return null;
        }
    
        public List<HexTile> GetTiles(bool activeOnly = true)
        {
            var tiles = new List<HexTile>();
            foreach (var tile in _hexTiles.Values)
            {
                if (!activeOnly || tile.TileObject.activeInHierarchy)
                {
                    tiles.Add(tile);
                }
            }

            return tiles;
        }

        public List<GameObject> GetTilesObjects(bool activeOnly = true)
        {
            var tiles = new List<GameObject>();
            foreach (var tile in _hexTiles.Values)
            {
                if (!activeOnly || tile.TileObject.activeInHierarchy)
                {
                    tiles.Add(tile.TileObject);
                }
            }

            return tiles;
        
        }

        public Dictionary<(int q, int r), HexTile> GetAllTiles()
        {
            return _hexTiles;
        }
    
    

        public List<HexTile> GetNeighbors(int q, int r)
        {
            var neighbors = new List<HexTile>();
            int[][] directions = new int[][]
            {
                new int[] { 1, 0 }, new int[] { 1, -1 }, new int[] { 0, -1 },
                new int[] { -1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }
            };

            foreach (var dir in directions)
            {
                int nq = q + dir[0];
                int nr = r + dir[1];
                HexTile neighbor = GetTile(nq, nr);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    
        public void Clear()
        {
            // Destroy all hex tiles in the dictionary and reset the dictionary
            foreach (var tile in _hexTiles)
            {
                if (tile.Value.TileObject)
                {
                    Object.DestroyImmediate(tile.Value.TileObject);
                }
            
            }
            _hexTiles.Clear();
        }
    
        // Scuffed method to get the closest tile to a position
        public HexTile GetTileAtPosition(Vector3 position)
        {
            HexTile closestTile = null;
            var closestDistanceSqr = float.MaxValue;
            const float thresholdSqr = 1.5f * 1.5f;

            foreach (var tile in _hexTiles.Values)
            {
                if (tile.TileObject.activeInHierarchy)
                {
                    var tilePosition = tile.TileObject.transform.position;
                    var distanceSqr = (tilePosition - position).sqrMagnitude;

                    if (distanceSqr < closestDistanceSqr && distanceSqr < thresholdSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        closestTile = tile;


                        if (distanceSqr < 0.01f) 
                        {
                            break;
                        }
                    }
                }
            }
            return closestTile;
        }
    
    
        public void Disable()
        {
            foreach (var tile in _hexTiles)
            {
                if (tile.Value.TileObject)
                {
                    tile.Value.TileObject.SetActive(false);
                }
            }
        }
        public List<HexTile> GetTrueEdgeTiles()
        {
            // Step 1: Identify all tiles connected to the outer boundary using flood fill
            var connectedToOuterBoundary = FloodFillFromOuterEdge();

            // Step 2: Identify true edge tiles, excluding internal edges
            var trueEdgeTiles = new List<HexTile>();

            foreach (var tile in _hexTiles.Values)
            {
                if (tile.TileObject.activeInHierarchy && HasMissingOrInactiveNeighbors(tile.Q, tile.R))
                {
                    if (connectedToOuterBoundary.Contains((tile.Q, tile.R)))
                    {
                        trueEdgeTiles.Add(tile);
                    }
                }
            }

            return trueEdgeTiles;
        }

        private HashSet<(int, int)> FloodFillFromOuterEdge()
        {
            var visited = new HashSet<(int, int)>();
            var toVisit = new Queue<(int, int)>();

            // Start flood fill from all tiles on the outer boundary of the grid
            foreach (var tile in _hexTiles.Values)
            {
                if (tile.TileObject.activeInHierarchy && IsOnOuterBoundary(tile.Q, tile.R))
                {
                    toVisit.Enqueue((tile.Q, tile.R));
                    visited.Add((tile.Q, tile.R));
                }
            }

            int[][] directions = new int[][]
            {
                new int[] { 1, 0 }, new int[] { 1, -1 }, new int[] { 0, -1 },
                new int[] { -1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }
            };

            while (toVisit.Count > 0)
            {
                var (q, r) = toVisit.Dequeue();

                foreach (var dir in directions)
                {
                    int nq = q + dir[0];
                    int nr = r + dir[1];

                    var neighborTile = GetTile(nq, nr);
                    if (neighborTile != null && !visited.Contains((nq, nr)))
                    {
                        visited.Add((nq, nr));
                        toVisit.Enqueue((nq, nr));
                    }
                }
            }

            return visited;
        }

        private bool IsOnOuterBoundary(int q, int r)
        {
            // Check if the tile is on the outer boundary by seeing if it has any missing neighbors
            return HasMissingOrInactiveNeighbors(q, r);
        }

        private bool HasMissingOrInactiveNeighbors(int q, int r)
        {
            var directions = new int[][]
            {
                new int[] { 1, 0 }, new int[] { 1, -1 }, new int[] { 0, -1 },
                new int[] { -1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }
            };

            foreach (var dir in directions)
            {
                var nq = q + dir[0];
                var nr = r + dir[1];
                var neighborTile = GetTile(nq, nr);
                if (neighborTile == null || !neighborTile.TileObject.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }


        private Vector3 HexToWorld(int q, int r, float yOffset = 0f)
        {
            var x = _hexSize * (Mathf.Sqrt(3) * q + Mathf.Sqrt(3) / 2 * r);
            var z = _hexSize * (3f / 2f * r);
            var localPosition = new Vector3(x, yOffset, z);

            // Transform the local position according to the grid's tilt
            return _gridOrigin.TransformPoint(localPosition);
        }

        private bool IsTile(int q, int r)
        {
            var tile = GetTile(q, r);
            return tile != null;
        }
    }
}