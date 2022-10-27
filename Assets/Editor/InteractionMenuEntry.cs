using UnityEditor;

using UnityEngine;

public static class InteractionMenuEntry
{
    private const string GroupingObjectName = "Interactions";
    
    [MenuItem("GameObject/Quest Game/" + nameof(Interaction), false, 0)]
    public static void Create()
    {
        var clickedGameObject = Selection.activeGameObject;
        if (clickedGameObject == null) { return; }
        var interactable = clickedGameObject.GetComponentInParent<Interactable>();
        GameObject interactions = interactable.transform.Find(GroupingObjectName)?.gameObject;

        if (interactions == null)
        {
            interactions = new GameObject(GroupingObjectName);
            GameObjectUtility.SetParentAndAlign(interactions, interactable.gameObject);
            Undo.RegisterCreatedObjectUndo(interactions, $"Create {GroupingObjectName}");
        }
        
        var interaction = new GameObject(nameof(Interaction), typeof(Interaction));
        GameObjectUtility.SetParentAndAlign(interaction, interactions);
        Undo.RegisterCreatedObjectUndo(interaction, $"Create {nameof(Interaction)}");
        Selection.activeGameObject = interaction;
    }

    [MenuItem("GameObject/Quest Game/" + nameof(Interaction), true, 0)]
    public static bool ValidateCreate()
    {
        var clickedGameObject = Selection.activeGameObject;
        if (clickedGameObject == null) { return false; }
        return clickedGameObject.GetComponentInParent<Interactable>() != null;
    }
}
