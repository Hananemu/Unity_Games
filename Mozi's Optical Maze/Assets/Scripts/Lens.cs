using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 透镜类型枚举
public enum LensType
{
    Convex, // 凸透镜
    Concave // 凹透镜
}

public class Lens : MonoBehaviour
{
    public LensType lensType;
    public float focalLength; // 焦距

    // 可以添加更多属性和方法，例如可视化焦距等

    private void OnDrawGizmosSelected()
    {
        // 可视化焦距
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