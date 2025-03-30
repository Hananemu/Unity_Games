using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ͸������ö��
public enum LensType
{
    Convex, // ͹͸��
    Concave // ��͸��
}

public class Lens : MonoBehaviour
{
    public LensType lensType;
    public float focalLength; // ����

    // ������Ӹ������Ժͷ�����������ӻ������

    private void OnDrawGizmosSelected()
    {
        // ���ӻ�����
        if (lensType == LensType.Convex)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * focalLength);
            Gizmos.DrawSphere(transform.position + transform.forward * focalLength, 0.1f);
        }
        else if (lensType == LensType.Concave)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.forward * focalLength);
            Gizmos.DrawSphere(transform.position - transform.forward * focalLength, 0.1f);
        }
    }
}