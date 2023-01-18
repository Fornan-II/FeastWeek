using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct ViewData
{
    public Transform Parent;
    public float FieldOfView;

    public ViewData(Transform t)
    {
        Parent = t;
        FieldOfView = MainCamera.IsValid() ? MainCamera.Camera.fieldOfView : -1;
    }
}

[Serializable]

public struct ViewBlend
{
    ViewData Target;
    public AnimationCurve Blend;

    public ViewBlend(ViewData target, AnimationCurve blend)
    {
        Target = target;
        Blend = blend;
    }

    public ViewBlend(ViewData target, float duration)
    {
        Target = target;
        Blend = AnimationCurve.EaseInOut(0f, 0f, duration, 1f);
    }

    public IEnumerator GetTransition()
    {
        if(!MainCamera.IsValid())
        {
            Debug.LogError("MainCamera instance is invalid; unable to create camera transition.");
            yield break;
        }

        float duration = Util.AnimationCurveLengthTime(Blend);
        if(duration <= 0f)
        {
            MainCamera.RequestView(Target);
            yield break;
        }

        ViewData initial = new ViewData(MainCamera.RootTransform.parent);
        Vector3 initPos = MainCamera.RootTransform.position;
        Quaternion initRot = MainCamera.RootTransform.rotation;
        MainCamera.RootTransform.SetParent(null);

        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float t = Blend.Evaluate(timer / duration);
            MainCamera.RootTransform.SetPositionAndRotation(
                Vector3.Lerp(initPos, Target.Parent.position, t),
                Quaternion.Slerp(initRot, Target.Parent.rotation, t)
                );
            MainCamera.Camera.fieldOfView = Mathf.Lerp(initial.FieldOfView, Target.FieldOfView, t);
            yield return null;
        }

        MainCamera.RequestView(Target);
    }
}