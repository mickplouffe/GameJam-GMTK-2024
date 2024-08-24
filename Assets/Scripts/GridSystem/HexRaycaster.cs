using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRaycaster : MonoBehaviour
{
    Camera mainCamera;
    
    public LayerMask hexLayer;
    private static readonly int Removed = Animator.StringToHash("Removed");

    void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // if left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer))
            {
                // Get the hex tile that was clicked
                HexTile hexTile = HexGridManager.Instance.GetTileAtPosition(hit.point);
                //GameObject tile = HexGridManager.Instance.GetTileObject(hexTile);
                hexTile.TileObject.GetComponent<Animator>().SetTrigger(Removed);
                
            }
        }
    }
}
