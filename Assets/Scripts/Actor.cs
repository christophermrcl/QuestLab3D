using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public int maxMana;
    public int currentMana;
    public int manaRegenPerSec;
    public float secondBuffer;
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        passiveManaRestoration();
    }

    void passiveManaRestoration()
    {
        if(currentMana < maxMana)
        {
            if(secondBuffer > 0f)
            {
                secondBuffer -= Time.deltaTime;
            }
            else
            {
                secondBuffer = 1f;
                if(currentMana + manaRegenPerSec > maxMana)
                {
                    currentMana = maxMana;
                }
                else
                {
                    currentMana += manaRegenPerSec;
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    public void HealHP(int amount)
    {
        if (currentHealth + amount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
    }

    public void RestoreMana(int amount)
    {
        if (currentMana + amount > maxMana)
        {
            currentMana = maxMana;
        }
        else
        {
            currentMana += amount;
        }
    }

}
