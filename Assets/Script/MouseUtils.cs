using UnityEngine;

public static class MouseUtils
{

    /// <summary>
    /// Devuelve la posición en mundo donde el cursor apunta sobre un plano horizontal en y = 0.
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("MouseUtils.GetMouseWorldPosition: no hay Camera.main en la escena");
            return Vector3.zero;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // Plano horizontal en y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        else
        {
            // Si no intersecta (muy raro), devolvemos el origen del rayo
            return ray.origin;
        }
    }

    /// <summary>
    /// Devuelve la posición en mundo donde el cursor apunta sobre un plano arbitrario.
    /// </summary>
    public static Vector3 GetMouseWorldPosition(Plane plane)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("MouseUtils.GetMouseWorldPosition: no hay Camera.main en la escena");
            return Vector3.zero;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        else
        {
            return ray.origin;
        }
    }
}