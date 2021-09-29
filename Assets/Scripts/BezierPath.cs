using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierPath
{
    [SerializeField,HideInInspector]
    List<Vector3> points;
    [SerializeField, HideInInspector]
    bool isClosed;
    [SerializeField, HideInInspector]
    bool autoSetControlPoints;

    public BezierPath(Vector3 center)
    {
        points = new List<Vector3>
        {
            center + Vector3.left,
            center +(Vector3.left+Vector3.up)*0.5f,
            center +(Vector3.right+Vector3.down)*0.5f,
            center + Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get 
        {
            return points[i];
        }
    }

    public bool Isclosed
    {
        get
        {
            return isClosed;   
        }
        set
        {
            if (isClosed != value)
            {
                isClosed = value;

                if (isClosed)
                {
                    points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
                    points.Add(points[0] * 2 - points[1]);
                    if (autoSetControlPoints)
                    {
                        AutoSetAnchorControlPoint(0);
                        AutoSetAnchorControlPoint(points.Count - 3);
                    }
                }
                else
                {
                    points.RemoveRange(points.Count - 2, 2);
                    if (autoSetControlPoints)
                    {
                        AutoSetStartAndEndControls();
                    }
                }
            }
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return autoSetControlPoints;
        }
        set
        {
            if (autoSetControlPoints != value)
            {
                autoSetControlPoints = value;
                if (autoSetControlPoints)
                {
                    AutoSetAllcontrolPoints();
                }
            }
        }
    }

    public int NumPoints 
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get 
        {
            return points.Count / 3; /*(points.Count - 4) / 3 + 1 + ((isClosed)?1:0);*/    
        }
    }

    public void AddSegment(Vector3 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);

        if (autoSetControlPoints)
        {
            AutoSetAffectdeControlPoints(points.Count - 1);
        }
    }

    public void SplitSegment(Vector3 anchorPos, int segmentIndsx)
    {
        points.InsertRange(segmentIndsx * 3 + 2, new Vector3[] { Vector3.zero, anchorPos, Vector3.zero });
        if (autoSetControlPoints)
        {
            AutoSetAffectdeControlPoints(segmentIndsx * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoint(segmentIndsx * 3 + 3);
        }
    }

    public void DeleteSegment(int anchorIndex)
    {
        if (NumSegments > 2 || !isClosed && NumSegments > 1)
        {
            if (anchorIndex == 0)
            {
                if (isClosed)
                {
                    points[points.Count - 1] = points[2];
                }
                points.RemoveRange(0, 3);
            }
            else if (anchorIndex == points.Count - 1 && !isClosed)
            {
                points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                points.RemoveRange(anchorIndex - 1, 3);
            }
        }
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] 
        { 
            points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] 
        };
    }

    public void MovePoint(int i, Vector3 pos)
    {
        Vector3 deltaMove = pos - points[i];

        if (i % 3 == 0 || !autoSetControlPoints)
        { 
        points[i] = pos;

            if (autoSetControlPoints)
            {
                AutoSetAffectdeControlPoints(i);
            }
            else
            {
                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count || isClosed)
                    {
                        points[LoopIndex(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0 || isClosed)
                    {
                        points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                    {
                        float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector3 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                        points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }
    }


    void AutoSetAffectdeControlPoints(int updatedAnchorIndex)
    {
        for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i+=3)
        {
            if (i >= 0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoint(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAllcontrolPoints()
    {
        for (int i = 0; i < points.Count; i+=3)
        {
            AutoSetAnchorControlPoint(i);
        }
        AutoSetStartAndEndControls();
    }

    void AutoSetAnchorControlPoint(int anchorIndex)
    {
        Vector3 anchorPos = points[anchorIndex];
        Vector3 dir = Vector3.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || isClosed)
        {
            Vector3 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }

        if (anchorIndex + 3 >= 0 || isClosed)
        {
            Vector3 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * 0.5f;
            }
        }
    }

    void AutoSetStartAndEndControls()
    {
        if (!isClosed)
        {
            points[1] = (points[0] + points[2]) * 0.5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
        }
    }

    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}
