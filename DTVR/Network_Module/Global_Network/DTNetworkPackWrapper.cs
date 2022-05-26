/*
 *
 *  GNU General Public License (GPL)
 *
 * Update Information:
 *                    First: 2022-5-11 In Guangdong University of Technology By Yuanlin Yang  Email: yongwang0808@163.com
 *
 *
 *
 *
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCM.LCM;
using lcm_iot_msgs;
using DTUAV.Message;
using geometry_msgs;
using DTUAV.Time;
using dtvr_msgs;
using ComputerControl = DTUAV.Message.ComputerControl;

namespace DTUAV.Network_Module.Global_Network
{
    public class DTNetworkPackWrapper
    {
        private int _sourceId;
        private int _targetId;

        private string _computerControlTopicName;
        private string _uavCommandTopicName;
        private string _uavControlTopicName;
        private string _networkTopicName;

        private LCM.LCM.LCM _computerControlSub;
        private LCM.LCM.LCM _uavCommandSub;
        private LCM.LCM.LCM _uavControlSub;

        private LCM.LCM.LCM _lcmMessagePub;
        private LcmIotMessage _lcmIotMessage;

        public string GetNetworkTopicName()
        {
            return _networkTopicName;
        }
        public string GetComputerControlTopicName()
        {
            return _computerControlTopicName;
        }
        public string GetUavCommandTopicName()
        {
            return _uavCommandTopicName;
        }
        public string GetUavControlTopicName()
        {
            return _uavControlTopicName;
        }

        public bool SetNetworkTopicName(string name)
        {
            _networkTopicName = name;
            return true;
        }
        public bool SetComputerControlTopicName(string name)
        {
            _computerControlTopicName = name;
            return true;
        }
        public bool SetUavCommandTopicName(string name)
        {
            _uavCommandTopicName = name;
            return true;
        }
        public bool SetUavControlTopicName(string name)
        {
            _uavControlTopicName = name;
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
        public bool LcmPublishMessage(string messageData, int messageId)
        {
            if (messageId == MessageId.ComputerControlID)
            {
                _lcmIotMessage.SourceID = _sourceId;
                _lcmIotMessage.TargetID = _targetId;
                _lcmIotMessage.MessageData = messageData;
                _lcmIotMessage.MessageID = messageId;
                _lcmIotMessage.TimeStamp = SimTime.GetSystemTimeStampMs();
                try
                {
                    _lcmMessagePub.Publish(_networkTopicName, _lcmIotMessage);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            else if ( messageId == MessageId.UavCommandID)
            {
                _lcmIotMessage.SourceID = _sourceId;
                _lcmIotMessage.TargetID = _targetId;
                _lcmIotMessage.MessageData = messageData;
                _lcmIotMessage.MessageID = messageId;
                _lcmIotMessage.TimeStamp = SimTime.GetSystemTimeStampMs();
                try
                {
                    _lcmMessagePub.Publish(_networkTopicName, _lcmIotMessage);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            else if (messageId == MessageId.UavControlID)
            {
                _lcmIotMessage.SourceID = _sourceId;
                _lcmIotMessage.TargetID = _targetId;
                _lcmIotMessage.MessageData = messageData;
                _lcmIotMessage.MessageID = messageId;
                _lcmIotMessage.TimeStamp = SimTime.GetSystemTimeStampMs();
                try
                {
                    _lcmMessagePub.Publish(_networkTopicName, _lcmIotMessage);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        public DTNetworkPackWrapper(string computerControlTopicName, string uavCommandTopicName, string uavControlTopicName, string networkTopicName, int sourceId, int targetId)
        {
            _computerControlTopicName = computerControlTopicName;
            _uavCommandTopicName = uavCommandTopicName;
            _uavControlTopicName = uavControlTopicName;
            _networkTopicName = networkTopicName;

            _sourceId = sourceId;
            _targetId = targetId;

            _computerControlSub = new LCM.LCM.LCM();
            _computerControlSub.Subscribe(_computerControlTopicName, new ComputerControlSubscriber(this));

            _uavCommandSub = new LCM.LCM.LCM();
            _uavCommandSub.Subscribe(_uavCommandTopicName, new UavCommandSubscriber(this));

            _uavControlSub = new LCM.LCM.LCM();
            _uavControlSub.Subscribe(_uavControlTopicName, new UavControlSubscriber(this));

            _lcmMessagePub = LCM.LCM.LCM.Singleton;
            _lcmIotMessage = new LcmIotMessage();
        }



        internal class ComputerControlSubscriber : LCMSubscriber
        {
            private DTNetworkPackWrapper _dTNetworkPackWrapper;
            public ComputerControlSubscriber(DTNetworkPackWrapper dTNetworkPackWrapper)
            {
                _dTNetworkPackWrapper = dTNetworkPackWrapper;
            }
            public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
            {
                dtvr_msgs.ComputerControl computerControl = new dtvr_msgs.ComputerControl(dins);
                DTUAV.Message.ComputerControl dtComputerControlMsg = new DTUAV.Message.ComputerControl();
                dtComputerControlMsg.IsClose = computerControl.IsClose;
                string messageData = JsonUtility.ToJson(dtComputerControlMsg);
                _dTNetworkPackWrapper.LcmPublishMessage(messageData, MessageId.ComputerControlID);
            }
        }

        internal class UavCommandSubscriber : LCMSubscriber
        {
            private DTNetworkPackWrapper _dTNetworkPackWrapper;
            public UavCommandSubscriber(DTNetworkPackWrapper dTNetworkPackWrapper)
            {
                _dTNetworkPackWrapper = dTNetworkPackWrapper;
            }
            public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
            {
                dtvr_msgs.UavCommand uavComand = new dtvr_msgs.UavCommand(dins);
                DTUAV.Message.UavCommand dtUavCommand = new DTUAV.Message.UavCommand();
                dtUavCommand.ComMode = uavComand.ComMode;
                dtUavCommand.IsArm = uavComand.IsArm;
                dtUavCommand.IsOffboard = uavComand.IsOffboard;
                dtUavCommand.IsStart = uavComand.IsStart;

                string messageData = JsonUtility.ToJson(dtUavCommand);
                _dTNetworkPackWrapper.LcmPublishMessage(messageData, MessageId.UavCommandID);

            }
        }

        internal class UavControlSubscriber : LCMSubscriber
        {
            private DTNetworkPackWrapper _dTNetworkPackWrapper;
            public UavControlSubscriber(DTNetworkPackWrapper dTNetworkPackWrapper)
            {
                _dTNetworkPackWrapper = dTNetworkPackWrapper;
            }
            public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
            {
                dtvr_msgs.UavControl uavControl = new dtvr_msgs.UavControl(dins);
                DTUAV.Message.UavControl dtUavControl = new DTUAV.Message.UavControl();
                dtUavControl.ComAX = uavControl.ComAX;
                dtUavControl.ComAY = uavControl.ComAY;
                dtUavControl.ComAZ = uavControl.ComAZ;
                dtUavControl.ComLX = uavControl.ComLX;
                dtUavControl.ComLY = uavControl.ComLY;
                dtUavControl.ComLZ = uavControl.ComLZ;
                dtUavControl.Mode = uavControl.Mode;
                string messageData = JsonUtility.ToJson(dtUavControl);
                _dTNetworkPackWrapper.LcmPublishMessage(messageData, MessageId.UavControlID);

            }
        }

    }
}
