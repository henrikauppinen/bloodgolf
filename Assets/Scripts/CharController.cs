using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {

  Animator animator;
  GameObject ball;

  [SerializeField] float speed = 5.0f;

  bool shootPhase = false;
  float charge = 0.0f;

  private void Start() {
    animator = GetComponent<Animator>();
    ball = GameObject.FindWithTag("Ball");
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
      charge = Mathf.Clamp(charge + Time.deltaTime, 0, 5);
    }

    if (!shootInput && shootPhase) {
      shootPhase = false;
      shoot();
    }

    Debug.Log(charge);

  }

  void shoot() {
    Transform ballT = ball.GetComponent<Transform>();
    Rigidbody ballRb = ball.GetComponent<Rigidbody>();

    Vector3 direction = ballT.position - transform.position;
    float distance = direction.magnitude;
    Vector3 normalDirection = direction / distance;

    Debug.Log(normalDirection);
    normalDirection.y = 1.0f;
    ballRb.AddForce(normalDirection * charge * 100);
    charge = 0.0f;
  }
}
