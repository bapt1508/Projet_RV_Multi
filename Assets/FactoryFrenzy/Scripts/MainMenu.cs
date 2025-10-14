//using WSMulti.Gameplay.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField PseudoField, IPField, PortField;

        public GameObject ErrorWindow;

        

        /// <summary>
        /// Starts the host using the given connection data.
        /// </summary>
        /// 
        public void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void StartHost()
        {
            SetUtpConnectionData();
            var result = NetworkManager.Singleton.StartHost();
            
            if (result)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Playground", UnityEngine.SceneManagement.LoadSceneMode.Single);
                AssignPlayerInformation();
            }
        }

        /// <summary>
        /// Starts the Client using the given connection data.
        /// </summary>
        public void StartClient()
        {
            SetUtpConnectionData();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            var result = NetworkManager.Singleton.StartClient();
            StartCoroutine(Timeout());
            
            if (result)
            {
                StopCoroutine(Timeout());
            }
        }

        public IEnumerator Timeout()
        {
            yield return new WaitForSeconds(5f);
            Debug.LogError("StartClient failed!");
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            GameObject ErrorWindowInstance = Instantiate(ErrorWindow);
            ErrorWindowInstance.SetActive(true);
            errorManager ErrorManager = ErrorWindowInstance.GetComponent<errorManager>();
            ErrorManager.SetErrorText("TimeOut on Client Connexion");
            
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
            if (pseudo == "")
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
        /// Use sanitized IP and Port to set up the connection.
        /// </summary>
        void SetUtpConnectionData()
        {
            var sanitizedIPText = SanitizeAlphaNumeric(IPField.text);
            var sanitizedPortText = SanitizeAlphaNumeric(PortField.text);
            if (IPField.text == "")
            {
                sanitizedIPText = "127.0.0.1";
            }
            if (PortField.text == "")
            {
                sanitizedPortText = "4242";
            }

            ushort.TryParse(sanitizedPortText, out var port);
            var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(sanitizedIPText, port);
        }

        /// <summary>
        /// Sanitize user port InputField box allowing only alphanumerics and '.'
        /// </summary>
        /// <param name="dirtyString"> string to sanitize. </param>
        /// <returns> Sanitized text string. </returns>
        static string SanitizeAlphaNumeric(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
        }
    }
