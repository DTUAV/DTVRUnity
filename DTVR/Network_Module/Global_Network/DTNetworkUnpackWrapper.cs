/*
 *
 *  GNU General Public License (GPL)
 *
 * Update Information:
 *                    First: 2021-8-6 In Guangdong University of Technology By Yuanlin Yang  Email: yongwang0808@163.com
 *
 *
 *
 *
 *
 */
using LCM.LCM;
using lcm_iot_msgs;
using DTUAV.Message;
using geometry_msgs;
using DTUAV.Time;
using std_msgs;
using UnityEngine;
using dtvr_msgs;

namespace DTUAV.Network_Module.Global_Network
{
    public class DTNetworkUnpackWrapper
    {
        private int _sourceId;
        private int _targetId;

        private string _uavInfoTopicName;
        private string _networkSubTopicName;
        private LCM.LCM.LCM _uavInfoPub;
        private dtvr_msgs.UavInfo _uavInfo;
        private LCM.LCM.LCM _lcmMessageSub;
        private LcmIotMessage _lcmIotMessage;

        public string GetGlobalNetworkSubTopicName()
        {
            return _networkSubTopicName;
        }

        public string GetUavInfoTopicName()
        {
            return _uavInfoTopicName;
        }

        public bool SetGlobalNetworkSubTopicName(string name)
        {
            _networkSubTopicName = name;
            return true;
        }

        public bool SetUavInfoTopicName(string name)
        {
            _uavInfoTopicName = name;
            return true;
        }

        public int GetTargetId()
        {
            return _targetId;
        }

        public int GetSourceId()
        {
            return _sourceId;
        }

        public bool SetSourceId(int sourceId)
        {
            _sourceId = sourceId;
            return true;
        }

        public bool SetTargetId(int targetId)
        {
            _targetId = targetId;
            return true;
        }

        public LcmIotMessage GetLcmIotMessage()
        {
            return _lcmIotMessage;
        }

        public bool LcmPublishMessage(LcmIotMessage msg)
        {
            if (msg.TargetID == _targetId && msg.SourceID == _sourceId)
            {
                switch (msg.MessageID)
                {
                    case MessageId.UavInfoID:
                    {
                        DTUAV.Message.UavInfo dtUavInfo = JsonUtility.FromJson<DTUAV.Message.UavInfo>(msg.MessageData);
                        _uavInfo.IsArm = dtUavInfo.IsArm;
                        _uavInfo.AVelX = dtUavInfo.AVelX;
                        _uavInfo.AVelY = dtUavInfo.AVelY;
                        _uavInfo.AVelZ = dtUavInfo.AVelZ;
                        _uavInfo.FMode = (byte) dtUavInfo.FMode;
                        _uavInfo.LVelX = dtUavInfo.LVelX;
                        _uavInfo.LVelY = dtUavInfo.LVelY;
                        _uavInfo.LVelZ = dtUavInfo.LVelZ;
                        _uavInfo.NetPx4 = dtUavInfo.NetPx4;
                        _uavInfo.PosX = dtUavInfo.PosX;
                        _uavInfo.PosY = dtUavInfo.PosY;
                        _uavInfo.PosZ = dtUavInfo.PosZ;
                        _uavInfo.Remaining = dtUavInfo.Remaining;
                        _uavInfo.RotW = dtUavInfo.RotW;
                        _uavInfo.RotX = dtUavInfo.RotX;
                        _uavInfo.RotY = dtUavInfo.RotY;
                        _uavInfo.RotZ = dtUavInfo.RotZ;
                        _uavInfo.Voltage = dtUavInfo.Voltage;
                        _uavInfoPub.Publish(_uavInfoTopicName, _uavInfo);
                    }
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public DTNetworkUnpackWrapper(string uavInfoTopicName, string networkSubTopicName, int sourceId, int targetId)
        {
            _uavInfoTopicName = uavInfoTopicName;
            _networkSubTopicName = networkSubTopicName;
            _sourceId = sourceId;
            _targetId = targetId;

            _uavInfoPub = LCM.LCM.LCM.Singleton;
            _uavInfo = new dtvr_msgs.UavInfo();

            _lcmMessageSub = new LCM.LCM.LCM();
            _lcmMessageSub.Subscribe(_networkSubTopicName, new LcmIotMessageSubscriber(this));
            _lcmIotMessage = new LcmIotMessage();
        }


        internal class LcmIotMessageSubscriber : LCMSubscriber
        {
            private DTNetworkUnpackWrapper _dtNetworkUnpackWrapper;

            public LcmIotMessageSubscriber(DTNetworkUnpackWrapper dtNetworkUnpackWrapper)
            {
                _dtNetworkUnpackWrapper = dtNetworkUnpackWrapper;
            }

            public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
            {
                LcmIotMessage iotMessage = new LcmIotMessage(ins);
                _dtNetworkUnpackWrapper.LcmPublishMessage(iotMessage);
            }
        }
    }
}  