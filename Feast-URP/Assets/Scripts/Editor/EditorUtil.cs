using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;

public static class EditorUtil
{
    [MenuItem("Tools/Open Scene/Level")]
    public static void OpenScene_Level() => EditorSceneManager.OpenScene("Assets/Scenes/Level.unity", OpenSceneMode.Single);
    [MenuItem("Tools/Open Scene/SampleScene")]
    public static void OpenScene_SampleScene() => EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);

    [MenuItem("Tools/Toggle Render Features")]
    public static void ToggleFog()
    {
        var renderPipeline = AssetDatabase.LoadAssetAtPath<ForwardRendererData>("Assets/Settings/ForwardRenderer.asset");
        foreach(var feature in renderPipeline.rendererFeatures)
        {
            feature.SetActive(!feature.isActive);
        }
    }
}
