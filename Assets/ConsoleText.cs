using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleText : MonoBehaviour
{
    [SerializeField] private TMP_Text console_Text = default;

    private void OnEnable()
    {
        Globals.OnConsoleUpdated += AddTextToConsole;
    }

    private void OnDisable()
    {
        Globals.OnConsoleUpdated -= AddTextToConsole;
    }

    public IEnumerator AddTextToConsole(string text)
    {
        console_Text.text += $"{text} \n";
        yield return null;
    }
}
