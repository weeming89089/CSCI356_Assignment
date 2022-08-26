using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private float HorizontalSens = 3.0f;
    [SerializeField] private float VerticalSens = 3.0f;

    private Vector3 offset;
    private float smoothFactor = 0.5f;
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



        //Vector2 mouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        //mouseMove = Vector2.Scale(mouseMove, new Vector2(HorizontalSens, VerticalSens));
        //character.transform.Rotate(0, mouseMove.x, 0); 
        //pivot.transform.Rotate(-mouseMove.y, 0, 0);


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
