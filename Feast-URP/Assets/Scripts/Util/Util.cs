using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            Cursor.visible = Visible;
            Cursor.lockState = LockMode;
        }

        public static CursorMode Default => new CursorMode(true, CursorLockMode.None);

        public static CursorMode GetCurrent() => new CursorMode(Cursor.visible, Cursor.lockState);

        public override bool Equals(object obj)
        {
            CursorMode other = (CursorMode)obj;
            return Visible == other.Visible && LockMode == other.LockMode;
        }

        public static bool operator ==(CursorMode c1, CursorMode c2) => c1.Equals(c2);
        public static bool operator !=(CursorMode c1, CursorMode c2) => !c1.Equals(c2);

        public override int GetHashCode()
        {
            return Visible.GetHashCode() ^ LockMode.GetHashCode();
        }
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
