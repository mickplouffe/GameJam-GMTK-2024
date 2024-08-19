//https://www.geeksforgeeks.org/breadth-first-search-or-bfs-for-a-graph/
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graphscript: MonoBehaviour 
{
    //struct before i learned about dictionaries
    struct link
    {
        public Node HomeNode;
        public Node AddedNode;
        public float weight;
    }
    [SerializeField] Node startnode;
    public List<Node> nodes = new List<Node>();
    [SerializeField] float maxdistance;
    
    public void Addnode(Node newnode)
    {
        nodes.Add(newnode);
        
        //checks for paths between all other nodes in the scene
        foreach(PathNode p in FindObjectsOfType<PathNode>()) 
        {
            if (p != newnode.pn)
            {
                //sees if there is an uninterrupted path between the too nodes
                RaycastHit hit;

                if (Physics.Raycast(p.transform.position, newnode.transform.position - p.transform.position, out hit, (DistanceCheck(newnode.transform.position, p.transform.position))))
                {
                    if (hit.collider == null)
                    {
                        Debug.Log("no coll");
                    }
                    else
                    {
                        Debug.Log(hit.collider.gameObject.name);
                    }
                }
                else
                {
                    if (Vector3.Distance(p.transform.position, newnode.transform.position) <= maxdistance)
                    {
                        //adds refrences between connected nodes
                        p.node.connectednodes.Add(newnode);
                        
                        //Debug.Log("and here");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach(PathNode p in FindObjectsOfType<PathNode>())
        {
            Gizmos.DrawSphere(p.transform.position, 1);
        }

        foreach (Node i in nodes)
        {
            foreach (Node n in i.connectednodes)
            {
                if (n != null)
                {
                    Gizmos.DrawLine(n.transform.position, i.transform.position);
                }
            }
        }
    }

    public PathNode FindClosestNode(Vector3 aPosition)
    {
        float closedis = Mathf.Infinity;

        PathNode closestnode = nodes[0].pn;

        foreach (Node node in nodes)
        {
            if (DistanceCheck(aPosition, node.transform.position) < closedis)
            {
                closedis = DistanceCheck(aPosition, node.transform.position);
                closestnode = node.pn;
            }
        }

        return closestnode;
    }
    public List<Node> BreadthFirstSearch(Node start, Node end)
    {
        if (start.connectednodes.Count > 0)
        {

            List<Node> shortest = new List<Node>();
            Queue<Node> frontier = new Queue<Node>();
            List<Node> seen = new List<Node>();
            List<link> links = new List<link>();

            foreach (Node n in start.connectednodes)
            {
                if (n != null)
                {

                    frontier.Enqueue(n);
                    link l = new link();
                    l.HomeNode = start;
                    l.AddedNode = n;
                    links.Add(l);
                }
            }

            seen.Add(start);

            while (frontier.Count > 0)
            {
                Debug.Log("inloop");
                Node currentnode = frontier.Dequeue();

                if (currentnode != null)
                {
                        foreach (Node p in currentnode.connectednodes)
                        {
                            if (p != null)
                            {
                                if (!frontier.Contains(p) && !seen.Contains(p))
                                {
                                    frontier.Enqueue(p);

                                link l = new link();
                                l.HomeNode = currentnode;
                                l.AddedNode = p;
                                links.Add(l);

                                }

                               

                            }
                        }
                    
                }

                seen.Add(currentnode);

                if (currentnode == end)
                {

                    break;
                }
            }
            Debug.Log("loop done");
           
            Node current = end;
            List<Node> BeenTo = new List<Node>();


            while (current != start)
            {
                //go backwards through list of links, getting the home node from the addednode
                foreach (link g in links)
                {
                    if (g.AddedNode == current && !BeenTo.Contains(current))
                    {
                        shortest.Insert(0, g.HomeNode);
                        BeenTo.Add(current);
                        current = g.HomeNode;
                        
                        
                    }
                }
            }

            return shortest;
        }
        else
        {
            Debug.Log("get nulled idiot");
            return null; 
        }

    }
    public List<Node> HeuristicSearch(Node start, Node end)
    {
        if (start.connectednodes.Count > 0)
        {

            List<Node> shortest = new List<Node>();
            PQ<Node> frontier = new PQ<Node>();
            List<Node> seen = new List<Node>();
            List<link> links = new List<link>();

            foreach (Node n in start.connectednodes)
            {
                if (n != null)
                {

                    frontier.Enqueue(n, (int)DistanceCheck(start.transform.position, n.transform.position));
                    link l = new link();
                    l.HomeNode = start;
                    l.AddedNode = n;
                    links.Add(l);
                }
            }

            seen.Add(start);

            while (frontier.Count() > 0)
            {
                Debug.Log("inloop");
                Node currentnode = frontier.Dequeue();

                if (currentnode != null)
                {
                    foreach (Node p in currentnode.connectednodes)
                    {
                        if (p != null)
                        {
                            if (!frontier.contains(p) && !seen.Contains(p))
                            {
                                frontier.Enqueue(p, (int)DistanceCheck(currentnode.transform.position, p.transform.position));

                                link l = new link();
                                l.HomeNode = currentnode;
                                l.AddedNode = p;
                                links.Add(l);

                            }



                        }
                    }

                }

                seen.Add(currentnode);

                if (currentnode == end)
                {

                    break;
                }
            }
            Debug.Log("loop done");

            Node current = end;
            List<Node> BeenTo = new List<Node>();


            while (current != start)
            {
                //go backwards through list of links, getting the home node from the addednode
                foreach (link g in links)
                {
                    if (g.AddedNode == current && !BeenTo.Contains(current))
                    {
                        shortest.Insert(0, g.HomeNode);
                        BeenTo.Add(current);
                        current = g.HomeNode;


                    }
                }
            }

            return shortest;
        }
        else
        {
            Debug.Log("get nulled idiot");
            return null;
        }
    }
       
    
        
        
    
    public List<Node> DijkstrasSearch(Node start, Node end)
    {
        if (start.connectednodes.Count > 0)
        {

            List<Node> shortest = new List<Node>();
            PQ<Node> frontier = new PQ<Node>();
            List<Node> seen = new List<Node>();
            List<link> links = new List<link>();

            foreach (Node n in start.connectednodes)
            {
                if (n != null)
                {

                    frontier.Enqueue(n, (int)DistanceCheck(start.transform.position, n.transform.position));
                    link l = new link();
                    l.HomeNode = start;
                    l.AddedNode = n;
                    links.Add(l);
                }
            }

            seen.Add(start);

            while (frontier.Count() > 0)
            {
                Debug.Log("inloop");
                Node currentnode = frontier.Dequeue();

                if (currentnode != null)
                {
                    foreach (Node p in currentnode.connectednodes)
                    {
                        if (p != null)
                        {
                            if (!frontier.contains(p) && !seen.Contains(p))
                            {
                                link l = new link();
                                l.HomeNode = currentnode;
                                l.AddedNode = p;
                                links.Add(l);

                                Node temp = p;
                                List<Node> done = new List<Node> ();
                                float totalweight = 0f;

                                while (temp != start)
                                {
                                    //go backwards through list of links, getting the home node from the addednode
                                    foreach (link g in links)
                                    {
                                        if (g.AddedNode == temp && !done.Contains(temp))
                                        {
                                           
                                            done.Add(temp);
                                            temp = g.HomeNode;
                                            totalweight += temp.pn.weight;

                                        }
                                    }
                                }

                                frontier.Enqueue(p, (int)totalweight + 1);

                                

                            }
                        }
                    }

                }

                seen.Add(currentnode);

                if (currentnode == end)
                {

                    break;
                }
            }
            Debug.Log("loop done");

            Node current = end;
            List<Node> BeenTo = new List<Node>();


            while (current != start)
            {
                //go backwards through list of links, getting the home node from the addednode
                foreach (link g in links)
                {
                    if (g.AddedNode == current && !BeenTo.Contains(current))
                    {
                        shortest.Insert(0, g.HomeNode);
                        BeenTo.Add(current);
                        current = g.HomeNode;


                    }
                }
            }

            return shortest;
        }
        else
        {
            Debug.Log("get nulled idiot");
            return null;
        }
    }
    public List<Node> AStarSearch(Node start, Node end)
    {
        if (start.connectednodes.Count > 0)
        {

            List<Node> shortest = new List<Node>();
            PQ<Node> frontier = new PQ<Node>();
            List<Node> seen = new List<Node>();
            List<link> links = new List<link>();

            foreach (Node n in start.connectednodes)
            {
                if (n != null)
                {

                    frontier.Enqueue(n, (int)DistanceCheck(start.transform.position, n.transform.position));
                    link l = new link();
                    l.HomeNode = start;
                    l.AddedNode = n;
                    links.Add(l);
                }
            }

            seen.Add(start);

            while (frontier.Count() > 0)
            {
                Debug.Log("inloop");
                Node currentnode = frontier.Dequeue();

                if (currentnode != null)
                {
                    foreach (Node p in currentnode.connectednodes)
                    {
                        if (p != null)
                        {
                            if (!frontier.contains(p) && !seen.Contains(p))
                            {
                                link l = new link();
                                l.HomeNode = currentnode;
                                l.AddedNode = p;
                                links.Add(l);

                                Node temp = p;
                                List<Node> done = new List<Node>();
                                float totalweight = 0f;

                                while (temp != start)
                                {
                                    //go backwards through list of links, getting the home node from the addednode
                                    foreach (link g in links)
                                    {
                                        if (g.AddedNode == temp && !done.Contains(temp))
                                        {

                                            done.Add(temp);
                                            temp = g.HomeNode;
                                            totalweight += temp.pn.weight;

                                        }
                                    }
                                }

                                frontier.Enqueue(p, (int)totalweight + 1 + (int)DistanceCheck(p.transform.position, currentnode.transform.position));



                            }
                        }
                    }

                }

                seen.Add(currentnode);

                if (currentnode == end)
                {

                    break;
                }
            }
            Debug.Log("loop done");

            Node current = end;
            List<Node> BeenTo = new List<Node>();


            while (current != start)
            {
                //go backwards through list of links, getting the home node from the addednode
                foreach (link g in links)
                {
                    if (g.AddedNode == current && !BeenTo.Contains(current))
                    {
                        shortest.Insert(0, g.HomeNode);
                        BeenTo.Add(current);
                        current = g.HomeNode;


                    }
                }
            }

            return shortest;
        }
        else
        {
            Debug.Log("get nulled idiot");
            return null;
        }
    }

    public float DistanceCheck(Vector3 a, Vector3 b)
    {
        Vector3 c = a - b;

        return ((c.x * c.x) + (c.y * c.y) + (c.z * c.z));
    }
}
