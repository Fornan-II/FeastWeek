using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.ProBuilder;

public static class EditorUtil
{
    [MenuItem("Tools/Open Scene/Level")]
    private static void OpenScene_Level() => EditorSceneManager.OpenScene("Assets/Scenes/Level.unity", OpenSceneMode.Single);
    [MenuItem("Tools/Open Scene/SampleScene")]
    public static void OpenScene_SampleScene() => EditorSceneManager.OpenScene("Assets/Scenes/Sample Scene.unity", OpenSceneMode.Single);
    [MenuItem("Tools/Open Scene/Main Menu")]
    public static void OpenScene_MainMenu() => EditorSceneManager.OpenScene("Assets/Scenes/Main Menu.unity", OpenSceneMode.Single);
    [MenuItem("Tools/Open Scene/Dark Forest")]
    public static void OpenScene_DarkRoom() => EditorSceneManager.OpenScene("Assets/Scenes/Dark Forest.unity", OpenSceneMode.Single);

    [MenuItem("Tools/Misc/Fix lingering screen fade")]
    public static void FixLingeringScreenFade() => Shader.SetGlobalFloat("_ScreenFade", 0f);

    [MenuItem("Tools/Toggle Render Features")]
    private static void ToggleFog()
    {
        var renderPipeline = AssetDatabase.LoadAssetAtPath<ForwardRendererData>("Assets/Settings/Renderer Data/Main Renderer.asset");
        foreach (var feature in renderPipeline.rendererFeatures)
        {
            feature.SetActive(!feature.isActive);
        }
    }

    [MenuItem("Tools/Bakery Helpers/Invisible Colliders/Enable")]
    private static void EnableInvisibleColliders() => InvisibleColliders(true);
    [MenuItem("Tools/Bakery Helpers/Invisible Colliders/Disable")]
    private static void DisableInvisibleColliders() => InvisibleColliders(false);

    private static void InvisibleColliders(bool enabled)
    {

        MeshRenderer[] renderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        Material probuilderInvisMaterial = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.probuilder/Content/Resources/Materials/Collider.mat");
        foreach (var r in renderers)
        {
            if (r.sharedMaterial == probuilderInvisMaterial)
            {
                Undo.RecordObject(r.gameObject, $"{(enabled ? "Enable" : "Disable")} Invisible Colliders");
                r.gameObject.SetActive(enabled);
            }
        }
    }

    #region Player
    [MenuItem("Tools/Player Control/Take Control of Player")]
    private static void TakeControlOfPlayer()
    {
        Controller c = GameObject.FindObjectOfType<Controller>();
        if (!c)
        {
            Debug.LogError("Could not find Controller in scene.");
            return;
        }

        FPSChar playerPawn = GameObject.FindObjectOfType<FPSChar>();
        if (!playerPawn)
        {
            Debug.LogError("Could not find FPSChar in scene.");
            return;
        }

        if (!UnityEditor.EditorApplication.isPlaying)
            Undo.RecordObject(c, "Take Control of Player");

        c.TakeControlOf(playerPawn);
    }

    [MenuItem("Tools/Player Control/Prep Player for Game")]
    private static void PrepPlayerForGame()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogError("Can not prep player for game while game is already playing!");
            return;
        }

        Controller c = GameObject.FindObjectOfType<Controller>();
        FPSChar playerPawn = null;
        if (c)
        {
            if (c.ControlledPawn && c.ControlledPawn is FPSChar)
            {
                Undo.RecordObject(c, "Prep Player for Game");
                playerPawn = c.ControlledPawn as FPSChar;
                c.ReleaseControl();
            }
        }

        if (!playerPawn)
            playerPawn = GameObject.FindObjectOfType<FPSChar>();
        if (!playerPawn)
        {
            Debug.LogError("Could not find FPSChar in scene.");
            return;
        }

        Checkpoint defaultCheckpoint = Checkpoint.DefaultCheckPoint;
        if (!defaultCheckpoint)
        {
            Debug.LogError("Could not find Default Checkpoint in scene.");
            return;
        }

        Undo.RecordObject(playerPawn.transform, "Prep Player for Game");
        Util.MoveTransformToTarget(playerPawn.transform, defaultCheckpoint.transform);
    }
    #endregion

    [MenuItem("Tools/Bakery Helpers/Area Light/On")]
    private static void BakeryAreaLightOn()
    {
        var bakeryLights = Resources.FindObjectsOfTypeAll<BakeryLightMesh>();
        foreach (var light in bakeryLights)
            light.gameObject.SetActive(true);
    }

    [MenuItem("Tools/Bakery Helpers/Area Light/Off")]
    private static void BakeryAreaLightOff()
    {
        var bakeryLights = GameObject.FindObjectsOfType<BakeryLightMesh>();
        foreach (var light in bakeryLights)
            light.gameObject.SetActive(false);
    }

    [MenuItem("Tools/Misc/Select all probuilder meshes.")]
    private static void SelectAllProbuilderMeshes()
    {
        ProBuilderMesh[] proBuilderMeshes = GameObject.FindObjectsOfType<ProBuilderMesh>();
        GameObject[] selection = new GameObject[proBuilderMeshes.Length];
        for (int i = 0; i < proBuilderMeshes.Length; ++i)
        {
            selection[i] = proBuilderMeshes[i].gameObject;
        }
        Selection.objects = selection;
    }

    [MenuItem("Tools/Misc/Make meshRenderers not use reflection.")]
    private static void MakeMeshRenderersNotUseReflection()
    {
        MeshRenderer[] allMR = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (var mr in allMR)
        {
            if (mr.gameObject.isStatic && mr.tag != "Use Reflection Probes")
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }
    }

    [MenuItem("Tools/Misc/Toggle Screen Invert Color Value")]
    private static void ToggleInvertColorValue()
    {
        Shader.SetGlobalFloat("_InvertValue", 1 - Shader.GetGlobalFloat("_InvertValue"));
    }

    [MenuItem("Tools/Misc/Unlock Cursor")]
    private static void UnlockCursor() => Util.CursorMode.Default.Apply();

    [MenuItem("Tools/Misc/Bake Selected Particles")]
    private static void BakeSelectedParticlesMesh()
    {
        if (!Selection.activeGameObject)
        {
            Debug.LogError("Could not bake particles - no ParticleSystem selected.");
            return;
        }


        ParticleSystemRenderer sourceParticles;
        if (!Selection.activeGameObject.TryGetComponent(out sourceParticles))
        {
            Debug.LogError("Could not bake particles - Selection is not ParticleSystem.");
            return;
        }

        GameObject newMesh = new GameObject(string.Format("Baked Mesh - {0}", Selection.activeGameObject.name));
        newMesh.transform.position = Selection.activeTransform.position;
        newMesh.transform.rotation = Selection.activeTransform.rotation;
        newMesh.transform.localScale = Selection.activeTransform.lossyScale;

        MeshFilter meshFilter = newMesh.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.name = Selection.activeGameObject.name;

        Camera[] sceneCameras = SceneView.GetAllSceneCameras();
        if (sceneCameras.Length > 0 && !EditorApplication.isPlaying)
            sourceParticles.BakeMesh(meshFilter.sharedMesh, sceneCameras[0]);
        else
            sourceParticles.BakeMesh(meshFilter.sharedMesh);

        MeshRenderer meshRenderer = newMesh.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = sourceParticles.sharedMaterial;
    }
}
