using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed=10f;
    private float hInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Rotate();
    }

    private void GetInput()
    {
        hInput = Input.GetAxis("Mouse X");
    }

    private void Rotate()
    {
        if (hInput == 0f)
        {
            return;
        }

        transform.Rotate(Vector3.up, hInput * rotationSpeed * Time.deltaTime);
    }
}
