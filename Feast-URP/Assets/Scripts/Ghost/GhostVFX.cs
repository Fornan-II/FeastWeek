using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostVFX : MonoBehaviour
{
    private const string k_GhostAlpha = "_AlphaMultiplier";

    public bool IsVisible => _isVisible || particles.isPlaying;

    [Header("Components")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private RandomBlink blink;
    [Header("Properties")]
    [SerializeField] private float visibilityTransitionTime = 1.0f;

    private bool _isVisible = false;
    private bool _shouldBeVisible = false;
    private Coroutine _activeVisibilityRoutine;

    private void OnEnable() => BecomeVisible();
    private void OnDisable() => _activeVisibilityRoutine = null;

    #region Visibility
    public void BecomeVisible()
    {
        _shouldBeVisible = true;
        if (_activeVisibilityRoutine == null)
            _activeVisibilityRoutine = StartCoroutine(HandleVisibility());
    }

    public void BecomeInvisible()
    {
        _shouldBeVisible = false;
        if (_activeVisibilityRoutine == null)
            _activeVisibilityRoutine = StartCoroutine(HandleVisibility());
    }

    private IEnumerator HandleVisibility()
    {
        _isVisible = true;
        float visibilityTimer = _shouldBeVisible ? 0.0f : visibilityTransitionTime;
        bool transitionInProgress = true;
        bool particlesActive = !_shouldBeVisible;
        blink.enabled = false;

        while(transitionInProgress)
        {
            float t = Mathf.Clamp01(visibilityTimer / visibilityTransitionTime);
            meshRenderer.material.SetFloat(k_GhostAlpha, t);
            blink.SetOpenness(Util.Remap(t, 0.25f, .75f, 0, 1));

            if(t <= 0.5f && particlesActive)
            {
                particles.Stop();
                particlesActive = false;
            }
            else if(t > 0.5f && !particlesActive)
            {
                particles.Play();
                particlesActive = true;
            }

            yield return null;

            visibilityTimer += Time.deltaTime * (_shouldBeVisible ? 1 : -1);
            if((_shouldBeVisible && visibilityTimer > visibilityTransitionTime) || (!_shouldBeVisible && visibilityTimer <= 0f))
            {
                transitionInProgress = false;
            }
        }

        if(_shouldBeVisible)
        {
            meshRenderer.material.SetFloat(k_GhostAlpha, 1f);
            blink.SetOpenness(1f);
            blink.enabled = true;
            if (!particles.isStopped) particles.Play();
        }
        else
        {
            meshRenderer.material.SetFloat(k_GhostAlpha, 0f);
            blink.SetOpenness(0f);
            if (particles.isPlaying) particles.Stop();
        }

        _isVisible = _shouldBeVisible;
        _activeVisibilityRoutine = null;
    }
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        particles = particles ?? GetComponent<ParticleSystem>();
        blink = blink ?? GetComponent<RandomBlink>();
    }

    [ContextMenu("Become Visible")]
    private void Editor_BecomeVisible()
    {
        if (UnityEditor.EditorApplication.isPlaying) BecomeVisible();
    }

    [ContextMenu("Become Invisible")]
    private void Editor_BecomeInvisible()
    {
        if (UnityEditor.EditorApplication.isPlaying) BecomeInvisible();
    }
#endif
}
