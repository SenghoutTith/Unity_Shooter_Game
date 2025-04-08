using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;
    public float agressionRage;

    [Header("Idle Data")]
    public float idleTime;

    [Header("Move Data")]
    public float moveSpeed;
    public float chaseSpeed;

    [SerializeField] private Transform[] partolPoints;
    private int currentPatrolIndex;
    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    
    protected virtual void Awake()
    {    
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Person Robot").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }
    protected virtual void Update()
    {
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = partolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;
        if (currentPatrolIndex >= partolPoints.Length)
        {
            currentPatrolIndex = 0;
        }
        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in partolPoints)
        {
            t.parent = null;
        }
        Debug.Log("Patrol points initialized");
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngels = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, agressionRage);
    }

    public bool PlayerInAgressionRange() => Vector3.Distance(transform.position, player.position) < agressionRage;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
}
