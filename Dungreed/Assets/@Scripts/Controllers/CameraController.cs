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
    void LateUpdate() //100개가 생성 되어야 한다면 50개 생성되고 카메라 그리고 50개가 생성되면 중간에 껴있기 때문에 떨림 현상이 일어남 그래서 LateUpdate를 사용함.
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
