using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    // When a node was chosen from a dropdown, we invoke the event with the name of the node selected
    // recipient is the Assignment class
    public Action<string, NodeType> NodeFromDropdown;

    [SerializeField] private TMP_Dropdown _soundDropdown;
    [SerializeField] private TMP_Dropdown _hookDropdown;
    [SerializeField] private TMP_Dropdown _modifierDropdown; 
    [SerializeField] private TMP_Dropdown _filterDropdown;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _filterText;
    [SerializeField] private AudioMixer _mixer;

    private List<string> _sounds;
    private List<string> _hooks;
    private List<string> _modifier;
    private List<string> _filter;

    FILTER _currentFilter = FILTER.NONE;
    private enum FILTER{
        HighPassFrequency, LowPassFrequency, NONE
    }

    void OnEnable()
    {
        // some dirty work
        _sounds = new List<string>();
        _hooks = new List<string>();
        _modifier = new List<string>();
        _filter = new List<string>();
        _soundDropdown.options.Clear();
        _hookDropdown.options.Clear();
        _modifierDropdown.options.Clear();
        _filterDropdown.options.Clear();
        AddPredefinedHooksToDropdown();
        AddPredefinedModifierToDropdown();
        AddPredefinesFilterToDropwdown();
        _hookDropdown.onValueChanged.AddListener(delegate { HandleHookDropdown(); });
        _soundDropdown.onValueChanged.AddListener(delegate { HandleSoundDropdown(); });
        _modifierDropdown.onValueChanged.AddListener(delegate { HandleModifierDropdown(); });
        _filterDropdown.onValueChanged.AddListener(delegate { HandleFilterDropdown(); });
        _slider.onValueChanged.AddListener(HandleSliderChange);
        _slider.maxValue = 22000f;
        _slider.minValue = 10f;
        _slider.gameObject.SetActive(false);
    }

    public void AddNodeToDropdown(string soundName)
    {
        // adding the name of a newly uploaded sound to the list and dropdown
        if (!_sounds.Contains(soundName))
        {
            _sounds.Add(soundName);
            _soundDropdown.options.Clear();
            _soundDropdown.AddOptions(_sounds);
        }
    }
    
    void HandleSoundDropdown()
    {
        int index = _soundDropdown.value;
        string soundName = _soundDropdown.options[index].text;
        NodeFromDropdown?.Invoke(soundName, NodeType.Sound);
    }
    
    void HandleHookDropdown()
    {
        int index = _hookDropdown.value;
        string hookName = _hookDropdown.options[index].text;
        NodeFromDropdown?.Invoke(hookName,NodeType.Hook);
    }
    
    void HandleModifierDropdown(){
        int index = _modifierDropdown.value;
        string modifierName = _modifierDropdown.options[index].text;
        // Names in the Modifier Dropdown have other names than in code.
        // Reassigning them to align them with their names in code.
        switch(modifierName){
            case "Volume":
                break;
         //   case "Speed":
         //       modifierName = "PitchSpeed"; break; 
            case "Chorus Depth":
                modifierName = "ChorusDepth"; break;
            case "Chorus Rate":
                modifierName = "ChorusRate"; break;
            case "Echo":
                modifierName = "EchoDryMix"; break;
            case "Flange":
                modifierName = "FlangeDryMix"; break;
            case "Flange Depth":
                modifierName = "FlangeDepth"; break;
            case "Flange Rate":
                modifierName = "FlangeRate"; break;
            case "Octave":
                modifierName = "ParamEQOctaveRange"; break;
            case "Frequency Gain":
                modifierName = "ParamEQFrequencyGain"; break;
            case "Pitch":
                modifierName = "PitchShifterPitch"; break;
            case "Reverb": 
                modifierName = "ReverbRoom"; break;
        }
        NodeFromDropdown?.Invoke(modifierName,NodeType.Modifier);
    }

    void HandleFilterDropdown()
    {
        int index = _filterDropdown.value;
        string filterName = _filterDropdown.options[index].text;
        _filterText.text = filterName;
        _mixer.SetFloat("HighPassFrequency",_slider.minValue);
        _mixer.SetFloat("LowPassFrequency",_slider.maxValue);
        switch(filterName){
            case "NONE":
            _slider.gameObject.SetActive(false);
            _currentFilter = FILTER.NONE;   
            break;
            case "Lowpass":
            _slider.SetValueWithoutNotify(_slider.maxValue);
            _slider.gameObject.SetActive(true);
            _currentFilter = FILTER.LowPassFrequency;
            break;
            case "Highpass":
            _slider.SetValueWithoutNotify(_slider.minValue);
            _slider.gameObject.SetActive(true);
            _currentFilter = FILTER.HighPassFrequency;
            break;
        }
    }
    
    void HandleSliderChange(float f){
        _mixer.SetFloat(_currentFilter.ToString(),f);
    }
    
    void AddPredefinedHooksToDropdown()
    {
        _hooks.Add("Counter");
        _hooks.Add("Numerator");
        _hooks.Add("Guard");

        _hookDropdown.AddOptions(_hooks);
    }
    
    private void AddPredefinedModifierToDropdown()
    {
        _modifier.Add("Volume");
        //modifier.Add("Speed");
        _modifier.Add("Chorus Depth");
        _modifier.Add("Chorus Rate");
        _modifier.Add("Echo");
        _modifier.Add("Flange");
        _modifier.Add("Flange Depth");
        _modifier.Add("Flange Rate");
        _modifier.Add("Octave");
        _modifier.Add("Frequency Gain");
        _modifier.Add("Pitch");
        _modifier.Add("Reverb");

        _modifierDropdown.AddOptions(_modifier);
    }

    private void AddPredefinesFilterToDropwdown()
    {
        _filter.Add("NONE");
        _filter.Add("Lowpass");
        _filter.Add("Highpass");
        _filterDropdown.AddOptions(_filter);
    }
}