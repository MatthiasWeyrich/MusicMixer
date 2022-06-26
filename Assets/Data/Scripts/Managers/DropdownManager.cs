using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    // when a Node was chosen from a dopdown, we invoke the event with the name of the node selected
    public Action<string, NodeCreator.Type> nodeFromDropdown;

    [SerializeField] private TMP_Dropdown soundDropdown;
    [SerializeField] private TMP_Dropdown hookDropdown;
    [SerializeField] private TMP_Dropdown modifierDropdown; 

    private List<string> sounds;
    private List<string> hooks;
    private List<string> modifier;

    void OnEnable()
    {
        // some dirty work
        sounds = new List<string>();
        hooks = new List<string>();
        modifier = new List<string>();
        soundDropdown.options.Clear();
        hookDropdown.options.Clear();
        modifierDropdown.options.Clear();
        addPredefinedHooksToDropdown();
        addPredefinedModifierToDropdown();
        hookDropdown.onValueChanged.AddListener(delegate { handleHookDropdown(); });
        soundDropdown.onValueChanged.AddListener(delegate { handleSoundDropdown(); });
        modifierDropdown.onValueChanged.AddListener(delegate { handleModifierDropdown(); });
    }

    public void AddNodeToDropdown(string soundName)
    {
        // adding the name of a newly uploaded sound to the list and dropdown
        if (!sounds.Contains(soundName))
        {
            sounds.Add(soundName);
            soundDropdown.options.Clear();
            soundDropdown.AddOptions(sounds);
        }
    }
    void handleSoundDropdown()
    {
        int index = soundDropdown.value;
        string soundName = soundDropdown.options[index].text;
        nodeFromDropdown?.Invoke(soundName, NodeCreator.Type.Sound);
    }
    void handleHookDropdown()
    {
        int index = hookDropdown.value;
        string hookName = hookDropdown.options[index].text;
        nodeFromDropdown?.Invoke(hookName,NodeCreator.Type.Hook);
    }
    void handleModifierDropdown(){
        int index = modifierDropdown.value;
        string modifierName = modifierDropdown.options[index].text;
        nodeFromDropdown?.Invoke(modifierName,NodeCreator.Type.Modifier);
    }
    // adding all predefined hooks to the list and dropdown
    void addPredefinedHooksToDropdown()
    {
        hooks.Add("Counter");
        hooks.Add("Numerator");
        hooks.Add("Guard");

        hookDropdown.AddOptions(hooks);
    }
    
    private void addPredefinedModifierToDropdown()
    {
        modifier.Add("Amplitude");
        modifier.Add("Echo");
        modifier.Add("Fade");
        modifier.Add("Pitch");
        modifier.Add("Priority");
        modifier.Add("Reverb");

        modifierDropdown.AddOptions(modifier);
    }
}
