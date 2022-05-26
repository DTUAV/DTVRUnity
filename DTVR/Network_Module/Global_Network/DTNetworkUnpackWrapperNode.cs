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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAV.Network_Module.Global_Network
{
    public class DTNetworkUnpackWrapperNode : MonoBehaviour
    {
        [Header("Configure LCM Network")]
        [Header("UAVInfo topic name in LCM Network")]
        public string UAVInfoMsgPubName;
        [Header("The Subscribe Topic of LCM Network to ConnectorLcmNode Publisher")]
        public string GlobalNetworkMessageSubName;

        [Header("Configure Object ID")]
        [Header("The Object Id of Message From")]
        public int SourceId;

        [Header("The Object Id of Message To")]
        public int TargetId;

        private DTNetworkUnpackWrapper _dtNetworkUnpackWrapper;
        // Start is called before the first frame update
        void Start()
        {
            _dtNetworkUnpackWrapper = new DTNetworkUnpackWrapper(UAVInfoMsgPubName,GlobalNetworkMessageSubName,SourceId,TargetId);
        }
    }
}
