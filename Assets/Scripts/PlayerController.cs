using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "MutantIdle";
    const string WALK = "MutantWalk";
    const string ATTACK = "MutantBasicAttack";
    const string DEAD = "MutantDead";

    Control input;

    NavMeshAgent agent;
    Animator animator;

    public ParticleSystem clickEffect;
    public LayerMask clickableLayers;

    float lookRotationSpeed = 8f;

    public float attackSpeed = 1.5f;
    public float attackDelay = 0.3f;
    public float interactionDistance = 1.5f;
    public int maxAttackDamage = 1;
    public int minAttackDamage = 1;

    bool playerBusy = false;
    bool isDead = false;

    Interactable target;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new Control();
        AssignInputs();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    

    void AssignInputs()
    {
        
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (isDead) return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            if (hit.transform.CompareTag("Interactable"))
            {
                target = hit.transform.GetComponent<Interactable>();
                if (clickEffect != null)
                {
                    ParticleSystem clickPart = Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                    Destroy(clickPart.gameObject, 0.5f);
                }
            }
            else
            {
                target = null;

                agent.destination = hit.point;
                if (clickEffect != null)
                {
                    ParticleSystem clickPart = Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                    Destroy(clickPart.gameObject, 0.5f);
                }
            }

            
        }
    }

    private void OnEnable()
    {
        input.Enable();
        
    }
    private void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        CheckHPToDie();
        if (isDead) return;
        FollowTarget();
        FaceTarget();
        SetAnimation();
        
    }

    void CheckHPToDie()
    {
        if(GetComponent<Actor>().currentHealth <= 0)
        {
            isDead = true;
            playerBusy = true;
            animator.Play(DEAD);
        }
    }

    void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        if(Vector3.Distance(target.transform.position, transform.position) <= interactionDistance)
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
        agent.SetDestination(transform.position);

        if (playerBusy)
        {
            return;
        }

        playerBusy = true;

        switch (target.interactionType)
        {
            case InteractableType.Enemy:
                animator.Play(ATTACK);
                target.GetComponent<EnemyBehavior>().Aggroed(this.gameObject);
                Invoke(nameof(SendAttack), attackDelay);
                Invoke(nameof(ResetBusyState), attackSpeed);
                break;
            case InteractableType.HPPotion:
                GetComponent<Actor>().HealHP(target.restorationItem.restorationAmount);
                target.InteractAndDestroy();
                target = null;

                Invoke(nameof(ResetBusyState), 0.5f);
                break;
            case InteractableType.ManaPotion:
                GetComponent<Actor>().RestoreMana(target.restorationItem.restorationAmount);
                target.InteractAndDestroy();
                target = null;

                Invoke(nameof(ResetBusyState), 0.5f);
                break;
        }
    }

    void SendAttack()
    {
        if(target == null)
        {
            return;
        }

        if(target.myActor.currentHealth <= 0 || target.GetComponent<EnemyBehavior>().isDead)
        {
            target = null;
            return;
        }

        Instantiate(clickEffect, target.transform.position + new Vector3(0,1,0), Quaternion.identity);
        target.GetComponent<Actor>().TakeDamage((int)Mathf.Round(Random.Range(minAttackDamage, maxAttackDamage)));
    }

    void ResetBusyState()
    {
        playerBusy = false;
        SetAnimation();
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

    void SetAnimation()
    {
        if (playerBusy)
        {
            return;
        }

        if(agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }

}
