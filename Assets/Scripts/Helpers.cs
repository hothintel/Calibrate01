using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
public sealed class Helpers
{
    public static GameObject CreateAxis(Material mat, float lineWidth, float originLen, float gradLen)
    {
        GameObject axis = new GameObject();

        // Create X axis
        List<Vector3> xVectors = new List<Vector3>();
        xVectors.Add(new Vector3(originLen * -1f, 0, 0));
        xVectors.Add(new Vector3(originLen, 0, 0));
        GameObject originX = new GameObject();
        LineRenderer xLine = originX.AddComponent<LineRenderer>();
        xLine.startWidth = lineWidth;
        xLine.endWidth = lineWidth;
        xLine.material = mat;
        xLine.SetPositions(xVectors.ToArray());
        xLine.useWorldSpace = false;
        xLine.shadowCastingMode = ShadowCastingMode.Off;
        originX.transform.parent = axis.transform;

        // Create Y axis
        List<Vector3> yVectors = new List<Vector3>();
        yVectors.Add(new Vector3(0, originLen * -1f, 0));
        yVectors.Add(new Vector3(0, originLen, 0));
        GameObject originY = new GameObject();
        LineRenderer yLine = originY.AddComponent<LineRenderer>();
        yLine.startWidth = lineWidth;
        yLine.endWidth = lineWidth;
        yLine.material = mat;
        yLine.SetPositions(yVectors.ToArray());
        yLine.useWorldSpace = false;
        yLine.shadowCastingMode = ShadowCastingMode.Off;
        originY.transform.parent = axis.transform;

        // Create Z axis
        List<Vector3> zVectors = new List<Vector3>();
        zVectors.Add(new Vector3(0, 0, originLen * -1f));
        zVectors.Add(new Vector3(0, 0, originLen));
        GameObject originZ = new GameObject();
        LineRenderer zLine = originZ.AddComponent<LineRenderer>();
        zLine.startWidth = lineWidth;
        zLine.endWidth = lineWidth;
        zLine.material = mat;
        zLine.SetPositions(zVectors.ToArray());
        zLine.useWorldSpace = false;
        zLine.shadowCastingMode = ShadowCastingMode.Off;
        originZ.transform.parent = axis.transform;

        float start = -0.1f;
        for (int i = 0; i <= 20; i++)
        {
            // create X grads
            List<Vector3> xGrads = new List<Vector3>();
            xGrads.Add(new Vector3(start, gradLen * -1f, 0));
            xGrads.Add(new Vector3(start, gradLen, 0));
            GameObject xPt = new GameObject();
            LineRenderer xGrad = xPt.AddComponent<LineRenderer>();
            xGrad.startWidth = lineWidth;
            xGrad.endWidth = lineWidth;
            xGrad.material = mat;
            xGrad.SetPositions(xGrads.ToArray());
            xGrad.useWorldSpace = false;
            xGrad.shadowCastingMode = ShadowCastingMode.Off;
            xPt.transform.parent = axis.transform;

            // create X grads
            List<Vector3> yGrads = new List<Vector3>();
            yGrads.Add(new Vector3(gradLen * -1f, start, 0));
            yGrads.Add(new Vector3(gradLen, start, 0));
            GameObject yPt = new GameObject();
            LineRenderer yGrad = yPt.AddComponent<LineRenderer>();
            yGrad.startWidth = lineWidth;
            yGrad.endWidth = lineWidth;
            yGrad.material = mat;
            yGrad.SetPositions(yGrads.ToArray());
            yGrad.useWorldSpace = false;
            yGrad.shadowCastingMode = ShadowCastingMode.Off;
            yPt.transform.parent = axis.transform;

            // create X grads
            List<Vector3> zGrads = new List<Vector3>();
            zGrads.Add(new Vector3(0, gradLen * -1f, start));
            zGrads.Add(new Vector3(0, gradLen, start));
            GameObject zPt = new GameObject();
            LineRenderer zGrad = zPt.AddComponent<LineRenderer>();
            zGrad.startWidth = lineWidth;
            zGrad.endWidth = lineWidth;
            zGrad.material = mat;
            zGrad.SetPositions(zGrads.ToArray());
            zGrad.useWorldSpace = false;
            zGrad.shadowCastingMode = ShadowCastingMode.Off;
            zPt.transform.parent = axis.transform;

            start += 0.01f;
        }

        return axis;
    }

    public static GameObject DrawLine(Material mat, float lineWidth, Vector3 startPt, Vector3 endPt)
    {
        GameObject axis = new GameObject();

        // Create X axis
        List<Vector3> xVectors = new List<Vector3>();
        xVectors.Add(startPt);
        xVectors.Add(endPt);
        GameObject originX = new GameObject();
        LineRenderer xLine = originX.AddComponent<LineRenderer>();
        xLine.startWidth = lineWidth;
        xLine.endWidth = lineWidth;
        xLine.material = mat;
        xLine.SetPositions(xVectors.ToArray());
        xLine.useWorldSpace = false;
        xLine.shadowCastingMode = ShadowCastingMode.Off;
        originX.transform.parent = axis.transform;

        return axis;
    }

    public static Vector3 CenterOfVectors(List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Count == 0)
            return sum;

        foreach (Vector3 vec in vectors)
            sum += vec;

        return sum / vectors.Count;
    }

} // end of class
