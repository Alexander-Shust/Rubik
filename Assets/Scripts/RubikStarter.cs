using UnityEngine;

public class RubikStarter : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        var offset = _speed * deltaTime;
        var angles = transform.eulerAngles;
        var newAngles = new Vector3(angles.x + offset, angles.y + offset, angles.z + offset);
        transform.eulerAngles = newAngles;
    }
}
