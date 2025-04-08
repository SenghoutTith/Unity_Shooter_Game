using UnityEngine;
using System.Collections.Generic; // Add this line

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactables> interactables = new List<Interactables>();

    private Interactables closestInteractable;

    private void Start() {
        Player player = GetComponent<Player>();
        player.controls.Character.UrMom.performed += ctx => InteractWithCloest();
    }

    private void InteractWithCloest() {
        if (closestInteractable == null) {
            Debug.LogWarning("No closest interactable found..");
            return;
        }
        closestInteractable?.Interaction();
        interactables.Remove(closestInteractable);
        UpdateClosestInteractables();
    }

    public void UpdateClosestInteractables()
    {
        closestInteractable?.HighlightActive(false);
        closestInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (Interactables interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        closestInteractable?.HighlightActive(true);
    }

    public List<Interactables> GetInteractables() => interactables;
}
