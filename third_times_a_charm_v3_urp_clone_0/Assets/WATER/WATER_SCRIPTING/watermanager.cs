using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class watermanager : MonoBehaviour
{
    private MeshFilter MeshFilter;

    private void Awake()
    {
        MeshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        Vector3[] vertices = MeshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = WaveManager.instance.GetWaveHeight(transform.position.x + vertices[i].x);
        }
        MeshFilter.mesh.vertices = vertices;
        MeshFilter.mesh.RecalculateNormals();

    }
}
