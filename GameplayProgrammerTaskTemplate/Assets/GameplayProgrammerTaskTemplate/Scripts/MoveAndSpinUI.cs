using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndSpinUI : MonoBehaviour
{
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _spinDuration;
    private Vector3 _offscreenPosition;
    private Vector3 _originalPosition;

    private RectTransform _rectTransform;
    private ScaleUI _scaleUI;

    private bool _hasMovedOntoScreen;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _scaleUI = GetComponent<ScaleUI>();

        _originalPosition = _rectTransform.anchoredPosition;
    }

    
    public void MoveFromOffscreen()
    {
        if (_hasMovedOntoScreen) return;

        _offscreenPosition = new Vector3(Random.Range(Screen.width/8, Screen.width/2), Screen.height);

        // Set the initial offscreen position
        _rectTransform.anchoredPosition = _offscreenPosition;

        // Create the move tween
        Tweener moveTween = _rectTransform.DOAnchorPos(_originalPosition, _moveDuration).SetEase(Ease.InOutQuad);

        // Create the rotation tween
        Tweener rotationTween = transform.DORotate(new Vector3(0, 0, 360), _spinDuration, RotateMode.FastBeyond360)
                                         .SetEase(Ease.Linear)
                                         .SetLoops(-1, LoopType.Restart)
                                         .SetRecyclable(true);

        // Play both tweens simultaneously
        moveTween.OnComplete(() => { rotationTween.Pause(); rotationTween.Rewind(); _scaleUI.ScaleGameObject(); }); // Stop rotation once the move is complete, then begin scaling animation
    }
    
    public void SetHasMovedBool(bool hasMoved)
    {
        _hasMovedOntoScreen = hasMoved;
    }
}
