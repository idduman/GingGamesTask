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
        var center = _boxCollider.bounds.center;
        var extents = _boxCollider.bounds.extents;
        var startPoint = _boxCollider.bounds.min;
        var offsetY = extents.y * relativeBrushSize.y * Vector3.up;
        
        _vertices.Clear();
        _triangles.Clear();
        foreach (var p in points)
        {
            var offset = new Vector3(p.x * extents.x, p.y * extents.y, 0f);
            _vertices.Add(startPoint + offset + offsetY);
            _vertices.Add(startPoint + offset - offsetY);
        }

        for (int i = 0; i < _vertices.Count - 2; i++)
        {
            if (_vertices[i + 2].x >= _vertices[i].x)
            {
                if (i % 2 == 0)
                {
                    _triangles.Add(i);
                    _triangles.Add(i+1);
                    _triangles.Add(i+2);
                }
                else
                {
                    _triangles.Add(i);
                    _triangles.Add(i+2);
                    _triangles.Add(i+1);
                }
            }
            else
            {
                if (i % 2 == 0)
                {
                    _triangles.Add(i);
                    _triangles.Add(i+2);
                    _triangles.Add(i+3);
                }
                else
                {
                    _triangles.Add(i);
                    _triangles.Add(i-1);
                    _triangles.Add(i+2);
                }
            }
        }
        
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
    }
}