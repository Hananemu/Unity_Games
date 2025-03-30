using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public float maxDistance = 2f; // 玩家与镜子的最大距离
    public float centerTolerance = 0.1f; // 视角中心的容差范围
    public bool isSelected = false; // 是否被选中
    private bool canRotate = true; // 是否可以旋转镜子

    void Update()
    {
        if (!canRotate) return;

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