using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAV.Network_Module.Global_Network
{
    public class DTNetworkPackWrapperNode : MonoBehaviour
    {
        [Header("Configure LCM Network")]
        [Header("Computer control topic name in LCM Network")]
        public string ComputerControlMsgSubName;
        [Header("Uav command topic name in LCM Network")]
        public string UavCommandMsgSubName;
        [Header("Uav control topic name in LCM Network")]
        public string UavControlMsgSubName;
        [Header("The Publish Topic of LCM Network to ConnectorLcmNode Subscribe")]
        public string GlobalNetworkMessagePubName;
        [Header("Configure Object ID")]
        [Header("The Object Id of Message From")]
        public int SourceId;
        [Header("The Object Id of Message To")]
        public int TargetId;

        private DTNetworkPackWrapper _wrapperNode;
        // Start is called before the first frame update
        void Start()
        {
            _wrapperNode = new DTNetworkPackWrapper(ComputerControlMsgSubName,UavCommandMsgSubName,UavControlMsgSubName,GlobalNetworkMessagePubName,SourceId,TargetId);

        }
    }
}
