using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporaryInputs : MonoBehaviour
{
    public TMP_InputField PlayFabID;
    public TMP_InputField DisplayName;

    public void SetInfo()
    {
        PlayFabSample.Instance.PlayFabID = PlayFabID.text;
        PlayFabSample.Instance.PlayFabDisplayName = DisplayName.text;
    }
}
