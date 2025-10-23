using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerOnPlatform : NetworkBehaviour
{
    private MoveManager currentPlatform;
    private CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void SetPlatform(MoveManager p) => currentPlatform = p;
    public void ClearPlatform(MoveManager p) { if (currentPlatform == p) currentPlatform = null; }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        if (currentPlatform != null)
            cc.Move(currentPlatform.PlatformVelocity * Time.fixedDeltaTime);
    }
}
