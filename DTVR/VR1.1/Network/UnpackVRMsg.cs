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
using System.Collections;
using System.Collections.Generic;
using DTUAV.Message;
using LCM.LCM;
using lcm_iot_msgs;
using sensor_msgs;
using UnityEngine;

namespace DTUAV.VR
{
    public class UnpackVRMsg : MonoBehaviour,LCMSubscriber
    {
        [Header("The Rigidbody of UAV")]
        public Rigidbody RibUAV;
        [Header("The Publish Topic Name of the UAV for Information")]
        public string LcmUavInfoPubTopic = "/PhysicalUav0/Information";
        [Header("The Publish Topic Name of the UAV for Camera")]
        public string LcmUavCamPubTopic = "/VirtualUav0/Camera";
        [Header("The Subscribe Topic of LCM Network to ConnectorLcmNode Publisher")]
        public string GlobalNetworkMessageSubName;
        [Header("Configure Object ID")]
        [Header("The Message Source ID")]
        public int SourceId;
        [Header("The Message Target ID")]
        public int TargetId;
        private LCM.LCM.LCM _uavInfoPub;
        private LCM.LCM.LCM _camPub;
        private dtvr_msgs.UavInfo _uavInfo;
        private sensor_msgs.CompressedImage _uavCamMsg;
        private Vector3 _objPosition;
        private Quaternion _objRotation;
        private LCM.LCM.LCM _lcmMessageSub;
        private LcmIotMessage _lcmIotMessage;

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            _lcmIotMessage = new LcmIotMessage(ins);

            if (_lcmIotMessage.TargetID == TargetId && _lcmIotMessage.SourceID == SourceId)
            {
                switch (_lcmIotMessage.MessageID)
                {
                    case MessageId.UavInfoID:
                    {
                        UavInfo uavInfo = JsonUtility.FromJson<UavInfo>(_lcmIotMessage.MessageData);
                        _uavInfo.PosX = uavInfo.PosX;
                        _uavInfo.PosY = uavInfo.PosY;
                        _uavInfo.PosZ = uavInfo.PosZ;
                        _uavInfo.RotX = uavInfo.RotX;
                        _uavInfo.RotY = uavInfo.RotY;
                        _uavInfo.RotZ = uavInfo.RotZ;
                        _uavInfo.RotW = uavInfo.RotW;
                        _uavInfo.LVelX = uavInfo.LVelX;
                        _uavInfo.LVelY = uavInfo.LVelY;
                        _uavInfo.LVelZ = uavInfo.LVelZ;
                        _uavInfo.AVelX = uavInfo.AVelX;
                        _uavInfo.AVelY = uavInfo.AVelY;
                        _uavInfo.AVelZ = uavInfo.AVelZ;
                        _uavInfo.FMode = (byte) uavInfo.FMode;
                        _uavInfo.IsArm = uavInfo.IsArm;
                        _uavInfo.NetPx4 = uavInfo.NetPx4;
                        _uavInfo.Voltage = uavInfo.Voltage;
                        _uavInfo.Remaining = uavInfo.Remaining;
                        _uavInfoPub.Publish(LcmUavInfoPubTopic, _uavInfo);

                        _objPosition = TF.TF.Ros2Unity(new Vector3((float)uavInfo.PosX, (float)uavInfo.PosY,
                            (float)uavInfo.PosZ));
                        _objRotation = TF.TF.Ros2Unity(new Quaternion((float)uavInfo.RotX,
                            (float)uavInfo.RotY, (float)uavInfo.RotZ,
                            (float)uavInfo.RotW));
                        }
                        break;
                    case MessageId.CompressedImageMessageID:
                    {
                        CompressedImageMessage physicalCompressedImage =
                            JsonUtility.FromJson<CompressedImageMessage>(_lcmIotMessage.MessageData);
                        _uavCamMsg.format = physicalCompressedImage.format;
                        _uavCamMsg.data_size = physicalCompressedImage.data.Length;
                        _uavCamMsg.data = new byte[_uavCamMsg.data_size];
                        for (int i = 0; i < _uavCamMsg.data_size; ++i)
                            _uavCamMsg.data[i] = physicalCompressedImage.data[i];
                        _camPub.Publish(LcmUavCamPubTopic, _uavCamMsg);
                    }
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _objPosition = new Vector3();
            _objRotation = new Quaternion();
            _uavInfo = new dtvr_msgs.UavInfo();
            _uavCamMsg = new CompressedImage();
            _camPub = LCM.LCM.LCM.Singleton;
            _uavInfoPub = LCM.LCM.LCM.Singleton;
            _lcmMessageSub = new LCM.LCM.LCM();
            _lcmMessageSub.Subscribe(GlobalNetworkMessageSubName, this);
            _lcmIotMessage = new LcmIotMessage();
        }

        // Update is called once per frame
        void Update()
        {
            RibUAV.position = _objPosition;
            RibUAV.rotation = _objRotation.normalized;
        }
    }
}
