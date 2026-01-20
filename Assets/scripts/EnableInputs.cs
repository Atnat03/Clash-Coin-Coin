using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class EnableInputs : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    
    void Start()
    {
        StartCoroutine(AssignGamepadsAfterDelay());
    }

    private IEnumerator AssignGamepadsAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        var gamepads = Gamepad.all;
        if (gamepads.Count < 2)
        {
            Debug.LogWarning($"Pas assez de manettes : {gamepads.Count}. Inputs partagés possibles.");
            yield break;
        }

        var input1 = player1.GetComponent<PlayerInput>();
        var input2 = player2.GetComponent<PlayerInput>();

        if (input1 == null || input2 == null)
        {
            Debug.LogError("PlayerInput manquant sur un joueur !");
            yield break;
        }

        if (input1.user.valid)
        {
            InputUser.PerformPairingWithDevice(gamepads[0], input1.user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
            input1.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            input1.ActivateInput(); 
            input1.currentActionMap?.Enable(); 
        }
        else
        {
            Debug.LogWarning("User Player1 invalide – pairing auto peut échouer");
        }

        if (input2.user.valid)
        {
            InputUser.PerformPairingWithDevice(gamepads[1], input2.user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
            input2.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
            input2.ActivateInput();
            input2.currentActionMap?.Enable();
        }
        else
        {
            Debug.LogWarning("User Player2 invalide");
        }

        Debug.Log($"Player1 devices: {string.Join(", ", input1.user.pairedDevices)}");
        Debug.Log($"Player2 devices: {string.Join(", ", input2.user.pairedDevices)}");

        Debug.Log("Pairing + activation forcés ! Teste les boutons maintenant.");
    }    

}
