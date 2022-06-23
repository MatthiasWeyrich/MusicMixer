using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    public Action<string, NodeCreator.Type> nodeFromDropdown;

    [SerializeField] private TMP_Dropdown soundDropdown;
    [SerializeField] private TMP_Dropdown hookDropdown;

    private List<string> sounds;
    private List<string> hooks;

    void OnEnable()
    {
        sounds = new List<string>();
        hooks = new List<string>();
        soundDropdown.options.Clear();
        hookDropdown.options.Clear();
        addPredefinedHooksToDropdown();
        hookDropdown.onValueChanged.AddListener(delegate { handleHookDropdown(); });
        soundDropdown.onValueChanged.AddListener(delegate { handleSoundDropdown(); });
        
    }
    public void AddNodeToDropdown(string soundName)
    {
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
    void addPredefinedHooksToDropdown()
    {
        hooks.Add("Counter");
        hooks.Add("Numerator");

        hookDropdown.AddOptions(hooks);
    }
}
