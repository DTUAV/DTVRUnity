using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCM.LCM;
using geometry_msgs;

namespace DTUAV.Network_Module.LCM_Network
{
    public class TestLcmCurrentVelocityPub : MonoBehaviour, LCM.LCM.LCMSubscriber
    {
        // Start is called before the first frame update
        public string TopicName;
        private LCM.LCM.LCM SubLcm;

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
           TwistStamp twistStamp = new TwistStamp(ins);
            Debug.Log("LocalVelocity.x: " + twistStamp.linear.x);
        }

        void Start()
        {
            SubLcm = new LCM.LCM.LCM();
            SubLcm.Subscribe(TopicName, this);
        }

    }
}