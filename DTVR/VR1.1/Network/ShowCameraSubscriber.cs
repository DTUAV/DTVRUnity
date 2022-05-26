using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAV.Message;
using LCM.LCM;
using sensor_msgs;
using UnityEngine.UI;

namespace DTUAV.VR
{
    public class ShowCameraSubscriber: MonoBehaviour, LCMSubscriber
    {
        [Header("The Subscribe UAV Camera Topic of LCM Network")]
        public string UavCameraSubTopic;
        [Header("The Width of Image")]
        public int ImageWidth;
        [Header("The Height of Image")]
        public int ImageHeight;
        [Header("The UI Commonents for RawImage")]
        public RawImage ShowImage;
        private LCM.LCM.LCM _UavCameraSub;
        private Texture2D _cameraTexture2D;
        private bool _isShow = false;
        private sensor_msgs.CompressedImage _compressedImage;
        public bool ShowCamera(bool isShow)
        {
            _isShow = isShow;
            return true;
        }
        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            _compressedImage = new CompressedImage(ins);
            _cameraTexture2D.LoadRawTextureData(_compressedImage.data);
        }

        // Start is called before the first frame update
        void Start()
        {
            _UavCameraSub = new LCM.LCM.LCM();
            _UavCameraSub.Subscribe(UavCameraSubTopic, this);
            _cameraTexture2D = new Texture2D(ImageWidth,ImageHeight);
        }

        void OnGUI()
        {
            if (_isShow)
            {
                ShowImage.texture = _cameraTexture2D;
            }
        }
    }
}
