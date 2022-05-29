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

namespace DTUAV.Visualization_Module.Light
{
    public class FlashingLightNode : MonoBehaviour
    {
        // Start is called before the first frame update
        public float Hz;//The frequency of light flashing
        public float LowDensity;
        public float UpDensity;
        public Color LightColor;
        public UnityEngine.Light Light;
        private FlashingLight _flashingLight;
        void Start()
        {
            _flashingLight = new FlashingLight(Hz,LowDensity,UpDensity,LightColor,Light);
        }

        void OnDestroy()
        {
            _flashingLight.CloseLight();
        }
    }
}
