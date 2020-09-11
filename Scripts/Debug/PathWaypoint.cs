using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathWaypoint : MonoBehaviour
{
    public List<Transform> nodes = new List<Transform>();
    public Vector3 currentNode;
    public Vector3 prevNode;
    public int wayCount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {

        if(transform.childCount != wayCount)
        {
            nodes.Clear();
            wayCount = 0;
        }


        if (transform.childCount > 0)
        {
            foreach (Transform T in transform)
            {
                if (!nodes.Contains(T))
                {
                    nodes.Add(T);
                }
                wayCount++;
            }
        }
        if (nodes.Count >= 1)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                currentNode = nodes[i].position;
                if(i>0)
                {
                    prevNode = nodes[i - 1].position;
                }
                else if(i==0 && nodes.Count >1)
                {
                    prevNode = nodes[nodes.Count - 1].position;
                }
                Gizmos.color = Color.red;
                Gizmos.DrawLine(prevNode, currentNode);
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(currentNode, Vector3.one);

            }

        }
    }



}
