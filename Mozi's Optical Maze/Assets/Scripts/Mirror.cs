using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public float maxDistance = 2f; // 玩家与镜子的最大距离
    public float centerToleranceRange = 0.1f; // 视角中心的容差范围
    public bool isSelected = false; // 是否被选中
    private bool canRotate = true; // 是否可以旋转镜子

    private static List<Mirror> allMirrors = new List<Mirror>(); // 存储所有镜子对象

    void Awake()
    {
        // 在初始化时将当前镜子添加到列表中
        allMirrors.Add(this);
    }

    void OnDestroy()
    {
        // 在销毁时从列表中移除当前镜子
        allMirrors.Remove(this);
    }

    void Update()
    {
        if (!CanRotate()) return;

        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateMirror(-rotationAngle); // 按下Q键，顺时针旋转
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateMirror(rotationAngle); // 按下E键，逆时针旋转
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
        // 遍历所有镜子并取消选中
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