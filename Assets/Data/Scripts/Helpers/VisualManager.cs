using UnityEngine;
using UnityEngine.EventSystems;

public class VisualManager : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Renderer _r;
    public Color _defaultC;

    void OnEnable(){
        _r = gameObject.GetComponentInChildren<Renderer>();
        _defaultC = _r.material.color;
    }

    public void SetColor(Color c)
    {
        _r.material.color = c;
    }

    public void ResetColor()
    {
        _r.material.color = _defaultC;
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
