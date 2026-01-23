using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class EnableInputs : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    
    public int numberManettes;

    void Start()
    {
        StartCoroutine(SetupInputs());
    }

    private IEnumerator SetupInputs()
    {
        yield return new WaitForSeconds(0.3f);

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

            input1.SwitchCurrentActionMap("Gamepad");
            input2.SwitchCurrentActionMap("Gamepad");
        }
        
        else if (numberManettes == 1)
        {
            Debug.Log("MODE: 1 Gamepad + 1 Keyboard");

            Pair(input1, gamepads[0]);
            input1.SwitchCurrentActionMap("Player1");

            input2.SwitchCurrentActionMap("Gamepad");
        }
        
        else
        {
            Debug.Log("MODE: 2 Keyboard Players");

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
            input.user
        );
    }
}
