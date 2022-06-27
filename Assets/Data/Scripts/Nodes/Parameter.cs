using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public abstract class Parameter : Node
{
    public Canvas canvas;
    public Slider slider;
    public Button Button;
    public TextMeshProUGUI text;
    public string paramName;
    public float minValue, maxValue;
    public float value;

    // This will in classes of actual modifiers (parameters) change the Slider's min and max value.
    public void addListeners()
    {
        Button.onClick.AddListener(handleDeleteButton);
        slider.onValueChanged.AddListener(handleValueChange);
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        canvas.enabled = false;
    }
    // overriding since you can't draw lines from a Parameter.
    // You can only connect sounds with parameters when starting from a sound node
    // this restriction is set for simplicity reasons and ease of development
    public override void OnBeginDrag(PointerEventData eventData)
    {
        mouseOffset = transform.position - GetMouseWorldPos();
        if (Input.GetMouseButton(1))
        {
            DeleteLinesDueToMovement?.Invoke(id);
            for (int i = outgoingLines.Count - 1; i >= 0; i--)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
        // same reasoning as above. One shouldn't be able to draw a line that's outgoing from a parameter
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - mouseOffset;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // supposed to be empty
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        canvas.enabled = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
       StartCoroutine(Countdown(2f));
    }

    // When the mouse leaves the object, we'll give it about 2 seconds until the Node UI disappears
    // Bugs as of now:
        /// If you're sliding the value with the slider, your mouse is no longer on the object. The user has thus only 2 seconds until the UI fades. 
    IEnumerator Countdown(float f){
        yield return new WaitForSeconds(f);
        canvas.enabled = false;
    }

    // delete button handler
    public void handleDeleteButton(){
        base.OnDeletion();
    }

    // slider interaction handler
    public void handleValueChange(float f)
    {
        value = f;
        text.text = "Value: " + value.ToString("0.00");
    }

    public override void OnContinueCommand()
    {
        paused = false;
    }

    public override void OnStartCommand()
    {
        paused = false;
    }

    public override void onStopCommand()
    {
        paused = true;
    }

    public override void Interact()
    {
        // supposed to be empty
    }
}
