using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;
using System.Threading;
using DTUAV.Message;
using DTUAV.TF;
using sensor_msgs;

namespace DigitalTwin.UavCarProject
{
    public class VR_Server : MonoBehaviour
    {
        public string ProductKey = "a1GcqS5WFte";
        public string DeviceName = "VR_Server";
        public string DeviceSecret = "67a5eb9e157e61261a5b0a75a8e2f7d1";
        public string RegionId = "cn-shanghai";
        private string PubTopic;
        private string SubTopic;
        public string Pub = "/user/VR_Server_PUB";
        public string Sub = "/user/VR_Server_SUB";
        private MqttClient clientPub;
        public Rigidbody SimulationUAV;
        public Rigidbody PhysicalUAV;
        [Header("Using LCM Network? If True,two following topic name should be configured!!!")]
        public bool IsUsingLCM = false;
        [Header("The Publish Topic Name of the Virtual UAV for Information")]
        public string LcmVirtualUavInfoPubTopic = "/VirtualUav0/Information";
        [Header("The Publish Topic Name of the Physical UAV for Information")]
        public string LcmPhysicalUavInfoPubTopic = "/PhysicalUav0/Information";
        [Header("The Publish Topic Name of the Virtual UAV for Camera")]
        public string LcmVirtualUavCamPubTopic = "/VirtualUav0/Camera";
        [Header("The Publish Topic Name of the Physical UAV for Camera")]
        public string LcmPhysicalUavCamPubTopic = "/PhysicalUav0/Camera";
        private LCM.LCM.LCM _virtualUavInfoPub;
        private LCM.LCM.LCM _physicalUavInfoPub;
        private LCM.LCM.LCM _virtualCamPub;
        private LCM.LCM.LCM _physicalCamPub;
        private dtvr_msgs.UavInfo _virtualUavInfo;
        private dtvr_msgs.UavInfo _physicalUavInfo;
        private sensor_msgs.CompressedImage _virtualUavCamMsg;
        private sensor_msgs.CompressedImage _physicalUavCamMsg;
        private Vector3 _simObjPosition;
        private Quaternion _simObjRotation;
        private Vector3 _phsObjPosition;
        private Quaternion _phsObjRotation;


        // Start is called before the first frame update
        void Start()
        {
            _simObjPosition = new Vector3();
            _simObjRotation = new Quaternion();
            _phsObjPosition = new Vector3();
            _phsObjRotation = new Quaternion();
            _virtualUavInfo = new dtvr_msgs.UavInfo();
            _physicalUavInfo = new dtvr_msgs.UavInfo();
            _virtualUavCamMsg = new CompressedImage();
            _physicalUavCamMsg = new CompressedImage();
            _virtualCamPub = LCM.LCM.LCM.Singleton;
            _physicalCamPub = LCM.LCM.LCM.Singleton;
            _virtualUavInfoPub = LCM.LCM.LCM.Singleton;
            _physicalUavInfoPub = LCM.LCM.LCM.Singleton;
            PubTopic = "/" + ProductKey + "/" + DeviceName + Pub;
            SubTopic = "/" + ProductKey + "/" + DeviceName + Sub;
            Run();
        }
        void Run()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string clientId = host.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            string t = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string signmethod = "hmacmd5";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("productKey", ProductKey);
            dict.Add("deviceName", DeviceName);
            dict.Add("clientId", clientId);
            dict.Add("timestamp", t);

            string mqttUserName = DeviceName + "&" + ProductKey;
            string mqttPassword = SignUtils.sign(dict, DeviceSecret, signmethod);
            string mqttClientId = clientId + "|securemode=3,signmethod=" + signmethod + ",timestamp=" + t + "|";

            string targetServer = ProductKey + ".iot-as-mqtt." + RegionId + ".aliyuncs.com";
            ConnectMqtt(targetServer, mqttClientId, mqttUserName, mqttPassword);
        }

        void ConnectMqtt(string targetServer, string mqttClientId, string mqttUserName, string mqttPassword)
        {
            MqttClient client = new MqttClient(targetServer);
            client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;
            clientPub = client;
            client.Connect(mqttClientId, mqttUserName, mqttPassword, false, 60);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            String content = "{'content':'msg from :" + mqttClientId + "hello px4'}";
            var id = client.Publish(PubTopic, Encoding.ASCII.GetBytes(content));

            client.Subscribe(new string[] { SubTopic }, new byte[] { 2 });

        }

        void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //handle message received
           // Debug.Log("ddddddddddddddddd");
            string topic = e.Topic;
            string message = Encoding.ASCII.GetString(e.Message);
           // Debug.Log("message: "+message);
            UnpackMsg(message);
        }
        string CheckRecvData(string msg)
        {
            string ret = "";
            if (msg[0] != '{')
            {
                ret = "";
            }
            else
            {
                int num = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] == '}')
                    {
                        num++;
                    }
                    if (num == 2)
                    {
                        ret = msg.Substring(0, i + 1);
                        break;
                    }
                }
            }
            return ret;
        }

        void UnpackMsg(string msg)
        {
            string data_msg = CheckRecvData(msg);
            if (data_msg != "")
            {
               IotMessage iotMsg = JsonUtility.FromJson<IotMessage>(data_msg);
               switch (iotMsg.MessageID)
               {
                   case MessageId.UavInfoID:
                   {
                       if (iotMsg.SourceID == ObjectId.R_UAV_0)
                       {
                           UavInfo physicalUavInfo = JsonUtility.FromJson<UavInfo>(iotMsg.MessageData);
                           _physicalUavInfo.PosX = physicalUavInfo.PosX;
                           _physicalUavInfo.PosY = physicalUavInfo.PosY;
                           _physicalUavInfo.PosZ = physicalUavInfo.PosZ;
                           _physicalUavInfo.RotX = physicalUavInfo.RotX;
                           _physicalUavInfo.RotY = physicalUavInfo.RotY;
                           _physicalUavInfo.RotZ = physicalUavInfo.RotZ;
                           _physicalUavInfo.RotW = physicalUavInfo.RotW;
                           _physicalUavInfo.LVelX = physicalUavInfo.LVelX;
                           _physicalUavInfo.LVelY = physicalUavInfo.LVelY;
                           _physicalUavInfo.LVelZ = physicalUavInfo.LVelZ;
                           _physicalUavInfo.AVelX = physicalUavInfo.AVelX;
                           _physicalUavInfo.AVelY = physicalUavInfo.AVelY;
                           _physicalUavInfo.AVelZ = physicalUavInfo.AVelZ;
                           _physicalUavInfo.FMode = (byte)physicalUavInfo.FMode;
                           _physicalUavInfo.IsArm = physicalUavInfo.IsArm;
                           _physicalUavInfo.NetPx4 = physicalUavInfo.NetPx4;
                           _physicalUavInfo.Voltage = physicalUavInfo.Voltage;
                           _physicalUavInfo.Remaining = physicalUavInfo.Remaining;
                           _physicalUavInfoPub.Publish(LcmPhysicalUavInfoPubTopic, _physicalUavInfo);
                       }
                       else if (iotMsg.SourceID == ObjectId.V_UAV_0)
                       {
                           UavInfo virtualUavInfo = JsonUtility.FromJson<UavInfo>(iotMsg.MessageData);
                           _virtualUavInfo.PosX = virtualUavInfo.PosX;
                           _virtualUavInfo.PosY = virtualUavInfo.PosY;
                           _virtualUavInfo.PosZ = virtualUavInfo.PosZ;
                           _virtualUavInfo.RotX = virtualUavInfo.RotX;
                           _virtualUavInfo.RotY = virtualUavInfo.RotY;
                           _virtualUavInfo.RotZ = virtualUavInfo.RotZ;
                           _virtualUavInfo.RotW = virtualUavInfo.RotW;
                           _virtualUavInfo.LVelX = virtualUavInfo.LVelX;
                           _virtualUavInfo.LVelY = virtualUavInfo.LVelY;
                           _virtualUavInfo.LVelZ = virtualUavInfo.LVelZ;
                           _virtualUavInfo.AVelX = virtualUavInfo.AVelX;
                           _virtualUavInfo.AVelY = virtualUavInfo.AVelY;
                           _virtualUavInfo.AVelZ = virtualUavInfo.AVelZ;
                           _virtualUavInfo.FMode = (byte)virtualUavInfo.FMode;
                           _virtualUavInfo.IsArm = virtualUavInfo.IsArm;
                           _virtualUavInfo.NetPx4 = virtualUavInfo.NetPx4;
                           _virtualUavInfo.Voltage = virtualUavInfo.Voltage;
                           _virtualUavInfo.Remaining = virtualUavInfo.Remaining;
                           _virtualUavInfoPub.Publish(LcmVirtualUavInfoPubTopic, _virtualUavInfo);
                       }
                   }
                       break;
                   case MessageId.CompressedImageMessageID:
                   {
                       if (iotMsg.SourceID == ObjectId.R_UAV_0)
                       { 
                           CompressedImageMessage physicalCompressedImage = JsonUtility.FromJson<CompressedImageMessage>(iotMsg.MessageData);
                           _physicalUavCamMsg.format = physicalCompressedImage.format;
                           _physicalUavCamMsg.data_size = physicalCompressedImage.data.Length;
                           _physicalUavCamMsg.data = new byte[_physicalUavCamMsg.data_size];
                           for (int i = 0; i < _physicalUavCamMsg.data_size; ++i)
                               _physicalUavCamMsg.data[i] = physicalCompressedImage.data[i];
                           _physicalCamPub.Publish(LcmPhysicalUavCamPubTopic,_physicalUavCamMsg);
                       }
                       else if (iotMsg.SourceID == ObjectId.V_UAV_0)
                       {
                           CompressedImageMessage virtualCompressedImage = JsonUtility.FromJson<CompressedImageMessage>(iotMsg.MessageData);
                           _physicalUavCamMsg.format = virtualCompressedImage.format;
                           _physicalUavCamMsg.data_size = virtualCompressedImage.data.Length;
                           _physicalUavCamMsg.data = new byte[_physicalUavCamMsg.data_size];
                           for (int i = 0; i < _physicalUavCamMsg.data_size; ++i)
                               _physicalUavCamMsg.data[i] = virtualCompressedImage.data[i];
                           _virtualCamPub.Publish(LcmVirtualUavCamPubTopic,_virtualUavCamMsg);
                       }
                   }
                       break;
                   default:
                       break;
               }
               if (iotMsg.MessageID == MessageId.CurrentLocalPositionMsgID)
                {
                    CurrentLocalPositionMsg positionMsg =
                        JsonUtility.FromJson<CurrentLocalPositionMsg>(iotMsg.MessageData);
                    if (iotMsg.SourceID == ObjectId.R_UAV_0)
                    {
                        _phsObjPosition = TF.Ros2Unity(new Vector3((float)positionMsg.position_x, (float)positionMsg.position_y,
                            (float)positionMsg.position_z));
                        _phsObjRotation = TF.Ros2Unity(new Quaternion((float) positionMsg.rotation_x,
                            (float) positionMsg.rotation_y, (float) positionMsg.rotation_z,
                            (float) positionMsg.rotation_w));
                    }
                    else if (iotMsg.SourceID == ObjectId.V_UAV_0)
                    {
                        _simObjPosition = TF.Ros2Unity(new Vector3((float)positionMsg.position_x, (float)positionMsg.position_y,
                            (float)positionMsg.position_z));  
                        _simObjRotation = TF.Ros2Unity(new Quaternion((float)positionMsg.rotation_x,
                            (float)positionMsg.rotation_y, (float)positionMsg.rotation_z,
                            (float)positionMsg.rotation_w));
                      //  Debug.Log("get simulation data");
                    }
                }
                

            }
        }

        // Update is called once per frame
        public void SendMessage(string message)
        {
            clientPub.Publish(PubTopic, Encoding.ASCII.GetBytes(message));
        }
        /*
           //send ref_position to V_UAV_0
           IOT_MSG iotMsg = new IOT_MSG();
               iotMsg.packet_id = message_id.VR_REF_UAV_POSITION_ID;
               iotMsg.packet_object_from = object_id.VR_Server;
               iotMsg.packet_object_to = object_id.V_UAV_0;
               REF_UAV_POSITION_MSG refUavPositionMsg = new REF_UAV_POSITION_MSG();
               refUavPositionMsg.ref_position_x = RefUavPosition.x;
               refUavPositionMsg.ref_position_y = RefUavPosition.y;
               refUavPositionMsg.ref_position_z = RefUavPosition.z;
               iotMsg.packet_data = JsonUtility.ToJson(refUavPositionMsg);
               string iotMsgJson = JsonUtility.ToJson(iotMsg);
               var id = clientPub.Publish(PubTopic, Encoding.ASCII.GetBytes(iotMsgJson));

               //send ref_position to R_UAV_0
               iotMsg.packet_object_to = object_id.R_UAV_0;
               iotMsgJson = JsonUtility.ToJson(iotMsg);
               id = clientPub.Publish(PubTopic, Encoding.ASCII.GetBytes(iotMsgJson));

               IsSendMessage = false;
        */
        void Update()
        {
            SimulationUAV.position = _simObjPosition;
            SimulationUAV.rotation = _simObjRotation.normalized;
            PhysicalUAV.position = _phsObjPosition;
            PhysicalUAV.rotation = _phsObjRotation.normalized;
        }
    }
}
