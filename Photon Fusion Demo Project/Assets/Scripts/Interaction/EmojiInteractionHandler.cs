using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class EmojiInteractionHandler : NetworkBehaviour
{
    [SerializeField] private Image emojiImage;
    [SerializeField] private Button emojiButton;

    private void Awake()
    {
        emojiButton = GameObject.Find("Emoji_btn").GetComponent<Button>();
    }

    private void Start()
    {
        emojiImage.gameObject.SetActive(false);
        emojiButton.onClick.AddListener(RPC_ShowEmoji);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_ShowEmoji()
    {
        StartCoroutine(ShowEmojiCO_RPC());
    }
    
    IEnumerator ShowEmojiCO_RPC()
    {
        emojiImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        emojiImage.gameObject.SetActive(false);
    }
}
