using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationSpeed = 15f;
    private bool isDragging = false;
    private bool canOperate = true; // 镜子是否可操作标志位

    void Start()
    {
        LightRay.OnLevelCleared += DisableMirrorOperation; // 订阅通关事件
    }

    void OnDestroy()
    {
        LightRay.OnLevelCleared -= DisableMirrorOperation; // 取消订阅通关事件
    }

    void OnMouseDown()
    {
        if (canOperate)
        {
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (canOperate && isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void DisableMirrorOperation()
    {
        canOperate = false; // 禁用镜子操作
    }
}