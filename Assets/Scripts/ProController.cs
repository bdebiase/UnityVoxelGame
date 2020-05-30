using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProController : MonoBehaviour {
    //INSTANCE VARIABLES
    public GameObject inventory;
    public GameObject crosshair;
    public float speed, accel, mouseSensitivity;

    private GameObject _world;
    private Vector3 _velocity;
    private Vector3 _cameraRot;
    private float _originalFov;

    //run at start of program
    private void Start() {
        _originalFov = GetComponent<Camera>().fieldOfView;
        _world = GameObject.Find("World");
    }

    //run every frame
    public void Update() {
        //camera vectors
        var camF = transform.forward;
        var camR = transform.right;
        camF.y = 0;
        camR.y = 0;
        camF.Normalize();
        camR.Normalize();

        //change speed based on player state
        float tempSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) {
            tempSpeed = speed * 2;
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, _originalFov * 1.1f, accel * Time.deltaTime);
        } else {
            tempSpeed = speed;
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, _originalFov, accel * Time.deltaTime);
        }

        //change height
        if (Input.GetButton("Jump")) {
            _velocity.y = Mathf.Lerp(_velocity.y, tempSpeed, accel * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.LeftShift)) {
            _velocity.y = Mathf.Lerp(_velocity.y, -tempSpeed, accel * Time.deltaTime);
        }

        //interpolate velocity
        _velocity = Vector3.Lerp(_velocity, (Input.GetAxisRaw("Horizontal") * camR + Input.GetAxisRaw("Vertical") * camF) * tempSpeed, accel * Time.deltaTime);

        //update position
        transform.position += _velocity * Time.deltaTime;

        //hide cursor
        if (inventory.activeSelf || _world.GetComponent<World>().paused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        //place/destroy block
        if (Input.GetMouseButton(1)) {
            _cameraRot.x += Input.GetAxis("Mouse X") * (PlayerPrefs.GetInt("Sensitivity") / 10f);
            _cameraRot.y = Mathf.Min(89, Mathf.Max(-89, _cameraRot.y + -Input.GetAxis("Mouse Y") * (PlayerPrefs.GetInt("Sensitivity") / 10f)));
            transform.localRotation = Quaternion.Euler(_cameraRot.y, _cameraRot.x, 0);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            crosshair.SetActive(true);
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
    }
}
