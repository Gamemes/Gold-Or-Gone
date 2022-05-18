using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlodControler : MonoBehaviour
{
    public Transform player;
    public float rotateSpeed = 10f;
    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //transform.Rotate(new Vector3(0, 0, 90));
            StartCoroutine(rotate());
            Debug.Log($"rotation to {transform.rotation}");
        }
    }
    IEnumerator rotate(float angle = 90f)
    {
        float _ang = 0f;
        while (_ang < angle)
        {
            transform.RotateAround(player.position, Vector3.forward, rotateSpeed * Time.deltaTime);
            _ang += rotateSpeed * Time.deltaTime;
            yield return null;
        }
        transform.RotateAround(player.position, Vector3.forward, angle - _ang);
    }
}
