using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class Util
{
    public static Vector3 XZVector3(Vector2 value, float yValue) => new Vector3(value.x, yValue, value.y);
    public static Vector3 XZVector3(Vector2 value) => XZVector3(value, 0f);

    // https://www.xarg.org/2017/07/how-to-map-a-square-to-a-circle/
    public static Vector2 RemapSquareToCircle(Vector2 value) => new Vector2(
        value.x * Mathf.Sqrt(1f - value.y * value.y / 2f),
        value.y * Mathf.Sqrt(1f - value.x * value.x / 2f)
        );

    public static Vector3 LimitVector3(Vector3 value, float limit) => value.sqrMagnitude > limit * limit ? value.normalized * limit : value;

    public static float RandomInRange(Vector2 range) => Random.Range(range.x, range.y);

    public static float AnimationCurveLengthTime(AnimationCurve curve) => curve[curve.length - 1].time;

    public static float RoundToPlaces(float value, int decimalPlaces = 0)
    {
        float scaleValue = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * scaleValue) / scaleValue;
    }

    public static float Remap(float value, float inMin, float inMax, float outMin, float outMax) => outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);

    public static T RandomFromCollection<T>(IList<T> collection) => collection[Random.Range(0, collection.Count)];

    public static void ShuffleCollection<T>(IList<T> collection)
    {
        // https://stackoverflow.com/a/110570
        int n = collection.Count;
        while(n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = collection[n];
            collection[n] = collection[k];
            collection[k] = temp;
        }
    }

    public static void UndoDontDestroyOnLoad(GameObject gameObject) => SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

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
            CursorVisibilityManager.CursorVisibility = Visible;
            Cursor.lockState = LockMode;
        }

        public static CursorMode Default => new CursorMode(true, CursorLockMode.None);

        public static CursorMode GetCurrent() => new CursorMode(CursorVisibilityManager.CursorVisibility, Cursor.lockState);

        public override bool Equals(object obj)
        {
            // Potentially use hashcode to avoid garbage made from unboxing?
            CursorMode other = (CursorMode)obj;
            return Visible == other.Visible && LockMode == other.LockMode;
        }

        public static bool operator ==(CursorMode c1, CursorMode c2) => c1.Equals(c2);
        public static bool operator !=(CursorMode c1, CursorMode c2) => !c1.Equals(c2);

        public override int GetHashCode() => Visible.GetHashCode() ^ LockMode.GetHashCode();
    }

    [System.Serializable]
    public class PawnEvent : UnityEvent<Pawn> { }

    public static void MoveTransformToTarget(Transform transform, Transform target, bool setTargetAsParent = false)
    {
        transform.SetPositionAndRotation(target.position, target.rotation);

        if (setTargetAsParent)
            transform.SetParent(target);
    }
}
