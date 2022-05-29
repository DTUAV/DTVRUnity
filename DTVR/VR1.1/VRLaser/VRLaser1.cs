/*
© Guangdong University of Technology,
© The Laboratory of Intelligent Decision and Cooperative Control,
© 2021-2022,
© Author: Yuanlin Yang (yongwang0808@163.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using DTUAV.Visualization_Module.Light;
using lcm_iot_msgs;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DTUAV.Message;
using DTUAV.Time;
using UnityEngine;

namespace DTUAV.VR
{
    public class VRLaser1 : MonoBehaviour
    {
        [Header("The Environment Layer Mask")]
        public LayerMask EnvironmentLayerMask;
        [Header("The move_uav Scripts")]
        public List<move_uav> Uavs;

        [Header("The Name of UAV")] 
        public List<string> UavNames;

        [Header("The ID of UAV")] 
        public List<int> UavIDs;

        [Header("The Frequency of Running")] 
        public float Hz;
        [Header("The Indicated Label of Control State")]
        public List<FlashingObjectNode> FlashingObjectNodes;

        [Header("The Velocity Control Panel Object")]
        public GameObject VelConPanelObj;

        [Header("The Selected Control Object Panel")]
        public GameObject SelectConPanelObj;

        [Header("The Transfer of VR Right Hand")]
        public Transform VrRightHand;

        private bool _isShowVelConPanel;

        [Header("The Distance of Control Panel and VR")]
        public float DstPanelVr;

        [Header("LCM Network: Message Topic To Cloud Network")]
        public string LcmNetworkPubTopic;
        private LCM.LCM.LCM _lcmMessagePub;
        private LcmIotMessage _lcmIotMessage;

        private int _sleepTime;
        private Thread _thread;
        private bool _isRun;

        private int _currentIndex;
        private int _maxUavNum;
        private bool _isControlAll;
        private CommandLaser _velConPanelObjLaser;
        private SelectObjectUI _selectObjectUi;
        private SelectObjectInfo _selectObjInfo;
        private bool _isShowSelectObjUi;
        private void CheckKey()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)) //Show Velocity Control Label
            {
                _isShowVelConPanel = _isShowVelConPanel ? false : true;
                VelConPanelObj.SetActive(_isShowVelConPanel);
                VelConPanelObj.transform.position = VrRightHand.position+new Vector3(0,0,DstPanelVr);
                _velConPanelObjLaser.ReInit();
            }



            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) //Control All object
            {
                _isShowSelectObjUi = _isShowSelectObjUi ? false : true;
                if (!_isShowSelectObjUi)
                {
                    _selectObjInfo = _selectObjectUi.GetSelectObjectInfo();
                }
                else
                {
                    SelectConPanelObj.transform.position = VrRightHand.position + new Vector3(0, 0, DstPanelVr);
                    _selectObjectUi.SetSelectObjectInfo(_selectObjInfo);
                }
                SelectConPanelObj.SetActive(_isShowSelectObjUi);
                /*
                _isControlAll = _isControlAll ? false : true;
                if (_isControlAll)
                {
                    for (int i = 0; i < _maxUavNum; i++)
                    {
                        Uavs[i].isVROk = true;
                        FlashingObjectNodes[i].SetColor(Color.green);
                       
                    }
                }
                else
                {
                    for (int i = 0; i < _maxUavNum; i++)
                    {
                        Uavs[i].isVROk = false;
                        FlashingObjectNodes[i].SetColor(Color.black);
                    }
                }
                */

            }

            /*

            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))//Change the control object
            {
                _currentIndex = _currentIndex == _maxUavNum - 1 ? 0 : ++_currentIndex;
               // Debug.Log(_currentIndex);
                for (int j = 0; j < _maxUavNum; ++j)
                {
                    if (j == _currentIndex)
                    {
                        Uavs[_currentIndex].isVROk = true;
                        FlashingObjectNodes[_currentIndex].SetColor(Color.green);
                        Debug.Log("dddddd");
                    }
                    else
                    {
                        Uavs[j].isVROk = false;
                        FlashingObjectNodes[j].SetColor(Color.black);

                    }
                }
            }
            */

            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))//Send Message to Cloud Network
            {
                Vector3 targetVelocity = _velConPanelObjLaser.GetTargetVelocity();
                UavControl uavControlMsg = new UavControl();
                uavControlMsg.Mode = 1;//velocity control
                uavControlMsg.ComLX = targetVelocity.x;
                uavControlMsg.ComLY = targetVelocity.y;
                uavControlMsg.ComLZ = targetVelocity.z;
                uavControlMsg.ComAX = 0;
                uavControlMsg.ComAY = 0;
                uavControlMsg.ComAZ = 0;

                LcmIotMessage msg = new LcmIotMessage();
                msg.MessageData = JsonUtility.ToJson(uavControlMsg);
                msg.MessageID = MessageId.UavCommandID;
                msg.SourceID = 201;//vr id
                msg.TimeStamp = SimTime.GetSystemTimeStampNs();
                if (_selectObjInfo.IsUav0)
                {
                    msg.TargetID = 0;
                    _lcmMessagePub.Publish(LcmNetworkPubTopic, msg);
                }

                if (_selectObjInfo.IsUav1)
                {
                    msg.TargetID = 1;
                    _lcmMessagePub.Publish(LcmNetworkPubTopic, msg);
                }

                if (_selectObjInfo.IsUav2)
                {
                    msg.TargetID = 2;
                    _lcmMessagePub.Publish(LcmNetworkPubTopic, msg);
                }
            }


        }

        //private void 



        // Start is called before the first frame update
        void Start()
        {
            _sleepTime = (int)((1.0 / Hz) * 1000);
            _isRun = true;
            _isControlAll = false;
            _currentIndex = 0;
            _maxUavNum = Uavs.Count;
           
            _velConPanelObjLaser = VelConPanelObj.GetComponent<CommandLaser>();
            _selectObjectUi = SelectConPanelObj.GetComponent<SelectObjectUI>();
            _isShowVelConPanel = true;
            _isShowSelectObjUi = true;
            _selectObjInfo = new SelectObjectInfo();
            _selectObjInfo.IsUav0 = false;
            _selectObjInfo.IsUav1 = false;
            _selectObjInfo.IsUav2 = false;

            //Init LCM Network
            _lcmMessagePub = LCM.LCM.LCM.Singleton;
            _lcmIotMessage = new LcmIotMessage();

            Loom.RunAsync(
                () =>
                {
                    _thread = new Thread(Run);
                    _thread.IsBackground = true;
                    _thread.Start();
                }
            );
        }

        private void Run()
        {
            while (_isRun)
            {
                Loom.QueueOnMainThread(() =>
                {
                    CheckKey();
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
