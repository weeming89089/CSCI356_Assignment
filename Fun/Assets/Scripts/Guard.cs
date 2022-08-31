using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;

    private Transform player;
    public Transform pathHolder;
    public Animator charAnim;
    public Light spotLight;
    public GameObject canKillUI;
    private AudioSource dieAudio;
    private BoxCollider boxCollider;

    private Color originalSpotlightColor;
    public LayerMask viewMask;

    private float viewAngle;
    private float playerVisibleTimer;
    private bool canKill = false;
    private bool dead = false;
    public float speed = 0.5f;
    public float waitTime = 0.3f;
    public float turnSpeed = 90.0f;
    public float viewDistance;
    public float timeToSpotPlayer = 0.5f;


    void Start()
    {
        charAnim = GetComponent<Animator>();
        viewAngle = spotLight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boxCollider = GetComponent<BoxCollider>();
        originalSpotlightColor = spotLight.color;
        dieAudio = GetComponent<AudioSource>();

        Vector3[] waypoints = new Vector3[pathHolder.childCount];

        if (waypoints.Length > 1)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathHolder.GetChild(i).position;
            }

            StartCoroutine(FollowPath(waypoints));
        }
        
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }

        if (canKill && !dead && Input.GetMouseButtonDown(0))
        {
            dieAudio.Play();
            dead = true;
            spotLight.enabled = false;
            viewDistance = 0;
            GameUI.guardsCount = GameUI.guardsCount - 1;
            canKillUI.SetActive(false);
        }

        if (dead)
        {
            charAnim.SetTrigger("die");
            boxCollider.enabled = false;
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    Debug.Log("You Have Been Spotted!");
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (!dead)
        {
            charAnim.SetTrigger("run");
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }


    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Player")
        {
            Debug.Log("Left Click To Kill");
            canKill = true;
            canKillUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider hitCollider)
    {
        if (hitCollider.tag == "Player")
        {
            Debug.Log("Left Kill Zone");
            canKill = false;
            canKillUI.SetActive(false);
        }
    }
}
