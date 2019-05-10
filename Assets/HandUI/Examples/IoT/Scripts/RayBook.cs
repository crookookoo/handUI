using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RayBook : HandRaycastItem
{    
    public RayHoverUI rayHover;
    public string author, title;
    public UIEntity authorEntity, titleEntity, indicator;
    private TextMeshPro authorText, titleText;
    
    
    [Range(0, 1)] public float progress;

    void Start()
    {
        authorText = authorEntity.GetComponent<TextMeshPro>();
        titleText = titleEntity.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHoverBegin()
    {
        //show UI
        if(!rayHover.shown) rayHover.Appear(base.hrt.WhichHandHovering());
        
        authorEntity.SetColor(0);
        titleEntity.SetColor(0);

        titleText.text = title;
        authorText.text = author;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            authorEntity.SetInitialColor();
            titleEntity.SetInitialColor();

        });

        
        indicator.Show();
        Vector3 target = indicator.transform.position;
        target.x = transform.position.x;
        indicator.transform.position = target;
        
    }
    protected override void OnHoverEnd()
    {
        //hide UI
        rayHover.Disappear();
        indicator.Hide();

    }

}
