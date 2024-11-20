using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
public class MutantSkill : MonoBehaviour
{
    public ParticleSystem ultEffect;
    public Image ultCanvasRange;
    public Image ultCanvasCircle;
    public Canvas ultCanvas;
    public float ultCooldown = 65f;
    public float ultCooldownBuffer;
    public int ultDamage = 50;
    bool isUltCooldown = false;
    public int manaCost = 105;
    public float maxUltDistance;
    public float ultCircleDistance;
    private Vector3 posUp;
    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        ultCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UltSkill();

        
    }

    void UltSkill()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                posUp = new Vector3(hit.point.x, 10f, hit.point.z + 1f);
                position = hit.point;
            }
        }

        var hitPosDir = (hit.point - transform.position).normalized;
        float distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, maxUltDistance);

        var newHitPos = transform.position + hitPosDir * distance;
        ultCanvasCircle.transform.position = (newHitPos);

        if (ultCooldownBuffer > 0f)
        {
            ultCooldownBuffer -= Time.deltaTime;
            ultCanvas.enabled = false;
            isUltCooldown = true;
        }
        else
        {
            isUltCooldown = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && !isUltCooldown)
        {
            ultCanvas.enabled = !ultCanvas.enabled;
        }

        if(ultCanvas.enabled && Input.GetMouseButtonDown(0))
        {
            ParticleSystem ultFX = Instantiate(ultEffect, newHitPos, Quaternion.identity);
            Destroy(ultFX.gameObject, 1f);
            Collider[] hitColliders = Physics.OverlapSphere(newHitPos, ultCircleDistance);

            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Interactable"))
                {
                    Debug.Log("detectedsomething");
                    Interactable interactable = collider.GetComponent<Interactable>();
                    if (interactable != null && interactable.interactionType == InteractableType.Enemy)
                    {
                        Actor actor = collider.GetComponent<Actor>();
                        if (actor != null)
                        {
                            actor.TakeDamage(ultDamage);
                        }
                    }
                }
            }

            ultCooldownBuffer = ultCooldown;
            ultCanvas.enabled = false;
        }
    }

}
