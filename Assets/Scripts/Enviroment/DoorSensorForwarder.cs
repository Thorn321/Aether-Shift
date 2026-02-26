using UnityEngine;

public class DoorSensorForwarder : MonoBehaviour
{
    private LockedDoor door;

    private void Awake()
    {
        door = GetComponentInParent<LockedDoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (door != null)
            door.TryUnlock(other);
    }
}