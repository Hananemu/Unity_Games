using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    private Transform cameraTra;
    private Camera cam;
    private float moveSpeed = 1f;
    private void Awake()
    {
        cameraTra = this.transform.Find("CameraTra");
        cam = cameraTra.GetComponentInChildren<Camera>();
        moveSpeed = 1f;

    }
    private void Update()
    {
        Vector3 tempMoveDir = Input.GetAxis("Horizontal")*this.transform.right+ Input.GetAxis("Vertical") * this.transform.forward;
        if (tempMoveDir != Vector3.zero) 
        {
            this.transform.position = Vector3.MoveTowards(
    this.transform.position,
    this.transform.position + tempMoveDir,
    Time.deltaTime * moveSpeed
);
        }
        if (Input.GetMouseButton(1)) //按住鼠标右键进行视角转换
        {
            this.transform.Rotate(Input.GetAxis("Mouse X") * Vector3.up);
            cameraTra.Rotate(-Input.GetAxis("Mouse Y")*Vector3.right);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (moveSpeed != 2f)
            {
                moveSpeed = 2f;
            }
        }
        else 
        {
            if (moveSpeed != 1f)
            {
                moveSpeed = 1f;
            }
        }
        if (Input.GetKey(KeyCode.Space)) 
        {
            if(cameraTra.localPosition.y != 0.9f) 
            {
                cameraTra.localPosition = new Vector3(cameraTra.localPosition.x,0.9f, cameraTra.localPosition.z);
            }
            if (moveSpeed != 0.5f)
            {
                moveSpeed = 0.5f;
            }
        }
        else 
        {
            if (cameraTra.localPosition.y != 1.5f)
            {
                cameraTra.localPosition = new Vector3(cameraTra.localPosition.x, 1.5f, cameraTra.localPosition.z);
            }
        }
    }
}
