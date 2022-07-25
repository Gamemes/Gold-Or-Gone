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
        public static void DelayInvoke(this MonoBehaviour obj, Action func, float time)
        {
            obj.StartCoroutine(DelayInvoke(func, time));
        }
        public static int countChar(this string str, char tar)
        {
            int res = 0;
            foreach (var s in str)
            {
                if (s == tar)
                    ++res;
            }
            return res;
        }
    }
}