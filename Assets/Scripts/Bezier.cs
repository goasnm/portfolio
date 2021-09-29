using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bezier : MonoBehaviour
{
    public GameObject GameObject;

    [Range(0,1)]
    public float move;

    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    private void Update()
    {
        //GameObject.transform.position = BezierPoint(P1,P2,P3,P4,move);
    }

    public Vector3 BezierPoint(Vector3 P_1, Vector3 P_2, Vector3 P_3, Vector3 P_4, float value)
    {
        Vector3 point_A = Vector3.Lerp(P_1, P_2, value);
        Vector3 point_B = Vector3.Lerp(P_2, P_3, value);
        Vector3 point_C = Vector3.Lerp(P_3, P_4, value);
        Vector3 point_D = Vector3.Lerp(point_A, point_B, value);
        Vector3 point_E = Vector3.Lerp(point_B, point_C, value);
        Vector3 point_F = Vector3.Lerp(point_D, point_E, value);

        return point_F;
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(Bezier))]

public class Bezier_Editor : Editor 
{
    private void OnSceneGUI()
    {
        Bezier Generator = (Bezier)target;

        Generator.P1 = Handles.PositionHandle(Generator.P1, Quaternion.identity);
        Generator.P2 = Handles.PositionHandle(Generator.P2, Quaternion.identity);
        Generator.P3 = Handles.PositionHandle(Generator.P3, Quaternion.identity);
        Generator.P4 = Handles.PositionHandle(Generator.P4, Quaternion.identity);

        Handles.DrawLine(Generator.P1, Generator.P2);
        Handles.DrawLine(Generator.P3, Generator.P4);

        int Count = 50;
        for (float i = 0; i < Count; i++)
        {
            float Value_Before = i / Count;
            Vector3 Before = Generator.BezierPoint(Generator.P1, Generator.P2, Generator.P3, Generator.P4, Value_Before);

            float Value_After = (i + 1) / Count;
            Vector3 After = Generator.BezierPoint(Generator.P1, Generator.P2, Generator.P3, Generator.P4, Value_After);

            Handles.DrawLine(Before, After);
        }
    }
}
