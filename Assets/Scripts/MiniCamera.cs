using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamera : MonoBehaviour
{

    void Start()
    {
        float ratio = (float)Screen.width / Screen.height;
        this.GetComponent<Camera>().rect = new Rect((1 - 0.2f), (1 - 0.2f * ratio), 0.2f, 0.2f * ratio);
    }
}
