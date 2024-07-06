using DG.Tweening;
using UnityEngine;

public class RotateUI : MonoBehaviour
{

    [Tooltip("The minimum duration of the rotation animation for this game object.")]
    [SerializeField] private float _minRotationDuration;
    [Tooltip("The maximum duration of the rotation animation for this game object.")]
    [SerializeField] private float _maxRotationDuration;
    [Tooltip("The duration of the rotation animation.")]
    private float _rotationDuration; 
    [Tooltip("The minimum angle of the rotation animation for this game object.")]
    [SerializeField] private float _minRotationAngle;
    [Tooltip("The maximum angle of the rotation animation for this game object.")]
    [SerializeField] private float _maxRotationAngle;
    [Tooltip("The angle of the rotation animation for this game object.")]
    private float _rotationAngle;

    private void Start()
    {
        _rotationDuration = Random.Range(_minRotationDuration, _maxRotationDuration);
        _rotationAngle = Random.Range(_minRotationAngle, _maxRotationAngle);
        RotateGameObject();
    }

    private void RotateGameObject()
    {
        transform.DORotate(new Vector3(0, 0, _rotationAngle), _rotationDuration)
                 .SetEase(Ease.InOutQuad) // Set the easing function to InOutQuad for smoother animation
                 .SetLoops(-1, LoopType.Yoyo); // Set to loop infinitely with Yoyo
    }
}
