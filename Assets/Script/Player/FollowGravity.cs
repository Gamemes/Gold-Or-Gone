using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGravity : MonoBehaviour
{
    void Start()
    {
        Manager.MyGameManager.instance.stageManager.onGravityRotated += this.rotate;
    }
    void rotate(float dangle)
    {
        transform.Rotate(new Vector3(0, 0, dangle));

    }
    void Update()
    {

    }
}
