using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum SearchType
    {
        breadthfirst,
        hueristic,
        dijkstra,
        A
    };

    [SerializeField] float closedis;
    [SerializeField] float movespeed;
    [SerializeField] GameObject targetindicator;

    Graphscript gs;

    Node destination;

    [SerializeField]List<Node> route = new List<Node>();
    [SerializeField]int index = 0;
    
    public SearchType searchType;

    void Start()
    {
        gs = FindObjectOfType<Graphscript>();
        //NewDestination();
    }

    
    void FixedUpdate()
    {
        
        if (destination == null)
        {
            Debug.Log("Destination Null");
            NewDestination();
        }
        if (index >= route.Count)
        {
            NewDestination();
        }
        else
        {

            //transform.position += (this.transform.position + route[index].transform.position) * movespeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(this.transform.position, route[index].transform.position, movespeed * Time.fixedDeltaTime);

            if ((DistanceCheck(this.transform.position, destination.transform.position) <= closedis))
            {
                NewDestination();
            }

            if ((DistanceCheck(this.transform.position, route[index].transform.position) <= closedis))
            {
                index++;
            }
        }
        //targetindicator.transform.position = route[route.Count - 1].transform.position;
        targetindicator.transform.position = destination.transform.position;
    }

    public float DistanceCheck(Vector3 a, Vector3 b)
    {
        Vector3 c = a - b;

        return ((c.x * c.x) + (c.y * c.y) + (c.z * c.z));
    }

    public void NewDestination()
    {
   
        destination = gs.nodes[Random.Range(0, gs.nodes.Count)];
        //Debug.Log(gs.nodes.Capacity);
        switch (searchType)
        {
            case SearchType.breadthfirst:
                {
                    //Debug.Log(gs.FindClosestNode(transform.position).name + " to " + destination.pn.name);
                    
                    route = gs.BreadthFirstSearch(gs.FindClosestNode(transform.position).node, destination);
                    break;
                }
            case SearchType.hueristic:
                {
                    route = gs.HeuristicSearch(gs.FindClosestNode(transform.position).node, destination);
                    break;
                }
            case SearchType.dijkstra:
                {
                    route = gs.DijkstrasSearch(gs.FindClosestNode(transform.position).node, destination);
                    break;
                }
            case SearchType.A:
               {
                    route = gs.AStarSearch(gs.FindClosestNode(transform.position).node, destination);
                    break;
               }
        }
        route.Add(destination);
        index = 0;
    }

    public void OnDrawGizmos()
    {
        foreach (Node n in route)
        {
            //Gizmos.DrawSphere(n.transform.position + new Vector3(0, 4, 0), 3);
            Gizmos.DrawWireCube(n.transform.position + new Vector3(0, 4, 0), new Vector3(2,2,2));
        }
    }
}
