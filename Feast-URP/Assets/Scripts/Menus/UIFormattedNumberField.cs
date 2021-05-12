using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIFormattedNumberField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField,Min(0)] private int decimalPlaces = 1;
    [SerializeField] private float scalar = 1.0f;

    public void SetValue(float value) => text.text = (value * scalar).ToString(string.Format("n{0}", decimalPlaces));

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
    }
#endif
}
