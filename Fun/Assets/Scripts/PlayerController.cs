using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event System.Action OnReachEndOfLevel;

    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] GameObject alienCenter;

    bool disabled;

    private void Start()
    {
        alienCenter.GetComponent<Renderer>().enabled = false;
        GuardController.OnGuardHasSpottedPlayer += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        float mvX = 0f;
        float mvZ = 0f;

        if (!disabled)
        {
            mvX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
            mvZ = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        }

        transform.Translate(mvX, 0f, mvZ);
        alienCenter.transform.position = transform.position;
    }

    void Disable()
    {
        disabled = true;
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachEndOfLevel != null)
            {
                OnReachEndOfLevel();
            }
        }
    }

    void OnDestroy()
    {
        GuardController.OnGuardHasSpottedPlayer -= Disable;
    }
}
