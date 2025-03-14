using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationSpeed = 15f;
    private bool isDragging = false;
    private bool canOperate = true; // �����Ƿ�ɲ�����־λ

    void Start()
    {
        LightRay.OnLevelCleared += DisableMirrorOperation; // ����ͨ���¼�
    }

    void OnDestroy()
    {
        LightRay.OnLevelCleared -= DisableMirrorOperation; // ȡ������ͨ���¼�
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
        canOperate = false; // ���þ��Ӳ���
    }
}