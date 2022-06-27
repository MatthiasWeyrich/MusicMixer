using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Intermediary : Node
{
    public Canvas canvas;
    public Slider slider;
    public Button Button;
    public TextMeshProUGUI text;
    public float minValue, maxValue;
    public float value;
    public virtual void addListeners()
    {
        Button.onClick.AddListener(handleDeleteButton);
        slider.onValueChanged.AddListener(handleValueChange);
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        canvas.enabled = false;
    }
    // Intermediary class between Nodes and more advanced types (sound, hook, modifier)
    // we need this to simplify the check whether we've hit a VALID node in the OnEndDrag() function in the node class
    public override void OnDrag(PointerEventData eventData)
    {
        // updating the line, drawing, along the mouse position
        if (Input.GetMouseButton(0))
        {
            Vector2 position = GetMouseWorldPos();
            if (Vector2.Distance(position, skeleton.linePositions[skeleton.linePositions.Count - 1]) > .2f)
                skeleton.UpdateLine(position);
        }
        // just moving the node along the mouse position
        else if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - mouseOffset;
                // Making Intermediary Nodes able to be deleted
            //if (Input.GetKey(KeyCode.D)) OnDeletion();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        canvas.enabled = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(Countdown(2f));
    }

    IEnumerator Countdown(float f){
        yield return new WaitForSeconds(f);
        canvas.enabled = false;
    }

    public void handleDeleteButton(){
        base.OnDeletion();
    }
    public void handleValueChange(float f)
    {
        value = f;
        text.text = "Value: " + value.ToString("0.00");
    }
}
   