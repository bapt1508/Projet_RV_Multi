using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continuer : NetworkBehaviour
{


    public void OnContinue()
    {

        SceneManager.LoadScene("Startup");
    }


}
