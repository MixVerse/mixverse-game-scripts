using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deneme : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int totalVertexes = 0;
        int totalTriangles = 0;


        foreach (MeshFilter mf in FindObjectsOfType(typeof(MeshFilter)))
        {
            totalVertexes += mf.mesh.vertexCount;
            totalTriangles += mf.mesh.triangles.Length / 3;
        }
        Debug.Log("Vertexes: " + totalVertexes);
        Debug.Log("Triangles: " + totalTriangles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
