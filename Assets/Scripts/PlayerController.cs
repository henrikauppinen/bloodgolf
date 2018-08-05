using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  [SerializeField] float speed = 0.0f;
  [SerializeField] float maxSpeed = 10.0f;
  [SerializeField] float enginePower = 0.06f;
  [SerializeField] float brakePower = 0.08f;
  [SerializeField] float windResistancePower = 0.03f;

  void Update() {
    float steeringInput = Input.GetAxis("Horizontal") * Time.deltaTime * 120.0f;

    float throttle = Input.GetAxis("Vertical");

    if (throttle > 0) {
      accelerate();
    }
    else if (throttle < 0) {
      decelerate();
    }
    else {
      coast();
    }

    move();
    steer(steeringInput);
  }

  public void accelerate() {
    speed = speed >= maxSpeed ? maxSpeed : speed + enginePower;
  }

  public void decelerate() {
    float newSpeed = speed - brakePower;
    speed = newSpeed < 0 ? 0 : newSpeed;
  }

  public void coast() {
    float newSpeed = speed - windResistancePower;
    speed = newSpeed < 0 ? 0 : newSpeed;
  }

  public void move() {
    transform.Translate(0, 0, speed * Time.deltaTime);
  }

  public void steer(float steeringInput) {
    transform.Rotate(0, steeringInput, 0);
  }
}
