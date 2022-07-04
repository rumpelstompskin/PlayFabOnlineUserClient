using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabSample : MonoBehaviour
{
    public static PlayFabSample Instance;

    [field: SerializeField, Tooltip("PlayFab Display Name")]
    public string PlayFabDisplayName { get; set; } = default;

    [field: SerializeField, Tooltip("PlayFab ID")]
    public string PlayFabID { get; set; } = default;

    [field: SerializeField, Tooltip("PlayFab current Network ID")]
    public string PlayFabNetworkID { get; set; } = default;

    [field: SerializeField, Tooltip("Certificate Name")]
    public string ServerName { get; set; } = default;

    private void Awake()
    {
        if(Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }
}
