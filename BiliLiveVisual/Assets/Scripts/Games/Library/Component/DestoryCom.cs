using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BLVisual
{
    public class DestoryCom : MonoBehaviour
    {
        public bool isEditor;
        void Start()
        {
            if (isEditor)
            {
#if UNITY_EDITOR
                Object.Destroy(this);
#endif
            }
        }

    }
}