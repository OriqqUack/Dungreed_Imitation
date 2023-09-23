using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate() //100개가 생성 되어야 한다면 50개 생성되고 카메라 그리고 50개가 생성되면 중간에 껴있기 때문에 떨림 현상이 일어남 그래서 LateUpdate를 사용함.
    {
        if (Target == null)
            return;

        transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, -10);
    }
}
