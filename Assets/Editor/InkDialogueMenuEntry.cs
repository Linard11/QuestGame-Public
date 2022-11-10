using System.Reflection;

using UnityEditor;
using UnityEditor.Events;

using UnityEngine.Events;

public static class InkDialogueMenuEntry
{
    private const string GroupingObjectName = "Interactions";
    
    [MenuItem("GameObject/Quest Game/" + nameof(InkDialogue), false, 1)]
    public static void Create()
    {
        var clickedGameObject = Selection.activeGameObject;
        if (clickedGameObject == null) { return; }

        bool hasInteraction = clickedGameObject.TryGetComponent<Interaction>(out var interaction);
        if (!hasInteraction)
        {
            InteractionMenuEntry.Create();
            interaction = Selection.activeGameObject.GetComponent<Interaction>();
        }
        
        var inkDialogue = Undo.AddComponent<InkDialogue>(interaction.gameObject);
        Undo.RecordObject(inkDialogue, $"Add event");
        
        var onInteracted = typeof(Interaction).GetField("onInteracted", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(interaction) as UnityEvent;
        UnityEventTools.AddVoidPersistentListener(onInteracted, inkDialogue.StartDialogue);
        Undo.SetCurrentGroupName(hasInteraction ? $"Add {nameof(InkDialogue)} to {nameof(Interaction)}" : $"Create {nameof(Interaction)} and {nameof(InkDialogue)}");
    }

    [MenuItem("GameObject/Quest Game/" + nameof(InkDialogue), true, 1)]
    public static bool ValidateCreate()
    {
        var clickedGameObject = Selection.activeGameObject;
        if (clickedGameObject == null) { return false; }
        return clickedGameObject.GetComponentInParent<Interactable>() != null;
    }
}
