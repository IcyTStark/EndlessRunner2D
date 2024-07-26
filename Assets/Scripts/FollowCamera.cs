using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FollowCamera : MonoBehaviour
{
    [Header("Camera Component: ")]
    [SerializeField] private Camera _camera;

    [Header("Camera Properties: ")]
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private Vector3 _desiredPosition;

    [SerializeField] private Vector3 _cameraOffset;

    private Vector3 _velocity = Vector3.zero;

    [SerializeField]
    [Range(0f, 5f)]
    public float CameraSmoothSpeed = 1.1f;

    public UnityEvent OnRetryClicked;

    private void Start()
    {
        OnRetryClicked.AddListener(OnRetry);
    }

    private void LateUpdate()
    {
        _desiredPosition = _playerTransform.position + _cameraOffset;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _velocity, CameraSmoothSpeed);
    }

    private void OnRetry()
    {
        _velocity = Vector3.zero;
        
        _desiredPosition = _playerTransform.position + _cameraOffset;

        transform.position = _desiredPosition;
    }

#if UNITY_EDITOR
    //private void OnValidate()
    //{
    //    _desiredPosition = _playerTransform.position + _cameraOffset;

    //    transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _velocity, CameraSmoothSpeed);
    //}
#endif
}
