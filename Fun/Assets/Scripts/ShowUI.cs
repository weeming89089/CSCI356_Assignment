using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject uiObject;
    [SerializeField] private float Waitsecond = 0.5f;
    void Start()
    {

        uiObject.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider player)
    {
        if(player.gameObject.tag == "Player")
        {
            uiObject.SetActive(true);
            StartCoroutine("WaitForSec");
        }
    }
    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(Waitsecond);
        Destroy(uiObject);
        Destroy(gameObject);
    }
}
