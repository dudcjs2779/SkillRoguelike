using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class InkExternalFunctions
{
    public void Bind(Story story, Animator emoteAnimator){
        story.BindExternalFunction("playEmote", (string emoteName) => PlayEmote(emoteName, emoteAnimator));
        story.BindExternalFunction("pauseDialogue", () => PauseDialogue());
    }

    public void Unbind(Story story){
        story.UnbindExternalFunction("playEmote");
        story.UnbindExternalFunction("pauseDialogue");
    }

    public void PlayEmote(string emoteName, Animator emoteAnimator){
        if (emoteAnimator != null)
        {
            emoteAnimator.Play(emoteName);
        }
        else
        {
            Debug.LogWarning("Tried to play emote, but emote animator was "
            + "not initialized when entering dialogue mode.");
        }
    }

    public void PauseDialogue(){
        DialogueManager.Instance.PauseDialogueMode();
    }
}
