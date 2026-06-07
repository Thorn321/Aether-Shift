using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [SerializeField] private float floatAmount = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        transform.position = startPosition + Vector3.up * offset;
    }
}
