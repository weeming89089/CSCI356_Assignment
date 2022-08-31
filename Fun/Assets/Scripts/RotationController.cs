using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private float HorizontalSens = 3.0f;
    [SerializeField] private float VerticalSens = 3.0f;

    private Vector3 offset;
    private bool lookAtTarget = true;
    float HorizontalRot;
    float VerticalRot;
    public GameObject character;
    public GameObject pivot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        offset = transform.InverseTransformDirection(transform.position - character.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX;
        float mouseY;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        HorizontalRot += mouseX * HorizontalSens;
        VerticalRot -= mouseY * VerticalSens;
        VerticalRot = Mathf.Clamp(VerticalRot, -35f, 70f);

        Quaternion camRot = Quaternion.Euler(0f, HorizontalRot, 0f);
        Quaternion playerRot = Quaternion.Euler(VerticalRot, 0f, 0f);

        character.transform.rotation = camRot;
        pivot.transform.rotation = playerRot;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void LateUpdate()
    {

        Vector3 newPosition = character.transform.position + character.transform.rotation * pivot.transform.rotation * offset;
        transform.position = newPosition;

        if (lookAtTarget)
        {
            transform.LookAt(character.transform);
        }
    }
}
