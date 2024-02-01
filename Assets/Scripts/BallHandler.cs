using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{

    [SerializeField] private GameObject _BallPrefab;
    [SerializeField] private Rigidbody2D _BallPivotPoint;
    [SerializeField] float _DetachDelay = 0.15f;
    [SerializeField] float _RespawnDelay;

    Camera _MainCamera;
    private Rigidbody2D _CurrentBallRB;
    private SpringJoint2D _CurrentBallSpringJoint;

    bool isDragging;

    private void Start()
    {
        _MainCamera = Camera.main;

        SpawnNewBall();
    }

    void Update()
    {
        if (_CurrentBallRB == null) return;

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            return;
        }

        isDragging = true;
        _CurrentBallRB.isKinematic = true;

        Vector2 primaryTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldTouchPos = _MainCamera.ScreenToWorldPoint(primaryTouchPos);
        _CurrentBallRB.position = worldTouchPos;

    }

    private void SpawnNewBall()
    {
        GameObject ball = Instantiate(_BallPrefab, _BallPivotPoint.position, Quaternion.identity);
        _CurrentBallRB = ball.GetComponent<Rigidbody2D>();
        _CurrentBallSpringJoint = ball.GetComponent<SpringJoint2D>();

        _CurrentBallSpringJoint.connectedBody = _BallPivotPoint;
    }

    private void LaunchBall()
    {
        _CurrentBallRB.isKinematic = false;
        _CurrentBallRB = null;

        StartCoroutine(DetachBall());
    }

    private IEnumerator DetachBall()
    {
        yield return new WaitForSeconds(_DetachDelay);

        _CurrentBallSpringJoint.enabled = false;
        _CurrentBallSpringJoint = null;

        yield return new WaitForSeconds(_RespawnDelay);
        SpawnNewBall();
    }

}
