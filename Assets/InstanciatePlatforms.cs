using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class LevelSpawner : NetworkBehaviour
{
    private string levelJson;

    private SceneData sceneData;

    public override void OnNetworkSpawn()
    {
        levelJson = GameSceneData.ActiveLevelName;
        if (levelJson == null)
        {
            Debug.LogError("Aucun fichier JSON assigné au LevelSpawner !");
            return;
        }

        string jsonContent = File.ReadAllText(levelJson);

        sceneData = JsonUtility.FromJson<SceneData>(jsonContent);

        if (sceneData == null || sceneData.objects == null)
        {
            Debug.LogError("Impossible de parser les données du JSON !");
            return;
        }

        // Seul le serveur fait le spawn initial
        if (IsServer)
        {
            StartCoroutine(SpawnObjectsFromJson());
        }
    }

    private IEnumerator SpawnObjectsFromJson()
    {
        foreach (var objData in sceneData.objects)
        {
            yield return StartCoroutine(SpawnFromAddressables(objData));
        }
    }

    private IEnumerator SpawnFromAddressables(ObjectData objData)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(objData.name);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab introuvable pour l'adressableKey: {objData.name}");
                yield break;
            }

            GameObject instance = Instantiate(prefab, objData.position, objData.rotation);

            // Vérifie que le prefab a un NetworkObject
            NetworkObject netObj = instance.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                Debug.LogWarning($"Le prefab '{objData.name}' n'a pas de NetworkObject — il ne sera pas synchronisé !");
                yield break;
            }

            // Spawn réseau
            netObj.Spawn();

            Debug.Log($"Spawned network object: {objData.name} à {objData.position}");
        }
        else
        {
            Debug.LogWarning($"Échec du chargement de l'adressable: {objData.name}");
        }
    }
}
