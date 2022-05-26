using LCM.LCM;
using System.Collections;
using System.Collections.Generic;
using DTUAV.Message;
using DTUAV.TF;
using dtvr_msgs;
using TMPro;
using UnityEngine;

namespace DTUAV.VR
{
    public class UavInfoSubscriber : MonoBehaviour, LCMSubscriber
    {
        [Header("The Subscribe UAV Information Topic of LCM Network")]
        public string UavInfoSubTopic;

        [Header("The UAV Object")] public GameObject Uav;
        [Header("Info Object")] public GameObject InfoObj;
        private LCM.LCM.LCM _lcmMessageSub;
        private bool _isRecv;
        private dtvr_msgs.UavInfo _uavInfo;
        private Vector3 _uavPosition;
        private Quaternion _uavRotation;
        private TextMeshPro _uavInfoText;
        private string _info;

        private bool _isShow;

        //Control Show the information of UAV
        public bool ShowInfo(bool isShow)
        {
            _isShow = isShow;
            InfoObj.SetActive(isShow);
            return true;
        }

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            _uavInfo = new dtvr_msgs.UavInfo(ins);
            _uavPosition.x = (float) _uavInfo.PosX;
            _uavPosition.y = (float) _uavInfo.PosY;
            _uavPosition.z = (float) _uavInfo.PosZ;
            _uavRotation.x = (float) _uavInfo.RotX;
            _uavRotation.y = (float) _uavInfo.RotY;
            _uavRotation.z = (float) _uavInfo.RotZ;
            _uavRotation.w = (float) _uavInfo.RotW;
            _isRecv = true;
        }

        private string showVector3(double x, double y, double z)
        {
            string ret = "( " + x + "," + y + "," + z + ")";
            return ret;
        }

        private string packShowData(dtvr_msgs.UavInfo info)
        {
            string ret = "UAV";
            string poseInfo = "position: " + showVector3(info.PosX, info.PosY, info.PosZ) + "\n";
            Quaternion rotation =
                new Quaternion((float) info.RotX, (float) info.RotY, (float) info.RotZ, (float) info.RotW);
            string rotationInfo = "Rotation: " +
                                  showVector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z) +
                                  "\n";
            string linearVelocityInfo = "Velocity: " + showVector3(info.LVelX, info.LVelY, info.LVelZ) + "\n";
            string angleVelocityInfo = "AngleVelocity: " + showVector3(info.AVelX, info.AVelY, info.AVelZ) + "\n";
            /*
             * 0: the manual mode
             * 1: the stabilizing mode
             * 2: the altitude mode
             * 3: the position mode
             * 4: the offboard mode
             * 5: the return mode
             */
            string flyModeInfo = "FlightMode: " + (info.FMode == 0 ? "Manual"
                                     : info.FMode == 1 ? "Stabilizing"
                                     : info.FMode == 2 ? "Altitude"
                                     : info.FMode == 3 ? "Position"
                                     : info.FMode == 4 ? "Offboard"
                                     : info.FMode == 5 ? "Return" : "Other") + "\n";
            string voltageInfo = "Voltage: " + info.Voltage + "\n";
            string battleRemaingInfo = "Battle Remaining: " + info.Remaining + "\n";
            string isArmInfo = "Arm: " + info.IsArm + "\n";
            string isComputerInfo = "Computer: " + info.NetPx4 + "\n";

            ret = poseInfo + rotationInfo + linearVelocityInfo + angleVelocityInfo + flyModeInfo + voltageInfo +
                  battleRemaingInfo + isArmInfo + isComputerInfo;
            return ret;
        }

        // Start is called before the first frame update
        void Start()
        {
            _uavPosition = new Vector3();
            _uavRotation = new Quaternion();
            _uavInfoText = Uav.GetComponentInChildren<TextMeshPro>(); //
            _lcmMessageSub = new LCM.LCM.LCM();
            _lcmMessageSub.Subscribe(UavInfoSubTopic, this);
            _info = "No Connection" + "\n" + "Wait information ....";
            // _uavInfoText.text = _info;
            _uavInfo = new dtvr_msgs.UavInfo();
            // _info = packShowData(_uavInfo);
            //  _uavInfoText.text = _info;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isRecv)
            {
                Uav.transform.position = TF.TF.Ros2Unity(_uavPosition);
                Uav.transform.rotation = TF.TF.Ros2Unity(_uavRotation);
                _isRecv = false;
                if (_isShow)
                {
                    _info = packShowData(_uavInfo);
                    _uavInfoText.text = _info;
                }
            }
        }
    }
}
