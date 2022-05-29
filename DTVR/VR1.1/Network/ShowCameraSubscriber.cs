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
