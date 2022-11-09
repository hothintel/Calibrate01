//CalibrationManager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using TMPro;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Microsoft.MixedReality.Toolkit.Input;
using System.Text;

#if ENABLE_WINMD_SUPPORT
using Windows.Perception.Spatial;
using HL2UnityPlugin;
#endif

public class CalibrationManager : MonoBehaviour, IMixedRealitySpeechHandler
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif

    bool enablePointCloud = true;

    public TextMeshPro MyInfo;

    private bool isUpdating = false;

    private GameObject _origin;
    private float _baseLineWidth = 0.001f;
    private float _baseOriginLen = 0.1f;
    private float _baseGradLen = 0.0025f;
    public Color pointColor = Color.white;
    private PointCloudRenderer _pointCloudRenderer;
    public GameObject PointCloudRendererGo;

    private ConcurrentQueue<string> ErrorQueue = new ConcurrentQueue<string>();
    private ConcurrentQueue<List<Vector3>> SensorQueue = new ConcurrentQueue<List<Vector3>>();
    private ConcurrentQueue<Vector3> SensorCoordQueue = new ConcurrentQueue<Vector3>();
    private List<Vector3> CurrentSensorList = new List<Vector3>();
    private Vector3 CurrentClosePoint = Vector3.zero;

    private float[] currentBuffer = new float[] { };
    private Vector3 _curCamPosition = Vector3.zero;
    private Vector3 _curForward = Vector3.zero;
    private float _calibrate = 0f;
    private bool _renderPointCloud = true;
    private bool _isLocked = false;

#if ENABLE_WINMD_SUPPORT
    Windows.Perception.Spatial.SpatialCoordinateSystem unityWorldOrigin;
#endif

    void Start()
    {
        MyInfo.text = "Starting...\r\n";
        CoreServices.DiagnosticsSystem.ShowProfiler = false;
        CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);

        if (PointCloudRendererGo != null)
        {
            _pointCloudRenderer = PointCloudRendererGo.GetComponent<PointCloudRenderer>();
        }

        //_curForward = CameraCache.Main.transform.forward;
        // create origin axis
        Material whiteMat = new Material(Shader.Find("Standard"));
        whiteMat.SetColor("_Color", Color.white);
        _origin = Helpers.CreateAxis(whiteMat, _baseLineWidth, _baseOriginLen, _baseGradLen);
        _origin.transform.position = new Vector3(0, 0, 0);
        _origin.transform.rotation = Quaternion.identity;

        InitResearchMode();
    }

    private void InitResearchMode()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();
        researchMode.InitializeLongDepthSensor();
        researchMode.InitializeSpatialCamerasFront();

        try
        {
            IntPtr WorldOriginPtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
            unityWorldOrigin = Marshal.GetObjectForIUnknown(WorldOriginPtr) as Windows.Perception.Spatial.SpatialCoordinateSystem;

            researchMode.SetReferenceCoordinateSystem(unityWorldOrigin);
        }
        catch(Exception myEx)
        {
            MyInfo.text += $"myEx={myEx.Message}\r\n";
            unityWorldOrigin=null;
        }

        researchMode.SetPointCloudDepthOffset(0);
        researchMode.StartLongDepthSensorLoop(enablePointCloud);
        researchMode.StartSpatialCamerasFrontLoop();
#endif    

    }

    private int maxFromCenter = 20;

    void Update()
    {
        if (!isUpdating && !_isLocked)
        {
#if ENABLE_WINMD_SUPPORT
            if (!researchMode.LongThrowPointCloudUpdated()) return;
#endif
            isUpdating = true;
            if (SensorQueue.Count > 0)
            {
                List<Vector3> dequeuedPoints = new List<Vector3>();
                if (SensorQueue.TryDequeue(out dequeuedPoints))
                {
                    CurrentSensorList = dequeuedPoints;
                }

                Vector3 closePt = Vector3.zero;
                if (SensorCoordQueue.TryDequeue(out closePt))
                    CurrentClosePoint = closePt;
            }

            currentBuffer = new float[] { };
#if ENABLE_WINMD_SUPPORT
            currentBuffer = researchMode.GetLongThrowPointCloudBuffer();
#endif
            _curCamPosition = CameraCache.Main.transform.position;
            _curForward = CameraCache.Main.transform.forward;

            Thread worker = new Thread(new ThreadStart(UpdateSensorPoints));
            worker.Start();
        }

        if (_renderPointCloud)
            _pointCloudRenderer.Render(CurrentSensorList.ToArray(), pointColor, CurrentClosePoint);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) StopSensorsEvent();
    }

    public void StopSensorsEvent()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode.StopAllSensorDevice();
#endif
    }

    private void UpdateSensorPoints()
    {
        isUpdating = true;

        float closest = float.MaxValue;

        List<Vector3> tempPoints = new List<Vector3>();
        List<Vector3> sensorPoints = new List<Vector3>();
        Vector3 closestPoint = Vector3.zero;
        Vector3 deltaError = _curForward * _calibrate;
        try
        {
            if (currentBuffer.Length == 0)
                return;

            int pointCloudLength = currentBuffer.Length / 3;
            for (int i = 0; i < pointCloudLength; i++)
            {
                var x = currentBuffer[3 * i];
                var y = currentBuffer[3 * i + 1];
                var z = currentBuffer[3 * i + 2];
                var curPt = new Vector3(x, y, z) + deltaError;

                var dist = Vector3.Distance(_curCamPosition, curPt);
                if (dist < 0.75f)
                {
                    tempPoints.Add(curPt);
                    if (dist < closest)
                    {
                        closest = dist;
                        closestPoint = curPt;
                    }
                }
            }

            for (int j = 0; j < tempPoints.Count; j++)
            {
                var dist = Vector3.Distance(tempPoints[j], closestPoint);
                if (dist < 0.5f)
                    sensorPoints.Add(tempPoints[j]);
            }
        }
        catch (Exception exception)
        {
            string exMsg = "";
            if (exception.Message.Contains("Out of Range"))
                exMsg = "Out of Range";
            else
                exMsg = exception.Message;
            string errMsg = $"Error: ex={exMsg}";
            ErrorQueue.Enqueue(errMsg);
        }
        finally
        {
            if (sensorPoints.Count > 0)
            {
                SensorQueue.Enqueue(sensorPoints);
                SensorCoordQueue.Enqueue(closestPoint);
            }

            isUpdating = false;
        }

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

    public static Vector3 GetSmoothedPoint(Vector3 newPoint, ref List<Vector3> myPoints, int maxQueueLength)
    {
        myPoints.Add(newPoint);
        if (myPoints.Count >= maxQueueLength)
        {
            myPoints.RemoveAt(0);
        }

        return CenterOfVectors(myPoints);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        //MyInfo.text += $"word={eventData.Command.Keyword.ToLower()}\r\n";

        switch (eventData.Command.Keyword.ToLower())
        {
            case "lock scan":
                _isLocked = true;
                break;

            case "start scan":
                _isLocked = false;
                break;

            case "forward":
                _calibrate -= 0.001f;
                MyInfo.text += $"_calibrate={_calibrate}\r\n";
                break;

            case "backward":
                _calibrate += 0.001f;
                MyInfo.text += $"_calibrate={_calibrate}\r\n";
                break;

            default:
                break;
        }

    }
} // end of class
