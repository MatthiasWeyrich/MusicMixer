using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : CanvasBase
{
    public Slider _slider;
    public TextMeshProUGUI _text;

    public void AddListeners(float minValue, float maxValue, bool wholeNumbers)
    {
        base.AddListeners();
        _slider.onValueChanged.AddListener(HandleValueChange);
        _slider.minValue = minValue;
        _slider.maxValue = maxValue;
        _slider.wholeNumbers = wholeNumbers;
    }
    
    public void HandleValueChange(float f)
    {
        _value = f;
        _text.text = _value.ToString("0.00");
    }
    
}