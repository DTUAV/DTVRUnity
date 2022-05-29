using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAV.Visualization_Module.Light
{
    public class FlashingObjectNode : MonoBehaviour
    {
        // Start is called before the first frame update
        public float Hz;//The frequency of light flashing
        public Color LightColor;
        public Material LightMaterial;
        public FlashingObject _flashingObject;

        public bool SetColor(Color color)
        {
            _flashingObject.SetColor(color);
            return true;
        }
        void Start()
        {
            _flashingObject = new FlashingObject(Hz,LightColor,LightMaterial);
        }

        void OnDestroy()
        {
            _flashingObject.CloseLight();
        }

       
    }
}
