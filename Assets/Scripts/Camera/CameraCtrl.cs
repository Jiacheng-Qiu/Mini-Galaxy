using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public float sensitivity = 17f;
    // Update is called once per frame
    void Update()
    {
        float fov = gameObject.GetComponent<Camera>().fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        fov = Mathf.Clamp(fov, 35, 70);
        gameObject.GetComponent<Camera>().fieldOfView = fov;
    }
}
