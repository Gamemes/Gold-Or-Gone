using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UISpaced : MonoBehaviour
{
    public Sprite OnImage;
    public Sprite OffImage;
    public float space = 0.5f;
    protected List<Image> images;
    protected virtual void Awake()
    {
        images = new List<Image>();
        images.AddRange(GetComponentsInChildren<Image>());
    }
    protected virtual void Update()
    {
        for (int i = 0; i < images.Count; ++i)
        {
            images[i].transform.localPosition = new Vector3(i * space, 0, 0);
        }
    }
    protected virtual void OnDrawGizmos()
    {
        if (images == null)
        {
            images = new List<Image>();
        }
        if (images.Count == 0)
        {
            images.AddRange(GetComponentsInChildren<Image>());
        }
        for (int i = 0; i < images.Count; ++i)
        {
            images[i].transform.localPosition = new Vector3(i * space, 0, 0);
        }
    }
}