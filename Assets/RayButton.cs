using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayButton : HandUIItem
{
    private UIEntity self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<UIEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHoverBegin()
    {
        self.SetColor(1);
        self.Scale(1.2f);
    }
    
    protected override void OnHoverEnd()
    {
        self.SetInitialColor();
        self.Scale(1f);
    }
    
    protected override void OnPinchBegin()
    {
        self.Punch();
    }

}
