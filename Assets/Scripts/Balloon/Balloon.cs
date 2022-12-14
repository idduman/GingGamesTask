using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Joint))]
public class Balloon : MonoBehaviour
{
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _bottomPoint;
    [SerializeField] private float _maxSize = 1;
    private Rigidbody _rb;
    private LineRenderer _lineRenderer;
    private Joint _joint;

    public bool Active;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        transform.localScale = Vector3.zero;
        _joint = GetComponent<Joint>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!Active)
            return;

        //_joint.anchor = -0.5f * transform.up;
        var scale = transform.localScale;
        if(scale.magnitude < (_maxSize * Vector3.one).magnitude)
            transform.localScale += Vector3.one * Time.fixedDeltaTime;
        
        _rb.AddForceAtPosition(Vector3.up * (12f * Time.fixedDeltaTime), _topPoint.position);
    }

    private void Update()
    {
        if (!Active)
            return;
        
        _lineRenderer.SetPosition(0, _bottomPoint.position);
        _lineRenderer.SetPosition(1, _joint.connectedBody.position);
    }

    public void SetConnectedBody(Rigidbody other)
    {
        _joint.connectedBody = other;
    }
}
