using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharController : MonoBehaviour {

    public float WalkingSpeed = 5.0f;
    void UpdateWalking() {
        float horizontalInput = Input.GetAxis(xAxis) * Time.deltaTime * WalkingSpeed;
        float verticalInput = Input.GetAxis(yAxis) * Time.deltaTime * WalkingSpeed;
        bool shootInputPressed = Input.GetButtonUp(shootKey);
        var movement = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(movement, Space.World);
        bool isMoving = movement != Vector3.zero;
        if (isMoving)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(movement),
                800 * Time.deltaTime
            );
        }
        animator.SetBool("isWalking", isMoving);

        if (IsInRangeForShoot() && shootInputPressed)
        {
            EnterShootMode();
        }
    }
}
