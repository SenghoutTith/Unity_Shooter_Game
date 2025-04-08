using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineCamera virtualCamera;
    private CinemachinePositionComposer transposer;

    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        virtualCamera = FindAnyObjectByType<CinemachineCamera>();
        
        if (virtualCamera == null)
        {
            return;
        }

        transposer = virtualCamera.GetComponent<CinemachinePositionComposer>();
    }

    private void Update() {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance() {
        if (canChangeCameraDistance == false) {
            return;
        }
        float currentDistance = transposer.CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentDistance) < .01f) {   
            return;
        }
        transposer.CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, Time.deltaTime * distanceChangeRate);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}
