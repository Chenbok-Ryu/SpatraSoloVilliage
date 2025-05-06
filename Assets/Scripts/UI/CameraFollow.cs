using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("������ �÷��̾� Transform")]
    public Transform target;

    [Tooltip("ī�޶�� �÷��̾� ������ ������ �Ÿ�")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("ī�޶��� �ε巯�� ����")]
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
