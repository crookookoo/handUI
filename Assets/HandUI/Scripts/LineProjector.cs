using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LineProjector : MonoBehaviour {
	public Transform lineStart, lineEnd;
	
	private bool shown = false;
	private LineRenderer line;

	private bool direct = false;
	private bool indirect = false;

	private MapControlPoint mapControlPoint;

	private Vector3 curveTop;

	[HideInInspector]
	public Vector3 bPoint1, bPoint2;

    public Vector3[] points;

	private int lineResolution = 20; // must be not divisible by 3! otherwise loop crashes unity ¯\_(ツ)_/¯

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer>();

		mapControlPoint = transform.parent.GetComponent<MapControlPoint>();
		Hide();

		points = new Vector3[4];

		SetPoints();
		// points.Append();
	}

	void SetPoints(){
		points[0] = lineStart.position;
		points[1] = bPoint1;
		points[2] = bPoint2;
		points[3] = lineEnd.position;
	}

	void RedrawLine(){

		SetPoints();
		// points[0] = lineStart.position;
		// points[1] = lineEnd.position;
		// curveTop = bezierControlPoint.position;

        // if (points == null || points.Length <= 0)
        // {
        //     line.positionCount = 0;
        //     line.SetPositions(new Vector3[] { Vector3.zero});
        //     return;
        // }

	    var pointList = new List<Vector3>();
	    for (float ratio = 0; ratio <= 1; ratio += 1.0f / lineResolution)
        {
            Vector3 bezierPoint = CalculateBezierPoint(ratio, points.Select(point => point));
            pointList.Add(bezierPoint);
        }
        
		line.positionCount = pointList.Count;
		line.SetPositions(pointList.ToArray());
	
	}

    private Vector3 CalculateBezierPoint(float ratio, IEnumerable<Vector3> points)
    {
        if (points.Count() == 1)
        {
            return points.First();
        }

        LinkedList<Vector3> subPoints = new LinkedList<Vector3>();
        Vector3? lastPoint = null;
        foreach (var point in points)
        {
            if (!lastPoint.HasValue)
            {
                lastPoint = point;
                continue;
            }
            else
            {
                subPoints.AddLast(Vector3.Lerp(lastPoint.Value, point, ratio));

                lastPoint = point;
            }
        }

        return CalculateBezierPoint(ratio, subPoints);
    }

	// Update is called once per frame
	void LateUpdate () {
		if(line.enabled){
			if(shown){
			
				RedrawLine();
			
			}		

		}

	}

	public void Show(){
		if(!line.enabled) line.enabled = true;
		shown = true;
	}

	public void Hide(){
		line.enabled = false;
		shown = false;
	}

}
