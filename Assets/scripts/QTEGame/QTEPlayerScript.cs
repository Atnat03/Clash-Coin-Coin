using System.Collections.Generic;
using UnityEngine;

public class QTEPlayerScript : MonoBehaviour
{
    public int score = 0;
    

    public Queue<QTEGameManager.ButtonDirection> inputList = new Queue<QTEGameManager.ButtonDirection>();


    void Start()
    {
        InitInpuList();
    }

    private int rnd = 0;
    private void InitInpuList()
    {
        for (int i = 0; i < 150; i++)
        {
            rnd = Random.Range(0, 4);
            inputList.Enqueue((QTEGameManager.ButtonDirection)rnd);
        }
    }

    public void RegisterInput(QTEGameManager.ButtonDirection input)
    {
        if (input == inputList.Peek())
        {
            inputList.Dequeue();
            score++;
        }
    }
    
}
