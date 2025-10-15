using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;
using StarterAssets;
using Unity.VisualScripting;

public class PlayerData : NetworkBehaviour
    {
        public PlayerInput PlayerInput;
        public GameObject Camera;
        public StarterAssets.StarterAssetsInputs StarterAssets;
        public StarterAssets.ThirdPersonController ThirdPersonController;
        public NetworkTransform networkTransform;


    public NetworkVariable<FixedString64Bytes> PlayerPseudo = new NetworkVariable<FixedString64Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
        
        public override void OnNetworkSpawn()
        {
        
            if (!IsOwner)
            {
                return;
            }
            transform.SetPositionAndRotation(new Vector3(transform.position.x, 1.5f, transform.position.z), transform.rotation);
            PlayerInput.enabled = true;
            Camera.SetActive(true);
            StarterAssets.enabled = true;
            ThirdPersonController.enabled = true;   
            

        }

        public void TeleportTo(Vector3 position, Quaternion rotation)
        {

            if (IsOwner)
            {
                Debug.Log("TP1");
                var cc = GetComponent<CharacterController>();
                if (cc) cc.enabled = false;
                transform.SetPositionAndRotation(position, rotation);
                if (cc) cc.enabled = true;
            }
            else
            {
                Debug.Log("TP2");
                StartCoroutine(TeleportationClient(position, rotation));
                
            }
            
            
        }
        public IEnumerator TeleportationClient(Vector3 position, Quaternion rotation)
        {
            yield return null;
            TeleportClientRpc(position, rotation);
        }

        [ClientRpc]
        public void TeleportClientRpc(Vector3 position, Quaternion rotation)
        {
            var cc = GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            if (cc) cc.enabled = true;
        }

        




        [ServerRpc(RequireOwnership = true)]
        public void SetPseudoServerRpc(string pseudo)
        {
            PlayerPseudo.Value = new FixedString64Bytes(pseudo);
        }
    }
