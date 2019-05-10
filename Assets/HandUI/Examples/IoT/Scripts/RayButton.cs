using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayButton : HandUIItem
{
    public UnityEvent OnPinchEvent;
    private UIEntity self;
    
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<UIEntity>();
    }

    protected override void OnHoverBegin()
    {
        self.SetColor(1);
        self.Scale(1.4f);
    }
    
    protected override void OnHoverEnd()
    {
        self.SetInitialColor();
        self.Scale(1f);
    }
    
    protected override void OnPinchBegin()
    {
        self.Punch();
        OnPinchEvent.Invoke();
    }

}
