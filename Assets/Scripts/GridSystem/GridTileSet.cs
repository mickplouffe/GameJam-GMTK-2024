using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    [CreateAssetMenu(fileName = "GridTileSet", menuName = "GridSystem/GridTileSet", order = 1)]
    [Serializable]
    public class GridTileSet : ScriptableObject
    {
        public List<GameObject> tiles;

        private GridTileSet()
        {
            tiles = new List<GameObject>();
            
        }
    
        // private void ValidateDefaultTile()
        // {
        //     GameObject defaultHexGridTile = tiles.Find(tile => tile.name == "DefaultHexGridTile");
        //
        //     if (defaultHexGridTile == null)
        //     {
        //         Debug.LogWarning("DefaultHexGridTile not found!");
        //     
        //         // Get the DefaultHexGridTile prefab that have the Tag HexTile
        //         // string[] hexTilesStrings = AssetDatabase.FindAssets("t:prefab", new[] {"Assets/_Prefabs/"});
        //         
        //         List<GameObject> hexTiles = new List<GameObject>();
        //         // foreach (string hexTileString in hexTilesStrings)
        //         // {
        //         //     GameObject hexTile = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(hexTileString));
        //         //     if (hexTile.CompareTag("HexTile"))
        //         //     {
        //         //         hexTiles.Add(hexTile);
        //         //     }
        //         // }
        //         
        //         foreach (GameObject hexTile in hexTiles)
        //         {
        //             if (hexTile.name != "DefaultHexGridTile") continue;
        //         
        //             tiles.Add(hexTile);
        //             break;
        //
        //         }
        //         return;
        //     }
        //
        //     if (tiles.Count <= 1) return;
        //
        //     tiles.Remove(defaultHexGridTile);
        //     tiles.Insert(0, defaultHexGridTile);
        // }
    
        public GameObject GetRandomTile()
        {
            return tiles[Random.Range(0, tiles.Count)];
        }
    
        public GameObject GetTile(int index)
        {
            if (index < 0 || index >= tiles.Count)
            {
                Debug.LogWarning("Index out of range!");
                return tiles[0];
            }

            return tiles[index];
        }
    
        public int GetTileIndex(GameObject tile)
        {
            return tiles.IndexOf(tile);
        }
    
        public void AddTile(GameObject tile)
        {
            tiles.Add(tile);
        }
    
        public void RemoveTile(GameObject tile)
        {
            tiles.Remove(tile);
        }
    
        public void RemoveTileAt(int index)
        {
            tiles.RemoveAt(index);
        }
    
        public void ClearTiles()
        {
            tiles.Clear();
            //ValidateDefaultTile();
        }
    
        public int GetTileCount()
        {
            return tiles.Count;
        }
    
        public void SetTile(int index, GameObject tile)
        {
            tiles[index] = tile;
        }
    
        public void InsertTile(int index, GameObject tile)
        {
            tiles.Insert(index, tile);
        }
    
        public void SwapTiles(int index1, int index2)
        {
            (tiles[index1], tiles[index2]) = (tiles[index2], tiles[index1]);
        }
    
        public void ShuffleTiles()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                int randomIndex = Random.Range(i, tiles.Count);
                (tiles[i], tiles[randomIndex]) = (tiles[randomIndex], tiles[i]);
            }
        }
    
        public void SortTiles()
        {
            tiles.Sort((a, b) => a.name.CompareTo(b.name));
        }
    
        public void ReverseTiles()
        {
            tiles.Reverse();
        }
    
        public void RemoveNullTiles()
        {
            tiles.RemoveAll(tile => tile == null || tile.name == "");
        }
    
        public void RemoveDuplicateTiles()
        {
            tiles = new List<GameObject>(new HashSet<GameObject>(tiles));
        }
    
        
        private void OnValidate()
        {
            //ValidateDefaultTile();
        }
    
    

    }
}
