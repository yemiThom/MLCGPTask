using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUI : MonoBehaviour
{
    [Tooltip("The minimum duration of the animation for this game object.")]
    [SerializeField] private float _minScaleDuration;
    [Tooltip("The maximum duration of the animation for this game object.")]
    [SerializeField] private float _maxScaleDuration;
    [Tooltip("The duration of the animation for this game object.")]
    private float _scaleDuration;
    [Tooltip("The minimum rate as which the game object should scale for this animation.")]
    [SerializeField] private float _minScaleRate;
    [Tooltip("The maximum rate as which the game object should scale for this animation.")]
    [SerializeField] private float _maxScaleRate;
    [Tooltip("The rate as which the game object should scale for this animation.")]
    private float _scaleRate;

    private void Start()
    {
        _scaleDuration = Random.Range(_minScaleDuration, _maxScaleDuration);
        _scaleRate = Random.Range(_minScaleRate, _maxScaleRate);
        ScaleGameObject();
    }

    private void ScaleGameObject()
    {
        transform.DOScale(_scaleRate, _scaleDuration)
                 .SetEase(Ease.InOutQuad) // Set the easing function to InOutQuad for smoother animation
                 .SetLoops(-1, LoopType.Yoyo); // Set to loop infinitely with Yoyo
    }
}
