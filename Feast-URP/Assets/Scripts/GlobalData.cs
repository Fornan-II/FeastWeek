public static class GlobalData
{
    public static bool HasCompletedGame = false;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Misc/Set HasCompleted Game/True")]
    private static void SetHasCompletedGameTrue() => HasCompletedGame = true;
    [UnityEditor.MenuItem("Tools/Misc/Set HasCompleted Game/False")]
    private static void SetHasCompletedGameFalse() => HasCompletedGame = false;
#endif
}
