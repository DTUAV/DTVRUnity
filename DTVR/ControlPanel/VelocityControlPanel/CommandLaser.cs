using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System.Threading;

namespace DTUAV.VR
{
    public class CommandLaser:MonoBehaviour
    {
        [Header("The Running Frequency of This Node")]
        public float Hz;

        public LayerMask EnvironmentLayerMask;
        public Transform LeftSphere;

        public Transform RightSphere;

        public TextMesh InfoTextMesh;

        public Transform VrHander;
        [Header("The Maximum Velocity")] 
        public float MaxVelocity;

        private Thread _thread;
        private int _sleepTime;
        private bool _isRun;
        private RaycastHit _hit;
        private GameObject _myLine = null;
        private LineRenderer _lr;
        private Vector3 _leftCerterPoint;
        private Vector3 _rightCerterPoint;
        private double _targetRosVelX;
        private double _targetRosVelY;
        private double _targetRosVelZ;


        public Vector3 GetTargetVelocity()
        {
            return new Vector3((float)_targetRosVelX,(float)_targetRosVelY,(float)_targetRosVelZ);
        }
        private double Value2Nomal(double value,double maxValue)
        {
          //  double ret = -maxValue + (value+maxValue)/maxValue;
            double ret = value > 1 ? 1 : value < -1 ? -1 : value;
            return ret;
        }
        private string PackShow(double targetX, double targetY, double targetZ)
        {
            string data = "Now Target Velocity: " + "\n"
                                                  + "X: " + targetX + "m/s"+ "\n"
                                                  + "Y: " + targetY + "m/s" + "\n"
                                                  + "Z: " + targetZ + "m/s";
            return data;
        }

        [System.Obsolete]
        void GetLaserInfo()
        {
            if (Physics.Raycast(VrHander.position, VrHander.forward, out _hit, float.MaxValue,
                EnvironmentLayerMask))
            {
              //  Debug.DrawLine(VrHander.position, _hit.point, Color.red);
                DrawLineGame(VrHander.position, _hit.point, Color.red);
                if (_myLine)
                {
                    _myLine.SetActive(true);
                }
                if (_hit.collider.gameObject.name == "LeftWorkingSpace")//forward, back, right, left
                {
                   // InfoTextMesh.text = "Left: "+_hit.point;
                    LeftSphere.position = _hit.point;
                    RightSphere.position = _rightCerterPoint;
                    _targetRosVelY = Value2Nomal(_hit.point.x - _leftCerterPoint.x,0.6) * MaxVelocity;
                    _targetRosVelX = Value2Nomal(_hit.point.y - _leftCerterPoint.y,0.6) * MaxVelocity;

                }
                else if (_hit.collider.gameObject.name == "RightWorkingSpace")//up, down
                {
                   // InfoTextMesh.text = "Right: " + _hit.point;
                    RightSphere.position = _hit.point;
                    LeftSphere.position = _leftCerterPoint;
                    _targetRosVelZ = Value2Nomal(_hit.point.y - _rightCerterPoint.y,0.6) * MaxVelocity;
                }
            }
            else
            {
                if (_myLine)
                {
                    _myLine.SetActive(false);
                }
                LeftSphere.position = _leftCerterPoint;
                RightSphere.position = _rightCerterPoint;
                _targetRosVelY = 0;
                _targetRosVelX = 0;
                _targetRosVelZ = 0;
            }

            InfoTextMesh.text = PackShow(_targetRosVelX, _targetRosVelY, _targetRosVelZ);
        }

        [System.Obsolete]
        private void DrawLineGame(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            if (_myLine == null)
            {
                _myLine = new GameObject();
                _myLine.AddComponent<LineRenderer>();
                _lr = _myLine.GetComponent<LineRenderer>();
                _lr.SetColors(color, color);
                _lr.SetWidth(0.01f, 0.01f);
            }

            _myLine.transform.position = start;
            _lr.SetPosition(0, start);
            _lr.SetPosition(1, end);
        }

        public bool ReInit()
        {
            _leftCerterPoint = LeftSphere.position;
            _rightCerterPoint = RightSphere.position;
            return true;
        }

        [System.Obsolete]
        void Start()
        {
            _isRun = true;

            _sleepTime = (int) ((1 / Hz) * 1000);
            _leftCerterPoint = LeftSphere.position;
            _rightCerterPoint = RightSphere.position;
            _targetRosVelX = 0;
            _targetRosVelY = 0;
            _targetRosVelZ = 0;
            Loom.RunAsync(
                () =>
                {
                    _thread = new Thread(NodeRun);
                    _thread.IsBackground = true;
                    _thread.Start();
                }
            );
        }

        [System.Obsolete]
        void NodeRun()
        {
            while (_isRun)
            {
                Loom.QueueOnMainThread(() =>
                {
                    GetLaserInfo();
                });
                
                System.Threading.Thread.Sleep(_sleepTime);
            }
        }

        void OnDestroy()
        {
            _isRun = false;
        }
    }
}
