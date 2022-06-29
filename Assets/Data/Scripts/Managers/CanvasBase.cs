using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas _canvas;
    public Button _button;
    public Toggle _toggle;

    public float _value;

    public void AddListeners()
    {
        _button.onClick.AddListener(HandleDeleteButton);
        _toggle.onValueChanged.AddListener(HandleDeactivationButton);
        _canvas.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canvas.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       StartCoroutine(Countdown(2f));
    }

    protected IEnumerator Countdown(float f){
        yield return new WaitForSeconds(f);
        _canvas.enabled = false;
    }
    protected void HandleDeleteButton(){
        if (TryGetComponent(out Parameter p))
        {
            p.OnDeletion();
        }
        else if (TryGetComponent(out Sound s))
        {
            s.OnDeletion();
        }
        else if (TryGetComponent(out Hook t))
        {
            t.OnDeletion();
        }
    }
    protected void HandleDeactivationButton(bool b){
        if(b) gameObject.GetComponent<Node>().Activated = false;
        else gameObject.GetComponent<Node>().Activated = true;
    }
}
