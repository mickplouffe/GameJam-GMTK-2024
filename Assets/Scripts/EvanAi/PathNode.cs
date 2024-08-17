using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField] Node nodeprefab;
    public Node node;
    public List<Vector3> points;

    public float weight;
    public void Awake()
    {
        node = Instantiate(nodeprefab, transform.position, Quaternion.identity);
        node.transform.SetParent(this.transform);
        node.pn = this;
        //look for graphscript in scene


    }
    public void Start()
    {
        FindObjectOfType<Graphscript>().Addnode(node);
    }

}
