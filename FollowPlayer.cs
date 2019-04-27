using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public Transform player;
    public Vector3 offsetPosition;
    private Space offsetPositionSpace = Space.Self;

    void Start()
    {
        if (offsetPositionSpace == Space.Self)
        {
            player.position = player.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = player.position + offsetPosition;
        }
    }

    void LateUpdate()
    {
        offsetPosition = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetPosition;
        transform.position = player.position + offsetPosition;
        transform.LookAt(player.position);
    }
}
