using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public override void FixedUpdateNetwork()
    {
        // Solo la autoridad de estado mueve de verdad
        if (!Object.HasStateAuthority)
            return;

        if (!GetInput(out NetworkInputData data))
            return;

        Vector3 moveDir = data.moveDirection;

        if (moveDir.sqrMagnitude < 0.0001f)
            return;

        // Movimiento usando Fusion (DeltaTime de la simulación)
        transform.position += moveDir * speed * Runner.DeltaTime;

        // Rotar hacia donde se mueve
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Runner.DeltaTime);
    }
}
