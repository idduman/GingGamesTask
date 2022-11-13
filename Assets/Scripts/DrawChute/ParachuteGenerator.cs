using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class ParachuteGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private BoxCollider _boxCollider;
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    
    private Mesh _mesh;
    
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _boxCollider = GetComponent<BoxCollider>();

        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;
    }
    
    public void Generate(List<Vector2> points, Vector2 relativeBrushSize)
    {
        _vertices.Clear();
        _triangles.Clear();

        CalculateMesh(points, relativeBrushSize);
        
        //Re-generate mesh
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }

    private void CalculateMesh(List<Vector2> points, Vector2 thickness)
    {
        var extents = _boxCollider.bounds.extents;
        
        //Generate Vertices
        Vector2 dir = Vector2.zero;
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            var offset = new Vector3(2f * p.x * extents.x, 2f *  p.y * extents.y, -extents.z);
            
            if(i == 0)
                dir = Vector2.Perpendicular(points[i+1] - p).normalized;
            else
                dir = Vector2.Perpendicular(p - points[i-1]).normalized;

            var brushOffset = new Vector3(extents.x * thickness.x * dir.x,
                extents.y * thickness.y * dir.y, 0f);
            
            _vertices.Add(offset - brushOffset);
            _vertices.Add(offset + brushOffset);

        }
        //Generate Front Face
        for (int i = 1; i < _vertices.Count - 1; i++)
        {
            var v = _vertices[i];
            var vP = _vertices[i - 1] - v;
            var vN = _vertices[i + 1] - v;

            if (Vector2.SignedAngle(vP, vN) > 0)
            {
                _triangles.Add(i);
                _triangles.Add(i+1);
                _triangles.Add(i-1);
            }
            else
            {
                _triangles.Add(i);
                _triangles.Add(i-1);
                _triangles.Add(i+1);
            }
        }
        //Generate Back Face
        var frontVertexCount = _vertices.Count;
        for (int v = 0; v < frontVertexCount; v++)
        {
            _vertices.Add(_vertices[v] + 2f * extents.z * Vector3.forward);
        }

        for (int j = frontVertexCount + 1; j < _vertices.Count - 1; j++)
        {
            var v = _vertices[j];
            var vP = _vertices[j - 1] - v;
            var vN = _vertices[j + 1] - v;

            if (Vector2.SignedAngle(vP, vN) <= 0)
            {
                _triangles.Add(j);
                _triangles.Add(j+1);
                _triangles.Add(j-1);
            }
            else
            {
                _triangles.Add(j);
                _triangles.Add(j-1);
                _triangles.Add(j+1);
            }
        }
        //Generate Bottom Face
        for (int k = 2; k < _vertices.Count - 2; k += 2)
        {
            int p;
            int n;
            Vector3 vP;
            Vector3 vN;
            var v = _vertices[k];
            if (k < frontVertexCount)
            {
                p = k + frontVertexCount - 2;
                n = k + frontVertexCount;
                vP = _vertices[p] - v;
                vN = _vertices[n] - v;
            }
            else
            {
                p = k - frontVertexCount + 2;
                n = k - frontVertexCount;
                vP = _vertices[p] - v;
                vN = _vertices[n] - v;
            }

            if (Vector2.SignedAngle(vP, vN) <= 0)
            {
                _triangles.Add(k);
                _triangles.Add(n);
                _triangles.Add(p);
            }
            else
            {
                _triangles.Add(k);
                _triangles.Add(p);
                _triangles.Add(n);
            }
        }
        //Generate Top Face
        for (int k = 3; k < _vertices.Count - 1; k += 2)
        {
            int p;
            int n;
            Vector3 vP;
            Vector3 vN;
            var v = _vertices[k];
            if (k < frontVertexCount)
            {
                p = k + frontVertexCount - 2;
                n = k + frontVertexCount;
                vP = _vertices[p] - v;
                vN = _vertices[n] - v;
            }
            else
            {
                p = k - frontVertexCount + 2;
                n = k - frontVertexCount;
                vP = _vertices[p] - v;
                vN = _vertices[n] - v;
            }

            if (Vector2.SignedAngle(vP, vN) > 0)
            {
                _triangles.Add(k);
                _triangles.Add(n);
                _triangles.Add(p);
            }
            else
            {
                _triangles.Add(k);
                _triangles.Add(p);
                _triangles.Add(n);
            }
        }
        //Generate Sides
        _triangles.Add(0);
        _triangles.Add(frontVertexCount);
        _triangles.Add(1);
        _triangles.Add(frontVertexCount);
        _triangles.Add(frontVertexCount + 1);
        _triangles.Add(1);
        _triangles.Add(frontVertexCount-2);
        _triangles.Add(frontVertexCount-1);
        _triangles.Add(_vertices.Count - 1);
        _triangles.Add(frontVertexCount-2);
        _triangles.Add(_vertices.Count - 1);
        _triangles.Add(_vertices.Count - 2);
    }
}