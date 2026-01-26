using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[DefaultExecutionOrder(-1)]
public class EnableInputs : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    
    public int numberManettes;

    void Awake()
    {
        StartCoroutine(SetupInputs());
    }

    private IEnumerator SetupInputs()
    {
        var input1 = player1.GetComponent<PlayerInput>();
        var input2 = player2.GetComponent<PlayerInput>();

        if (!input1 || !input2)
        {
            Debug.LogError("PlayerInput manquant !");
            yield break;
        }

        var gamepads = Gamepad.all;
        Debug.Log("Gamepads détectés: " + gamepads.Count);

        numberManettes = gamepads.Count;
        
        if (numberManettes >= 2)
        {
            Debug.Log("MODE: 2 Gamepads");

            Pair(input1, gamepads[0]);
            Pair(input2, gamepads[1]);

            input1.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            input2.SwitchCurrentControlScheme("Gamepad", gamepads[1]);

            input1.SwitchCurrentActionMap("Gamepad");
            input2.SwitchCurrentActionMap("Gamepad");
        }
        
        else if (numberManettes == 1)
        {
            Debug.Log("MODE: 1 Gamepad + 1 Keyboard");

            input1.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            input1.SwitchCurrentActionMap("Player1");
            
            Pair(input2, gamepads[0]);
            input2.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            input2.SwitchCurrentActionMap("Gamepad");
        }
        
        else
        {
            Debug.Log("MODE: 2 Keyboard Players");

            input1.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            input2.SwitchCurrentControlScheme("Keyboard", Keyboard.current);

            input1.SwitchCurrentActionMap("Player1");
            input2.SwitchCurrentActionMap("Player2");
        }

        input1.ActivateInput();
        input2.ActivateInput();

        Debug.Log("P1 Map = " + input1.currentActionMap.name);
        Debug.Log("P2 Map = " + input2.currentActionMap.name);
    }

    private void Pair(PlayerInput input, Gamepad pad)
    {
        if (!input.user.valid) return;

        InputUser.PerformPairingWithDevice(
            pad,
            input.user,
            InputUserPairingOptions.UnpairCurrentDevicesFromUser
        );
    }
}
