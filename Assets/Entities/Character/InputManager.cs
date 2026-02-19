using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector3 moveDirection;
    public float lookAngle;
    public bool attackPressed;
    public bool blockPressed;

    public Vector3 desiredTravelPoint;
    public bool shouldTravelToTravelPoint = true;
}
