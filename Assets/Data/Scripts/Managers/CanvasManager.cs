using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : CanvasBase
{
    public Slider _slider;
    public TextMeshProUGUI _text;

    public void AddListeners(float minValue, float maxValue)
    {
        base.AddListeners();
        _slider.onValueChanged.AddListener(HandleValueChange);
        _slider.minValue = minValue;
        _slider.maxValue = maxValue;
    }
    public void HandleValueChange(float f)
    {
        _value = f;
        _text.text = _value.ToString("0.00");
    }
    
}