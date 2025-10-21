using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class GameStarter : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI Lvl;
     private List<string> levels = new List<string>();
    private int index = 0;
    private string Active;
    private bool gameStarted = false;
    
    public string GameSceneName = "Game";

    void Start()
    {
        if (IsServer)
        {
            levels.Add("Demo");
            Active = levels[0];
            startText.gameObject.SetActive(true);
            startText.text = "Appuyez sur K pour lancer la partie";
            Lvl.text = Active;
            // Chemin complet du dossier StreamingAssets
            string path = Application.streamingAssetsPath;

            // Récupère tous les .json
            var files = Directory.GetFiles(path, "*.json");

            
            Regex rgx = new Regex(@"save\d+\.json$", RegexOptions.IgnoreCase);

            var dataFiles = files
                .Where(f => rgx.IsMatch(Path.GetFileName(f)))
                .ToList();

            // Debug pour vérifier
            foreach (var f in dataFiles)
            {
                Debug.Log(f);
                levels.Add($"{f}");
            }
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
            if (Active == "Demo")
            {
                Debug.Log("L'hôte lance la partie !");
                gameStarted = true;
                startText.gameObject.SetActive(false);
                StartGame();
            }
            else
            {
                GameSceneName = "GameWithSpawn";
                GameSceneData.ActiveLevelName = Active;
                Debug.Log("L'hôte lance la partie avec un Custom !");
                gameStarted = true;
                startText.gameObject.SetActive(false);
                StartGame();
            }
            //StartGameServerRpc();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            index++;
            if(index >= levels.Count)
            {
                index = 0;
            }
            Active = levels[index];
            Lvl.text = Path.GetFileNameWithoutExtension(Active);
        }
        if (Input.GetKeyDown(KeyCode.E)) { 
            index--;
            if(index < 0)
            {
                index = levels.Count - 1;
            }
            Active = levels[index];
            Lvl.text = Path.GetFileNameWithoutExtension(Active);

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
