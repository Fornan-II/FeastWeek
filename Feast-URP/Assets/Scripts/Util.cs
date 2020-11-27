using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 XZVector3(Vector2 value) => new Vector3(value.x, 0, value.y);
    public static Vector3 XZVector3(Vector2 value, float yValue) => new Vector3(value.x, yValue, value.y);

    public static float RandomInRange(Vector2 range) => Random.Range(range.x, range.y);

    public static float AnimationCurveLengthTime(AnimationCurve curve) => curve[curve.length - 1].time;

    [System.Serializable]
    public struct CursorMode
    {
        public bool Visible;
        public CursorLockMode LockMode;

        public CursorMode(bool visible, CursorLockMode lockMode)
        {
            Visible = visible;
            LockMode = lockMode;
        }

        public void Apply()
        {
            Cursor.visible = Visible;
            Cursor.lockState = LockMode;
        }

        public static CursorMode Default => new CursorMode(true, CursorLockMode.None);
    }
}
