using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType { Enemy, HPPotion, ManaPotion }

public class Interactable : MonoBehaviour
{
    public Actor myActor;
    public RestorationItem restorationItem;

    public InteractableType interactionType;
    // Start is called before the first frame update
    void Awake()
    {
        if(interactionType == InteractableType.Enemy)
        {
            myActor = GetComponent<Actor>();
        }
        if(interactionType == InteractableType.HPPotion)
        {
            restorationItem = GetComponent<RestorationItem>();
        }
        if (interactionType == InteractableType.ManaPotion)
        {
            restorationItem = GetComponent<RestorationItem>();
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void InteractAndDestroy()
    {
        Destroy(gameObject);
    }
}
