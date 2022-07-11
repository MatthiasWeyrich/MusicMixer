using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image _r;
    public Color _defaultC;

    void OnEnable(){
        _r = gameObject.GetComponentInChildren<Image>();
        _defaultC = _r.color;
    }

    public void SetColor(Color c)
    {
        _r.color = c;
    }

    public void ResetColor()
    {
        _r.color = _defaultC;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    public void OnDrop(PointerEventData eventData)
    {
    }
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}