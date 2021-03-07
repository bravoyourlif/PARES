// created by chaeyeon han
// 2019.04.26 
// Purpose : reading depth value from kinect 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class MeasureDepth : MonoBehaviour
{
    public MultiSourceManager mMultiSource;

    private CameraSpacePoint[] mCameraSpacePoints = null;
    private ColorSpacePoint[] mColorSpacePoints = null;

    private readonly Vector2Int mDepthResolution = new Vector2Int(512, 424);

    private KinectSensor mSensor = null; // connect to kinect sensor
    private CoordinateMapper mMapper = null; 

    private void Awake()
    {
        mSensor = KinectSensor.GetDefault();
        mMapper = mSensor.CoordinateMapper;

        int arraySize = mDepthResolution.x * mDepthResolution.y;

        mCameraSpacePoints = new CameraSpacePoint[arraySize];
        mColorSpacePoints = new ColorSpacePoint[arraySize];

    }
}
