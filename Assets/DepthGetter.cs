using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DepthGetter : MonoBehaviour
{
    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data;

    public ushort[] Data
    {
        get { return _Data; }
        private set { _Data = value; }
    }

    private ushort _MaxDist;

    public ushort MaxDist
    {
        get { return _MaxDist; }
        private set { _MaxDist = value; }
    }

    private ushort _MinDist;

    public ushort MinDist
    {
        get { return _MinDist; }
        private set { _MinDist = value; }
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
        }
    }

    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);
                MinDist = frame.DepthMinReliableDistance;
                MaxDist = frame.DepthMaxReliableDistance;
                frame.Dispose();
                frame = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
