using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("추적할 플레이어 Transform")]
    public Transform target;

    [Tooltip("카메라와 플레이어 사이의 고정된 거리")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("카메라의 부드러운 무빙")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 diriredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, diriredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
