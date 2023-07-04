using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float healthPoints = 5.0f;
    public float maxHealthPoints = 5.0f;

    public float flashTime;

    private SpriteRenderer sr;
    private Color originalColor;

    // Start is called before the first frame update
    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Update is called once per frame
    public void Update()
    {
        if (healthPoints <= Mathf.Epsilon)
        {
            CharacterDie();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        healthPoints -= damageAmount;
        FlashColor(flashTime);

    }

    virtual public void CharacterDie() 
    {
        Invoke("PlayerDestroy", 1.2f);
    }

    void FlashColor(float time)
    {
        sr.color = Color.red;
        Invoke("ResetColor", time);
    }

    void ResetColor()
    {
        sr.color = originalColor;
    }

    void PlayerDestroy()
    {
        Destroy(gameObject);
    }
}
