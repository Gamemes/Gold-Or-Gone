using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Block
{
    /// <summary>
    /// 弹跳砖块
    /// </summary>
    public class BlockBounce : BlockBase
    {
        [SerializeField] float val;
        GameObject player;
        private void Awake()
        {
           if(val == 0)
           {
               val = 1;
           } 
           player = GameObject.Find("player");
        }

        private bool GetFace(GameObject theObject)
        {
            Vector3 point = theObject.transform.position - gameObject.transform.position;
            float x = System.Math.Abs(point.x);
            float y = System.Math.Abs(point.y);
            float thex = gameObject.transform.localScale.x + theObject.GetComponent<BoxCollider2D>().size.x;
            float they = gameObject.transform.localScale.y + theObject.GetComponent<BoxCollider2D>().size.y;

            if(thex - x >= they - y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other); 
            player = GameObject.Find("player");
        }
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            bool face = GetFace(player);
            if(face == true)
            {
                Debug.Log("up and down");
            }
            if(face == false)
            {
                Debug.Log("left and right");
            }
        }
    }
}
