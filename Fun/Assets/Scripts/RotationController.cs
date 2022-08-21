using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private float HorizontalSens = 3.0f;
    [SerializeField] private float VerticalSens = 3.0f;

    public GameObject character;
    public GameObject pivot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseMove = Vector2.Scale(mouseMove, new Vector2(HorizontalSens, VerticalSens));
        character.transform.Rotate(0, mouseMove.x, 0); 
        pivot.transform.Rotate(-mouseMove.y, 0, 0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
