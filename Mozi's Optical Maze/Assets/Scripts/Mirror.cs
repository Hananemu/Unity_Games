using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public float maxDistance = 2f; // ����뾵�ӵ�������
    public float centerToleranceRange = 0.1f; // �ӽ����ĵ��ݲΧ
    public bool isSelected = false; // �Ƿ�ѡ��
    private bool canRotate = true; // �Ƿ������ת����

    private static List<Mirror> allMirrors = new List<Mirror>(); // �洢���о��Ӷ���

    void Awake()
    {
        // �ڳ�ʼ��ʱ����ǰ������ӵ��б���
        allMirrors.Add(this);
    }

    void OnDestroy()
    {
        // ������ʱ���б����Ƴ���ǰ����
        allMirrors.Remove(this);
    }

    void Update()
    {
        if (!CanRotate()) return;

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

    private bool CanRotate()
    {
        return canRotate;
    }

    public void SelectMirror()
    {
        DeselectAllMirrors();
        isSelected = true;
    }

    private void DeselectAllMirrors()
    {
        // �������о��Ӳ�ȡ��ѡ��
        foreach (Mirror mirror in allMirrors)
        {
            mirror.isSelected = false;
        }
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