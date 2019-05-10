using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CircleLine : MonoBehaviour
{
    public int segments = 36;
    public Vector2 radius = new Vector2(1,1);
    
	public float arcWidth = 360f;

    public bool autoUpdate = false;
	
	LineRenderer line;
    
    void Start ()
    {
        line = GetComponent<LineRenderer>();
        CreatePoints ();
    }

    private void Update()
    {
        if (autoUpdate)
        {
            ClearPoints();
            CreatePoints();

        }
    }

    private void OnValidate() {
        ClearPoints();
        CreatePoints();
    }

   
    void CreatePoints ()
    {
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;

        float x;
        float y;
        float z = 0f;
       
        float angle = 20f;
       
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius.x;
            y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius.y;
                   
            line.SetPosition (i,new Vector3(x,y,z) );
                   
            angle += (arcWidth / segments);
        }
    }

    void ClearPoints(){
        if(line == null) line = GetComponent<LineRenderer>();

        line.positionCount = 0;
    }
}
