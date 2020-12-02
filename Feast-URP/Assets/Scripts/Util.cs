using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Util
{
    public static Vector3 XZVector3(Vector2 value) => new Vector3(value.x, 0, value.y);
    public static Vector3 XZVector3(Vector2 value, float yValue) => new Vector3(value.x, yValue, value.y);

    public static float RandomInRange(Vector2 range) => Random.Range(range.x, range.y);

    public static float AnimationCurveLengthTime(AnimationCurve curve) => curve[curve.length - 1].time;

    public static float RoundToPlaces(float value, int decimalPlaces = 0)
    {
        float scaleValue = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * scaleValue) / scaleValue;
    }

    public static T RandomFromCollection<T>(IList<T> collection) => collection[Random.Range(0, collection.Count)];

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

        public static CursorMode GetCurrent() => new CursorMode(Cursor.visible, Cursor.lockState);
    }

    [System.Serializable]
    public class PawnEvent : UnityEvent<Pawn> { }
}
