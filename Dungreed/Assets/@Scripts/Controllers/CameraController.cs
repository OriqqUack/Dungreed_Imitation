using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform _playerTransform;
    [SerializeField]
    Vector3 _cameraPosition;

    [SerializeField]
    Vector2 _center;
    [SerializeField]
    Vector2 _mapSize;

    [SerializeField]
    float _cameraMoveSpeed;
    float _height;
    float _width;

    void Start()
    {
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        _height = Camera.main.orthographicSize;
        _width = _height * Screen.width / Screen.height;
    }

    // Update is called once per frame
    void LateUpdate() //100���� ���� �Ǿ�� �Ѵٸ� 50�� �����ǰ� ī�޶� �׸��� 50���� �����Ǹ� �߰��� ���ֱ� ������ ���� ������ �Ͼ �׷��� LateUpdate�� �����.
    {
        transform.position = Vector3.Lerp(transform.position,
                                          _playerTransform.position + _cameraPosition,
                                          Time.deltaTime * _cameraMoveSpeed);
        float lx = _mapSize.x - _width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + _center.x, lx + _center.x);

        float ly = _mapSize.y - _height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + _center.y, ly + _center.y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
}
