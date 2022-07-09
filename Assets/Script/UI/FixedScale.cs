using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class FixedScale : MonoBehaviour
    {
        void Update()
        {
            if (this.transform.lossyScale.x < 0)
            {
                var pre = this.transform.localScale;
                this.transform.localScale = new(-pre.x, pre.y, pre.z);
            }
        }
    }
}
