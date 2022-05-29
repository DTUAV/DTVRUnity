using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DTUAV.VR
{
    public struct SelectObjectInfo
    {
        public bool IsUav0;
        public bool IsUav1;
        public bool IsUav2;
    }

    public class SelectObjectUI : MonoBehaviour
    {
        [Header("The Running Frequency of This Node")]
        public float Hz;

        public Material Uav0LabelMat;
        public Material Uav1LabelMat;
        public Material Uav2LabelMat;

        public LayerMask EnvironmentLayerMask;
        public Transform VrHander;
        public TextMesh InfoTextMesh;
        private SelectObjectInfo _selectInfo;

        private Thread _thread;
        private int _sleepTime;
        private bool _isRun;
        private RaycastHit _hit;
        private GameObject _myLine = null;
        private LineRenderer _lr;
        // Start is called before the first frame update

        public bool SetSelectObjectInfo(SelectObjectInfo info)
        {
            _selectInfo = info;
            InfoTextMesh.text = PackShow(_selectInfo);
            return true;
        }
        public SelectObjectInfo GetSelectObjectInfo()
        {
            return _selectInfo;
        }
        void Start()
        {
            _isRun = true;

            _sleepTime = (int)((1 / Hz) * 1000);
            _selectInfo = new SelectObjectInfo();

            Loom.RunAsync(
                () =>
                {
                    _thread = new Thread(NodeRun);
                    _thread.IsBackground = true;
                    _thread.Start();
                }
            );
        }

        private string PackShow(SelectObjectInfo info)
        {
            string data = "Now Selected Objects:";
            string uav0Info = " ";
            if (_selectInfo.IsUav0)
            {
                uav0Info = " R_UAV_0";
            }
            else
            {
                uav0Info = "";
            }
            string uav1Info = " ";
            if (_selectInfo.IsUav1)
            {
                uav1Info = " R_UAV_1";
            }
            else
            {
                uav1Info = "";
            }
            string uav2Info = " ";
            if (_selectInfo.IsUav2)
            {
                uav2Info = " R_UAV_2";
            }
            else
            {
                uav2Info = "";
            }

            return data+uav0Info+uav1Info+uav2Info;
        }

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
                if (_hit.collider.gameObject.name == "Uav0Label")//forward, back, right, left
                {
                    if (_selectInfo.IsUav0)
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav0 = false;
                            Uav0LabelMat.color = Color.white;
                        }
                    }
                    else
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav0 = true;
                            Uav0LabelMat.color = Color.green;
                        }
                    }
                }
                else if (_hit.collider.gameObject.name == "Uav1Label")//up, down
                {
                    if (_selectInfo.IsUav1)
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav1 = false;
                            Uav1LabelMat.color = Color.white;
                        }
                    }
                    else
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav1 = true;
                            Uav1LabelMat.color = Color.green;
                        }
                    }
                }
                else if (_hit.collider.gameObject.name == "Uav2Label")//up, down
                {
                    if (_selectInfo.IsUav2)
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav2 = false;
                            Uav2LabelMat.color = Color.white;
                        }
                    }
                    else
                    {
                        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                        {
                            _selectInfo.IsUav2 = true;
                            Uav2LabelMat.color = Color.green;
                        }
                    }
                }
            }
            else
            {
                if (_myLine)
                {
                    _myLine.SetActive(false);
                }
                
            }

            InfoTextMesh.text = PackShow(_selectInfo);
        }
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
