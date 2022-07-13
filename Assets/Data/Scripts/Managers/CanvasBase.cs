using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasBase : MonoBehaviour
{
    public Button _button;
    public Toggle _toggle;

    public float _value;

    public void AddListeners()
    {
        _button.onClick.AddListener(HandleDeleteButton);
        _toggle.onValueChanged.AddListener(HandleDeactivationButton);
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
        gameObject.GetComponent<Node>().Activated = b;
    }
}