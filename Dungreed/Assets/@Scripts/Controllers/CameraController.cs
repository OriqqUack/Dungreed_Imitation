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
    void LateUpdate() //100���� ���� �Ǿ�� �Ѵٸ� 50�� �����ǰ� ī�޶� �׸��� 50���� �����Ǹ� �߰��� ���ֱ� ������ ���� ������ �Ͼ �׷��� LateUpdate�� �����.
    {
        if (Target == null)
            return;

        transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, -10);
    }
}
