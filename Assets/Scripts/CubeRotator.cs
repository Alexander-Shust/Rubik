using UnityEngine;
using UnityEngine.UI;

public class CubeRotator : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private RubikManager _manager;

    [SerializeField]
    private float _rotateSpeed = 1.0f;

    private Quaternion _startRotation;

    private bool _isPaused;

    public bool IsPaused { get => _isPaused;
        set
        {
            _isPaused = value;
            _startButton.gameObject.SetActive(value);
            _pauseButton.gameObject.SetActive(!value);
        }
    }

    public void ResetRotation()
    {
        IsPaused = true;
        if (_manager.IsMoving) return;
        transform.rotation = _startRotation;
    }

    private void Awake()
    {
        IsPaused = true;
        _startRotation = transform.rotation;
    }

    private void Update()
    {
        if (_isPaused) return;
        if (_manager.IsMoving)
        {
            IsPaused = true;
            return;
        }
        var deltaTime = Time.deltaTime;
        transform.Rotate(Vector3.up, _rotateSpeed * deltaTime);
    }
}