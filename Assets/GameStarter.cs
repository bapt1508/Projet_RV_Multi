using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameStarter : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI startText;
    private bool gameStarted = false;
    public string GameSceneName = "Game";

    void Start()
    {
        if (IsServer)
        {
            startText.gameObject.SetActive(true);
            startText.text = "Appuyez sur K pour lancer la partie";
        }
        else
        {
            startText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsServer || gameStarted)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("L'hôte lance la partie !");
            gameStarted = true;
            startText.gameObject.SetActive(false);
            StartGame();
            //StartGameServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            GetComponentInParent<Canvas>().gameObject.SetActive(false); // plus utile
            return;
        }
    }

    /*[ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams rpcParams = default)
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc(ClientRpcParams rpcParams = default)
    {
        StartGame();
    }*/

    private void StartGame()
    {
        Debug.Log("La partie démarre !");
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene(GameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
