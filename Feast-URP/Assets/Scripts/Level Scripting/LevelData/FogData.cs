using UnityEngine;

[CreateAssetMenu(fileName = "New Fog Data", menuName = "Data/New Fog Data")]
public class FogData : ScriptableObject
{
    private const string _fogStartDistanceProperty = "_FogStartDistance";
    private const string _fogEndDistanceProperty = "_FogEndDistance";
    private const string _fogExponentProperty = "_FogExponent";
    private const string _fogColorProperty = "_FogColor";

    //public float FogStartDistance => fogStartDistance;
    //public float FogEndDistance => fogEndDistance;
    //public float FogExponent => fogExponent;

#pragma warning disable 0649
    [SerializeField,ColorUsage(true,true)] private Color fogColor;
    [SerializeField] private float fogStartDistance;
    [SerializeField] private float fogEndDistance;
    [SerializeField] private float fogExponent;

    [ContextMenu("Apply fog settings")]
    public void Apply()
    {
        Shader.SetGlobalColor(_fogColorProperty, fogColor);
        Shader.SetGlobalFloat(_fogStartDistanceProperty, fogStartDistance);
        Shader.SetGlobalFloat(_fogEndDistanceProperty, fogEndDistance);
        Shader.SetGlobalFloat(_fogExponentProperty, fogExponent);
    }

#if UNITY_EDITOR
    public static void SetNULLFog()
    {
        Shader.SetGlobalColor(_fogColorProperty, Color.clear);
        Shader.SetGlobalFloat(_fogStartDistanceProperty, 0f);
        Shader.SetGlobalFloat(_fogEndDistanceProperty, 0f);
        Shader.SetGlobalFloat(_fogExponentProperty, 0f);
    }
#endif
}
