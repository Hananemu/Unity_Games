using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public float maxDistance = 2f; // ����뾵�ӵ�������
    public float centerTolerance = 0.1f; // �ӽ����ĵ��ݲΧ
    public bool isSelected = false; // �Ƿ�ѡ��
    private bool canRotate = true; // �Ƿ������ת����

    void Update()
    {
        if (!canRotate) return;

        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateMirror(-rotationAngle); // ����Q����˳ʱ����ת
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateMirror(rotationAngle); // ����E������ʱ����ת
            }
        }
    }

    public void SelectMirror()
    {
        Mirror[] allMirrors = FindObjectsOfType<Mirror>();
        foreach (Mirror mirror in allMirrors)
        {
            mirror.isSelected = false;
        }

        isSelected = true;
    }

    void RotateMirror(float angle)
    {
        transform.Rotate(Vector3.up, angle);
    }

    public void DisableRotation()
    {
        canRotate = false;
    }
}