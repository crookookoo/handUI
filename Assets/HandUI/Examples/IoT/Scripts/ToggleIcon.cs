using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleIcon : MonoBehaviour
{
    public Sprite iconA, iconB;

    private SpriteRenderer sprite;

    private bool aSet = true;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = iconA;
    }

    public void SwitchIcon()
    {
        sprite.sprite = aSet ? iconB : iconA;
        aSet = !aSet;
    }
    
}
