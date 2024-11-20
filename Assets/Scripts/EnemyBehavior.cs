using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    const string IDLE = "WarrokIdle";
    const string WALK = "WarrokWalk";
    const string ATTACK = "WarrokAttack";
    const string DEAD = "WarrokDead";

    public float interactionDistance = 1.0f;
    public float attackDelay = 1.0f;
    public float attackSpeed = 1.0f;
    public float aggroDistance = 10f;
    public int maxAttackDamage = 20;
    public int minAttackDamage = 20;

    float lookRotationSpeed = 8f;

    bool isBusy = false;
    public bool isDead = false;
    NavMeshAgent agent;
    Animator animator;

    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHPToDie();
        if (isDead)
        {
            return;
        }
        CheckDistancePlayer();
        FollowTarget();
        SetAnimation();
        FaceTarget();
    }

    void FaceTarget()
    {
        if (agent.velocity != Vector3.zero)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void CheckDistancePlayer()
    {
        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) <= aggroDistance)
        {
            Aggroed(GameObject.FindGameObjectWithTag("Player"));
        }
        else
        {
            Deaggroed();
        }
    }

    public void Aggroed(GameObject player)
    {
        target = player;
    }

    public void Deaggroed()
    {
        target = null;
    }

    void CheckHPToDie()
    {
        if (GetComponent<Actor>().currentHealth <= 0)
        {
            isDead = true;
            isBusy = true;
            animator.Play(DEAD);

            if(animator.GetCurrentAnimatorStateInfo(0).IsName(DEAD) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                Destroy(gameObject);
            }
        }
    }

    void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        if (Vector3.Distance(target.transform.position, transform.position) <= interactionDistance)
        {
            ReachDistance();
        }
        else
        {
            agent.SetDestination(target.transform.position);
        }
    }

    void ReachDistance()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        agent.SetDestination(transform.position);

        animator.Play(ATTACK);
        Invoke(nameof(SendAttack), attackDelay);
        Invoke(nameof(ResetBusyState), attackSpeed);
    }

    void ResetBusyState()
    {
        isBusy = false;
        SetAnimation();
    }

    void SendAttack()
    {
        if (target == null)
        {
            return;
        }

        if (target.GetComponent<Actor>().currentHealth <= 0)
        {
            target = null;
            return;
        }

        target.GetComponent<Actor>().TakeDamage((int) Mathf.Round(Random.Range(minAttackDamage, maxAttackDamage)));
    }

    void SetAnimation()
    {
        if (isBusy)
        {
            return;
        }

        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
}
