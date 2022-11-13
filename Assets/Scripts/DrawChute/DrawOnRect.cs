using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
public class DrawOnRect : MonoBehaviour
{
    [SerializeField] private int _brushSize = 5;
    [SerializeField] private Color _brushColor = Color.black;
    [SerializeField] private bool _sample = true;
    [SerializeField] private ParachuteGenerator _parachute;
    
    private RectTransform _rectTransform;
    private RawImage _image;
    private Texture2D _texture;

    private Vector2 _previousPos;
    private bool _selected;
    private Vector2 _previousPerc;
    private Vector2Int _textureSize;
    private bool _moved;

    private List<Vector2> _sampledPoints = new List<Vector2>();
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<RawImage>();
        var width = (int)Mathf.Ceil(_rectTransform.rect.width);
        var height = (int)Mathf.Ceil(_rectTransform.rect.height);
        _textureSize = new Vector2Int(width, height);
        _texture = new Texture2D(width, height);
        _image.texture = _texture;
    }
    
    public void OnPointerDown(BaseEventData data)
    {
        if (!(data is PointerEventData pData))
            return;

        _previousPos = pData.position;
        _selected = true;
        var perc = CalculatePercentage(pData);
        _previousPerc = perc;
        _sampledPoints.Add(perc);
    }

    public void OnPointerMoved(BaseEventData data)
    {
        if (!(data is PointerEventData pData) 
            || !_selected || Vector2.Distance(_previousPos, pData.position) < 20f)
            return;

        _previousPos = pData.position;
        var perc = CalculatePercentage(pData);
        _moved = true;
        _sampledPoints.Add(perc);
        Paint(perc);
    }

    public void OnPointerUp(BaseEventData data)
    {
        if (!(data is PointerEventData pData) || !_selected)
            return;
        
        _selected = false;
        if (_moved)
        {
            _moved = false;
            _parachute.Generate(_sampledPoints, new Vector2(
                (float)_brushSize/_textureSize.x, (float)_brushSize/_textureSize.y));
            _sampledPoints.Clear();
        }
        _texture = new Texture2D(_textureSize.x, _textureSize.y);
        _image.texture = _texture;
    }

    private Vector2 CalculatePercentage(PointerEventData pData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform,
            pData.position, pData.pressEventCamera, out var localPoint);

        var rect = _rectTransform.rect;
        return new Vector2(localPoint.x / rect.width, localPoint.y / rect.height);
    }

    private void Paint(Vector2 percentage)
    {
        var prevXPos = _previousPerc.x * _texture.width;
        var prevYPos = _previousPerc.y * _texture.height;
        var xPos = percentage.x * _texture.width;
        var yPos = percentage.y * _texture.height;

        var prevPos = new Vector2(prevXPos, prevYPos);
        var pos = new Vector2(xPos, yPos);
        
        var prevPixelVector = Vector2Int.FloorToInt(prevPos);
        var pixelVector = Vector2Int.FloorToInt(pos);

        var px = Mathf.Min(prevPixelVector.x, pixelVector.x);
        var qx = Mathf.Max(prevPixelVector.x, pixelVector.x);
        var py = Mathf.Min(prevPixelVector.y, pixelVector.y);
        var qy = Mathf.Max(prevPixelVector.y, pixelVector.y);

        var pxClamp = Mathf.Max(px - _brushSize/2, 0);
        var qxClamp = Mathf.Min(qx + _brushSize/2, _texture.width);
        var pyClamp = Mathf.Max(py - _brushSize/2, 0);
        var qyClamp = Mathf.Min(qy + _brushSize/2, _texture.height);

        for (int x = pxClamp; x <= qxClamp; x++)
        {
            for (int y = pyClamp; y <= qyClamp; y++)
            {
                if(!CheckPointInBoundary(
                    new Vector2(x, y), prevPos, pos, _brushSize))
                    continue;

                _texture.SetPixel(x, y, _brushColor);
            }
        }
        _texture.Apply();
        _previousPerc = percentage;
    }

    private bool CheckPointInBoundary(Vector2 point, Vector2 lineP1, Vector2 lineP2, float brushSize)
    {
        var thicknessDist = CalculateDistanceToLine(point, lineP1, lineP2);

        var diff = lineP2 - lineP1;
        var midPoint = (lineP1 + lineP2) / 2f;
        var point2 = midPoint + Vector2.Perpendicular(diff.normalized);
        var midDist = CalculateDistanceToLine(point, midPoint, point2);

        return thicknessDist <= brushSize / 2f && midDist <= (diff.magnitude / 2f) + (brushSize / 4f);
    }

    private float CalculateDistanceToLine(Vector2 point, Vector2 lineP1, Vector2 lineP2)
    {
        var x2_x1 = (lineP2.x - lineP1.x);
        var y2_y1 = (lineP2.y - lineP1.y);
        return Mathf.Abs(x2_x1 * (lineP1.y - point.y) - y2_y1 * (lineP1.x - point.x))
               / Mathf.Sqrt(x2_x1 * x2_x1 + y2_y1 * y2_y1);
    }
}