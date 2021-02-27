using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIFormattedNumberField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField,Min(0)] private int decimalPlaces = 1;

    public void SetValue(float value) => text.text = value.ToString("n1");

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
    }
#endif
}
