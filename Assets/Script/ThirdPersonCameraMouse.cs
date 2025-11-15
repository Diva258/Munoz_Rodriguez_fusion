using Fusion;
using UnityEngine;

public class ThirdPersonCameraMouse : NetworkBehaviour
{
    public float distance = 4f;
    public float height = 2f;
    public float mouseSensitivity = 2f;

    [HideInInspector] public float yaw = 0f;
    float pitch = 10f;

    private Camera cam;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            return;

        cam = Camera.main;

        // Bloqueamos el cursor solo al empezar a jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Si no es mi jugador, o no hay cámara, o el juego está en pausa, no hacemos nada
        if (!Object.HasInputAuthority || cam == null)
            return;

        if (Time.timeScale == 0f)
            return;

        // Mouse rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        cam.transform.position = transform.position + offset;
        cam.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }
}
