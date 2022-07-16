using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static IEnumerator DelayInvoke(Action func, float time)
        {
            yield return new WaitForSeconds(time);
            func?.Invoke();
        }
    }
}