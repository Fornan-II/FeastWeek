using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New ViewBlend", menuName = "Data/New ViewBlend")]
public class ViewBlend : ScriptableObject
{
    public AnimationCurve Blend;

    public ViewBlend(AnimationCurve blend)
    {
        Blend = blend;
    }

    public ViewBlend(float duration)
    {
        Blend = AnimationCurve.EaseInOut(0f, 0f, duration, 1f);
    }

    public IEnumerator CreateBlend(CameraView targetView, Action OnBlendComplete = null)
    {
        if (!MainCamera.IsValid())
        {
            Debug.LogError("MainCamera instance is invalid; unable to create camera transition.");
            yield break;
        }

        // Determine duration and handle 0 duration blends
        float duration = Util.AnimationCurveLengthTime(Blend);
        if (duration <= 0f)
        {
            Util.MoveTransformToTarget(MainCamera.RootTransform, targetView.transform, true);
            MainCamera.Camera.fieldOfView = targetView.FieldOfView;
            yield break;
        }

        // Cache starting information
        Vector3 initPos = MainCamera.RootTransform.position;
        Quaternion initRot = MainCamera.RootTransform.rotation;
        float initFoV = MainCamera.Camera.fieldOfView;

        MainCamera.RootTransform.SetParent(null);

        // Blend
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float t = Blend.Evaluate(timer);

            MainCamera.RootTransform.SetPositionAndRotation(
                Vector3.Lerp(initPos, targetView.Position, t),
                Quaternion.Slerp(initRot, targetView.Rotation, t)
                );
            MainCamera.Camera.fieldOfView = Mathf.Lerp(initFoV, targetView.FieldOfView, t);

            yield return null;
        }

        // Finish up
        Util.MoveTransformToTarget(MainCamera.RootTransform, targetView.transform, true);
        MainCamera.Camera.fieldOfView = targetView.FieldOfView;

        OnBlendComplete?.Invoke();
    }
}