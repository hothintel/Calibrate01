using UnityEngine;
using System.Linq;
using System;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ElemRenderer : MonoBehaviour
{
    Mesh mesh;


    private void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateMesh(Vector3[] arrVertices, int nPointsToRender, int nPointsRendered, Color pointColor, Vector3 closestPoint)
    {
        int nPoints;

        if (arrVertices == null)
        {
            nPoints = 0;
        }
        else
        {
            nPoints = Math.Min(nPointsToRender, arrVertices.Length - nPointsRendered);
        }
        nPoints = Math.Min(nPoints, 65535);

        Vector3[] points = arrVertices.Skip(nPointsRendered).Take(nPoints).ToArray();
        int[] indices = new int[nPoints];
        Color[] colors = new Color[nPoints];

        for (int i = 0; i < nPoints; i++)
        {
            indices[i] = i;
            //colors[i] = SetDepthColorClosest(points[i], closestPoint);
            //colors[i] = SetColorTotalPoints(points[i], nPoints);

            if (nPoints > 5000)
            {
                colors[i] = SetDepthColorClosestGreen(points[i], closestPoint);
            }
            else if (nPoints > 3000)
            {
                colors[i] = SetDepthColorClosestYellow(points[i], closestPoint);
            }
            else
            {
                colors[i] = SetDepthColorClosestRed(points[i], closestPoint);
            }


        }

        //"Align with Patient's Face\nWhen Face is Aligned\nSay \"Lock Face\" or \"Cancel\"";

        if (mesh != null)
            Destroy(mesh);
        mesh = new Mesh();
        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private Color SetColorTotalPoints(Vector3 point, int totalPoints)
    {
        //color = SetColor(255, 0, 0); //--- Red
        //color = SetColor(255, 165, 0); //--- Orange
        //color = SetColor(255, 255, 0); //--- Yellow
        //color = SetColor(0, 128, 0); //--- Green
        //color = SetColor(0, 0, 255); //--- Blue
        //color = SetColor(75, 0, 130); //--- Indego
        //color = SetColor(0, 0, 0, 0); //--- Black

        Color color = SetColor(0, 0, 0);

        if (totalPoints > 5000)
        {
            color = SetColor(0, 128, 0); //--- Green
        }
        else if (totalPoints > 3000)
        {
            color = SetColor(255, 255, 0); //--- Yellow
        }
        else
        {
            color = SetColor(255, 0, 0); //--- Red
        }

        return color;
    }

    private Color SetDepthColorClosest(Vector3 point, Vector3 closestPoint)
    {
        var camPos = CameraCache.Main.transform.position;

        Color color = SetColor(255, 0, 0); //--- Red

        try
        {
            var distToCam = Vector3.Distance(camPos, point);
            var distClosestPoint = Vector3.Distance(camPos, closestPoint);
            var dist = distToCam - distClosestPoint;

            if (dist > 0.15f)
            {
                color = SetColor(0, 0, 0, 0); //--- Black
            }
            else if (dist > 0.125f)
            {
                color = SetColor(75, 0, 130); //--- Indego
            }
            else if (dist > 0.1f)
            {
                color = SetColor(0, 0, 255); //--- Blue
            }
            else if (dist > 0.075f)
            {
                color = SetColor(0, 128, 0); //--- Green
            }
            else if (dist > 0.05f)
            {
                color = SetColor(255, 255, 0); //--- Yellow
            }
            else if (dist > 0.025f)
            {
                color = SetColor(255, 165, 0); //--- Orange
            }
        }
        catch (Exception)
        {

        }

        return color;
    }

    private Color SetDepthColorClosestGreen(Vector3 point, Vector3 closestPoint)
    {
        var camPos = CameraCache.Main.transform.position;

        //008F00(0, 143, 0)
        //00B800(0, 184, 0)
        //47FF47(71, 255, 71)
        //ADFFAD(173, 255, 173)

        Color color = SetColor(0, 143, 0); //--- 1 Green X

        try
        {
            var distToCam = Vector3.Distance(camPos, point);
            var distClosestPoint = Vector3.Distance(camPos, closestPoint);
            var dist = distToCam - distClosestPoint;

            if (dist > 0.15f)
            {
                color = SetColor(0, 0, 0, 0); //--- 5 Black
            }
            else if (dist > 0.1f)
            {
                color = SetColor(173, 255, 173); //--- 4 X
            }
            else if (dist > 0.05f)
            {
                color = SetColor(71, 255, 71); //--- 3 X 
            }
            else if (dist > 0.025f)
            {
                color = SetColor(0, 184, 0); //--- 2 X
            }
        }
        catch (Exception)
        {

        }

        return color;
    }

    private Color SetDepthColorClosestYellow(Vector3 point, Vector3 closestPoint)
    {
        var camPos = CameraCache.Main.transform.position;

        Color color = SetColor(184, 184, 0); //--- 1 Yellow X

        try
        {
            var distToCam = Vector3.Distance(camPos, point);
            var distClosestPoint = Vector3.Distance(camPos, closestPoint);
            var dist = distToCam - distClosestPoint;

            if (dist > 0.15f)
            {
                color = SetColor(0, 0, 0, 0); //--- 5 Black
            }
            else if (dist > 0.1f)
            {
                color = SetColor(255, 255, 214); //--- 4 X
            }
            else if (dist > 0.05f)
            {
                color = SetColor(255, 255, 112); //--- 3 X 
            }
            else if (dist > 0.025f)
            {
                color = SetColor(255, 255, 10); //--- 2 X
            }
        }
        catch (Exception)
        {

        }

        return color;
    }

    private Color SetDepthColorClosestRed(Vector3 point, Vector3 closestPoint)
    {
        var camPos = CameraCache.Main.transform.position;

        Color color = SetColor(204, 0, 0); //--- 1 Red X

        try
        {
            var distToCam = Vector3.Distance(camPos, point);
            var distClosestPoint = Vector3.Distance(camPos, closestPoint);
            var dist = distToCam - distClosestPoint;

            if (dist > 0.15f)
            {
                color = SetColor(0, 0, 0, 0); //--- 5 Black
            }
            else if (dist > 0.1f)
            {
                color = SetColor(255, 194, 194); //--- 4 X
            }
            else if (dist > 0.05f)
            {
                color = SetColor(255, 112, 112); //--- 3 X 
            }
            else if (dist > 0.025f)
            {
                color = SetColor(255, 31, 31); //--- 2 X
            }
        }
        catch (Exception)
        {

        }

        return color;
    }

    private static Color SetColor(byte red, byte green, byte blue, byte opacity = 255)
    {
        return new Color32(red, green, blue, opacity);
    }
}
