using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPathCreator))]
public class BezierPathEditor : Editor
{
    BezierPathCreator creator;
    BezierPath path;

    const float segmentSelectDistanceThreshold = 0.1f;
    int selectedSegmentIndex = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Create new");
            creator.CreatePath();
            path = creator.path;
        }

        bool isClosed = GUILayout.Toggle(path.Isclosed, "Closed");
        if (isClosed !=path.Isclosed)
        {
            Undo.RecordObject(creator, "Toggle Closed");
            path.Isclosed = isClosed;
        }

        bool autoSetControlPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle auto set controls");
            path.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Ray mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition)/*.origin*/;
        //Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(mousePos, out raycastHit, 100f, 1 << LayerMask.NameToLayer("Track")))
            {
                if (selectedSegmentIndex != -1)
                {
                    Undo.RecordObject(creator, "Split segment");
                    path.SplitSegment(raycastHit.point,selectedSegmentIndex);
                    //path.SplitSegment(mousePos, selectedSegmentIndex);

                }
                else if(!path.Isclosed)
                { 
                    Undo.RecordObject(creator, "Add segment");
                    path.AddSegment(raycastHit.point);
                    //path.AddSegment(mousePos);
                }
            }
        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            float minDstToAnchor = 0.05f;
            int closetAnchorIndex = -1;
            //RaycastHit raycastHit;
            //Physics.Raycast(mousePos, out raycastHit, 100f, 1 << LayerMask.NameToLayer("Point"));
            //for (int i = 0; i < path.NumPoints; i += 3)
            //{
            //    //float dst = Vector3.Distance(raycastHit.point, path[i]);
            //    //float dst = Vector3.Distance(mousePos, path[i]);
            //    if (dst < minDstToAnchor)
            //    {
            //        minDstToAnchor = dst;
            //        closetAnchorIndex = i;
            //    }
            //}

            if (closetAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete segment");
                path.DeleteSegment(closetAnchorIndex);
            }
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            float minDstToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;
            //for (int i = 0; i < path.NumSegments; i++)
            //{
            //    Vector3[] points = path.GetPointsInSegment(i);
            //    float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
            //    if (dst < minDstToSegment)
            //    {
            //        minDstToSegment = dst;
            //        newSelectedSegmentIndex = i;
            //    }
            //}

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }
    }

    void Draw()
    {
        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);
            Handles.color = Color.blue;
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? Color.red : Color.green;
            Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
        }

        Handles.color = Color.red;
        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.1f,
                Vector3.zero, Handles.CylinderHandleCap);
            if (path[i] != newPos)
            {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, newPos);
            }
        }
    }

    void OnEnable()
    {
        creator = (BezierPathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath();
        }
        path = creator.path;
    }
}
