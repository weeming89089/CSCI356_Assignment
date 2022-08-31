using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour {

	public static event System.Action OnGuardHasSpottedPlayer;

	[SerializeField] float moveSpeed = 0.01f;
	[SerializeField] float waitTime = 0.3f;
	[SerializeField] float turnSpeed = 90f;

	private Animator charAnim;
	private AudioSource dieAudio;
	private Transform guardTransf;
	private Transform playerTransf;
	public Transform pathHolder;
	public Light spotLight;

	private Color originalSpotLightColor;
	public LayerMask viewMask;

	private float viewAngle;
	private float playerVisibleTimer;
	private bool canKill = false;
	private bool dead = false;
	public float viewDistance;
	public float timeToSpotPlayer = 0.5f;


	void Start () {
		playerTransf = GameObject.FindWithTag("Player").transform;
		viewAngle = spotLight.spotAngle;
		originalSpotLightColor = spotLight.color;
		dieAudio = GetComponent<AudioSource>();

		GameObject guard = GameObject.FindWithTag("Guard");
		charAnim = GetComponent<Animator>();
		guardTransf = guard.transform;

		Vector3[] waypoints = new Vector3[pathHolder.childCount];
		for (int i = 0; i < waypoints.Length; i++)
		{
			waypoints[i] = pathHolder.GetChild(i).position;
		}

		if (waypoints.Length > 1 && !dead)
        {
			StartCoroutine(followPath(waypoints));
		}

	}
	

	void Update () 
	{
		if (guardTransf == null)
		{
			charAnim.SetTrigger("idle");
			return;
		}
		else
		{
			if (canSeePlayer())
			{
				playerVisibleTimer += Time.deltaTime;
				spotLight.color = Color.red;
			}
			else
			{
				playerVisibleTimer -= Time.deltaTime;
			}

			playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
			spotLight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

			if (playerVisibleTimer >= timeToSpotPlayer)
			{
				if (OnGuardHasSpottedPlayer != null)
				{
					OnGuardHasSpottedPlayer();
				}
			}

			if (canKill && Input.GetMouseButton(0) && (dead==false))
			{
				charAnim.SetTrigger("die");
				dieAudio.Play();
				spotLight.enabled = false;
				viewDistance = 0;
				dead = true;
			}
		}
	}

	bool canSeePlayer()
    {
		if (Vector3.Distance(transform.position, playerTransf.position) < viewDistance)
        {
			Vector3 dirToPlayer = (playerTransf.position - transform.position).normalized;
			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
				if (!Physics.Linecast(guardTransf.position, playerTransf.position, viewMask))
                {
					Debug.Log("You Have Been Spotted!");
					return true;
                }
            }
        }
		return false;
    
    }

	IEnumerator followPath(Vector3[] waypoints)
    {
		transform.position = waypoints[0];

		int targetWaypointIndex = 1;
		Vector3 targetWaypoint = waypoints[targetWaypointIndex];
		transform.LookAt(targetWaypoint);

		while(!dead)
        {
			charAnim.SetTrigger("run");
			transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, moveSpeed * Time.deltaTime);
			if (transform.position == targetWaypoint)
			{
				targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
				targetWaypoint = waypoints[targetWaypointIndex];
				yield return new WaitForSeconds(waitTime);
				yield return StartCoroutine(turnToFace(targetWaypoint));
			}

			yield return null;
        }
    }

	IEnumerator turnToFace(Vector3 lookTarget)
    {
		Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;

		while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f && !dead)
        {
			float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			charAnim.SetTrigger("turn");
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
        }
    }

    void OnDrawGizmos()
    {
		Vector3 startPos = pathHolder.GetChild(0).position;
		Vector3 previousPos = startPos;

		foreach(Transform waypoint in pathHolder)
        {
			Gizmos.DrawSphere(waypoint.position, 0.3f);
			Gizmos.DrawLine(previousPos, waypoint.position);
			previousPos = waypoint.position;
        }

		Gizmos.DrawLine(previousPos, startPos);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    private void OnTriggerStay(Collider hitCollider)
    {
        if (hitCollider.tag == "Player")
        {
			Debug.Log("Left Click to Kill");
			canKill = true;
			gameObject.tag = "Dead Guard";
            
        }
    }
}
