using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    public float rotSpeed = 44;
    public float distanceFromTarget = 14f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    [SerializeField] Camera cam;

    private void Start()
    {
      //  if (LevelManager.Instance.CurrentLevel ==0)
            cam.fieldOfView = 90;
    }
    private void Update()
    {
        /*        if(Input.GetMouseButtonDown(0))
                {
                    previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
                }*/


        this.transform.LookAt(target);
        if (Input.GetMouseButton(0))
        {
           // Vector3 dir = previousPos - cam.ScreenToViewportPoint(Input.mousePosition);




            if (Input.GetMouseButton(0)) // Rotate when left mouse button is held down
            {
                float mouseX = Input.GetAxis("Mouse X") * rotSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotSpeed;

                xRotation += mouseY;
                yRotation += mouseX;

                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation

                Quaternion rotation = Quaternion.Euler(-xRotation, yRotation, 0);
                Vector3 negDistance = new(0.0f, 0.0f, -distanceFromTarget);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;
            }


            // cam.transform.position = field.position;
            /*
                        cam.transform.Rotate(new Vector3(1, 0, 0), dir.y * rotSpeed);
                        cam.transform.Rotate(new Vector3(0, 1, 0), -dir.x * rotSpeed, Space.World);
                        cam.transform.Translate(new Vector3(0, 0, -10));
            */
            //   previousPos = cam.ScreenToViewportPoint(Input.mousePosition);

        }
    }
}
