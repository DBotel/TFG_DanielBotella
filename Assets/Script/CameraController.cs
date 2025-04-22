using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float edgeScrollSize = 10f;
    public float zoomSpeed = 300f;
    public float minY = 10f;
    public float maxY = 80f;

    public float rotationSpeed = 100f;

    public bool locked = false;

    //Cursor
    public Texture2D customCursor;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(customCursor, hotSpot, cursorMode);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.L))locked=!locked;
        if (locked) return;
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.mousePosition.x < edgeScrollSize)
            moveDir.x = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            moveDir.x = 1f;
        if (Input.mousePosition.y < edgeScrollSize)
            moveDir.z = -1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            moveDir.z = 1f;

        // Normaliza para evitar que se mueva más en diagonal
        moveDir = transform.forward * moveDir.z + transform.right * moveDir.x;
        moveDir.y = 0;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 position = transform.position;
        position.y -= scroll * zoomSpeed * Time.deltaTime;
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // botón derecho
        {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotation, 0f, Space.World);
        }
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
} 