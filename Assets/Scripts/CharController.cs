using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {

  Animator animator;
  GameObject ball;
  Camera camera;

  [SerializeField] float speed = 5.0f;

  bool shootPhase = false;
  float charge = 0.0f;

  private void Start() {
    animator = GetComponent<Animator>();
    ball = GameObject.FindWithTag("Ball");
    camera = Camera.main;
  }

  void Update() {
    float horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
    float verticalInput = Input.GetAxis("Vertical") * Time.deltaTime * speed;
    bool shootInput = Input.GetKey(KeyCode.Space);

    Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

    transform.Translate(movement, Space.World);

    bool isMoving = movement != Vector3.zero;

    if (isMoving) {
      transform.rotation = Quaternion.LookRotation(movement);
    }

    animator.SetBool("isWalking", isMoving);
    animator.SetBool("isShooting", shootInput);
    if (shootInput) {
      shootPhase = true;
      charge = Mathf.Clamp(charge + Time.deltaTime * 5.0f, 0, 10);
    }

    if (!shootInput && shootPhase) {
      shootPhase = false;
      shoot();
    }

    updateCameraPosition(transform.position, ball.transform.position);

  }

  void shoot() {
    Transform ballT = ball.GetComponent<Transform>();
    Rigidbody ballRb = ball.GetComponent<Rigidbody>();

    Vector3 direction = ballT.position - transform.position;
    float distance = direction.magnitude;
    Vector3 normalDirection = direction / distance;

    normalDirection.y = 1.0f;
    ballRb.AddForce(normalDirection * charge * 100);
    charge = 0.0f;
  }

  void updateCameraPosition(Vector3 playerPosition, Vector3 ballPosition) {

    float cameraDistance = 10 + Mathf.Abs(ballPosition.x - playerPosition.x);
    Vector3 newCameraPosition = new Vector3(getXBetween(playerPosition, ballPosition), cameraDistance - 3, transform.position.z - cameraDistance);
    camera.transform.position = newCameraPosition;
  }

  float getXBetween(Vector3 a, Vector3 b) {
    return a.x + (b.x - a.x) / 2;
  }
}
