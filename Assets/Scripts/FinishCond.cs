using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCond : MonoBehaviour
{
    public List<GameObject> availableEnemy;

    public GameObject finishCircle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < availableEnemy.Count; i++)
        {
            if (availableEnemy[i] == null)
            {
                availableEnemy.Remove(availableEnemy[i]);
            }
        }

        if(availableEnemy.Count == 0)
        {
            finishCircle.SetActive(true);
        }
    }
}
