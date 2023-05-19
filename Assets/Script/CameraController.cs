using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform sphereTransform; // Tham chiếu tới Transform của Sphere
    public float distance = 10f; // Khoảng cách giữa camera và Sphere

    private Vector3 offset; // Độ lệch giữa vị trí camera và Sphere

    private void Start()
    {
        // Tính toán độ lệch ban đầu giữa camera và Sphere
        offset = transform.position - sphereTransform.position;
    }

    private void Update()
    {
        // Kiểm tra xem Sphere có tồn tại không
        if (sphereTransform != null)
        {
            // Cập nhật vị trí của camera dựa trên vị trí hiện tại của Sphere và khoảng cách
            Vector3 targetPosition = sphereTransform.position + offset.normalized * distance;
            targetPosition.x = transform.position.x; // Cố định trục x của camera
            transform.position = targetPosition;
        }
    }
}
