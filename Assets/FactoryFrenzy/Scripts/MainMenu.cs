using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField PseudoField, IPField, PortField;
    public GameObject ErrorWindow;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartHost()
    {
        string ip = IPField.text.Trim();
        string portStr = PortField.text.Trim();

        if (string.IsNullOrEmpty(ip))
            ip = "127.0.0.1";

        if (string.IsNullOrEmpty(portStr))
            portStr = "4242";

        if (!IsValidIPAddress(ip))
        {
            ShowError("Adresse IP invalide !");
            return;
        }

        if (!IsValidPort(portStr))
        {
            ShowError("Port invalide ! (doit être entre 1 et 65535)");
            return;
        }

        SetUtpConnectionData(ip, portStr);
        bool result = NetworkManager.Singleton.StartHost();

        if (result)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Playground", LoadSceneMode.Single);
            AssignPlayerInformation();
        }
    }

    public void StartClient()
    {
        string ip = IPField.text.Trim();
        string portStr = PortField.text.Trim();

        if (string.IsNullOrEmpty(ip))
            ip = "127.0.0.1";

        if (string.IsNullOrEmpty(portStr))
            portStr = "4242";

        if (!IsValidIPAddress(ip))
        {
            ShowError("Adresse IP invalide !");
            return;
        }

        if (!IsValidPort(portStr))
        {
            ShowError("Port invalide ! (doit être entre 1 et 65535)");
            return;
        }

        SetUtpConnectionData(ip, portStr);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        bool result = NetworkManager.Singleton.StartClient();
        StartCoroutine(Timeout());

        if (result)
            StopCoroutine(Timeout());
    }

    public IEnumerator Timeout()
    {
        yield return new WaitForSeconds(5f);
        Debug.LogError("StartClient failed!");
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        ShowError("TimeOut on Client Connexion");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            AssignPlayerInformation();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void AssignPlayerInformation()
    {
        Debug.Log("attrib");
        PlayerData pm = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerData>();
        var pseudo = PseudoField.text;
        if (string.IsNullOrEmpty(pseudo))
        {
            Debug.Log("pseudo Random");
            pseudo = RandomString(5);
        }

        pm.SetPseudoServerRpc(pseudo);
    }

    public static string RandomString(int length)
    {
        System.Random rnd = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Configure le transport Unity avec l’adresse IP et le port validés.
    /// </summary>
    void SetUtpConnectionData(string ip, string portStr)
    {
        ushort.TryParse(portStr, out var port);
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ip, port);
    }

    private bool IsValidIPAddress(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }

    private bool IsValidPort(string portStr)
    {
        if (!ushort.TryParse(portStr, out ushort port))
            return false;

        return port > 0 && port <= 65535;
    }

    private void ShowError(string message)
    {
        Debug.LogError(message);
        GameObject errorWindowInstance = Instantiate(ErrorWindow);
        errorWindowInstance.SetActive(true);
        errorManager errorManager = errorWindowInstance.GetComponent<errorManager>();
        errorManager.SetErrorText(message);
    }
}
