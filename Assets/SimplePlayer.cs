using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    public UIEntity playerControls, volumeSlider, xButton;
    public RoundedQuadMesh panel;
    public Vector2 heightMinMax;
    
    private RayHoverUI rayHover;

    [HideInInspector] public bool expanded = false;
    // Start is called before the first frame update
    void Start()
    {
        rayHover = GetComponent<RayHoverUI>();
        Collapse();
    }

    public void Expand()
    {
        DOTween.To(()=> panel.size.y, x=> panel.size.y = x, heightMinMax.y, 0.2f);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            playerControls.ScaleY(1);
            volumeSlider.gameObject.SetActive(true);
            volumeSlider.ScaleY(1);            

            xButton.gameObject.SetActive(true);
            xButton.Show();
        });

        expanded = true;

    }

    public void Collapse()
    {
        playerControls.ScaleY(0);
        volumeSlider.ScaleY(0);            
        xButton.Hide();
        
        DOVirtual.DelayedCall(0.1f, () =>
        {
            xButton.gameObject.SetActive(false);
            volumeSlider.gameObject.SetActive(false);
            DOTween.To(()=> panel.size.y, x=> panel.size.y = x, heightMinMax.x, 0.1f);
        });
        
        expanded = false;

    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Collapse();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Expand();
        }

    }
}
