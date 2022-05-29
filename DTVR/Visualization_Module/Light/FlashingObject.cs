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
using System.Threading;

namespace DTUAV.Visualization_Module.Light
{
    public class FlashingObject
    {
        private float _hz;//The frequency of light flashing
        private Color _color;
        private Thread _thread;
        private int _sleepTime;
        private bool _isRun;
        private Material _material;
        private Color _curColor;
        public bool SetHz(float hz)
        {
            _hz = hz;
            _sleepTime = (int)((1.0 / _hz) * 1000);
            return true;
        }

       

        public bool SetColor(Color color)
        {
            _color = color;
            _material.color = _color;
            return true;
        }
        public FlashingObject(float hz, Color color,Material mat)
        {
            _hz = hz;
            _color = color;
            _sleepTime = (int)((1.0 / _hz) * 1000);
            _curColor = Color.white;
            _isRun = true;
            _material = mat;
            /*
            Loom.RunAsync(
                () =>
                {
                    _thread = new Thread(Run);
                    _thread.IsBackground = true;
                    _thread.Start();
                }
            );
            */

        }

        private void Run()
        {
            while (_isRun)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _curColor = (_curColor == Color.white) ? _color : Color.white;
                    _material.color = _curColor;
                });
                System.Threading.Thread.Sleep(_sleepTime);
            }

        }

        public bool CloseLight()
        {
            _isRun = false;
            return true;
        }


    }
}
