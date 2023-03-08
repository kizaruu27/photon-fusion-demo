using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameMessegeHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] messegeTexts;
    private Queue messegeQueue = new Queue();

    public void OnGameMessegeReceived(string messege)
    {
        Debug.Log($"In Game Meessege Handler {messege}");
        messegeQueue.Enqueue(messege);

        if (messegeQueue.Count > 3)
            messegeQueue.Dequeue();

        int queueIndex = 0;

        foreach (string messegeInQueue in messegeQueue)
        {
            messegeTexts[queueIndex].text = messegeInQueue;
            queueIndex++;
        }
    }
}
