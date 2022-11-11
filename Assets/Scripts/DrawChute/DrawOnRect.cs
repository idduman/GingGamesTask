using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DrawOnRect : MonoBehaviour
{
    [SerializeField] private RenderTexture _renderTexture;
    private RectTransform _rectTransform;
    
    private bool _selected;
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _renderTexture.width = (int)Mathf.Ceil(_rectTransform.rect.width);
        _renderTexture.height = (int)Mathf.Ceil(_rectTransform.rect.height);
    }
    
    public void OnPointerDown(BaseEventData data)
    {
        if (!(data is PointerEventData pData))
            return;
        
        _selected = true;
        var perc = CalculatePercentage(pData);
        Debug.Log(perc.ToString("F4"));
    }

    public void OnPointerMoved(BaseEventData data)
    {
        if (!(data is PointerEventData pData))
            return;
    }

    public void OnPointerUp(BaseEventData data)
    {
        if (!(data is PointerEventData pData))
            return;
        
        if(!_selected)
            return;

        _selected = false;
    }

    private Vector2 CalculatePercentage(PointerEventData pData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform,
            pData.position, pData.pressEventCamera, out var localPoint);

        var rect = _rectTransform.rect;
        return new Vector2(localPoint.x / rect.width, localPoint.y / rect.height);
    }
}
