using UnityEngine;

public class Interactables : MonoBehaviour
{
    protected PlayerWeaponController weaponController;
    protected MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;

    private void Start() {
        if (mesh == null) {
            mesh = GetComponentInChildren<MeshRenderer>();
        }
        defaultMaterial = mesh.material;
    }

    protected void UpdateMeshAndMeshRenderer(MeshRenderer newMesh) {
        mesh = newMesh;
        defaultMaterial = mesh.material;
    }

    public virtual void Interaction() {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public void HighlightActive(bool active) {
        if (mesh == null) {
            mesh = GetComponentInChildren<MeshRenderer>();
            defaultMaterial = mesh.material;
        }
        if (active) {
            mesh.material = highlightMaterial;
        } else {
            mesh.material = defaultMaterial;
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (weaponController == null)
        {
            weaponController = other.GetComponent<PlayerWeaponController>();
        }
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction == null) {
            return;
        }
        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteractables();
    }

    protected virtual void OnTriggerExit(Collider other) {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction == null) {
            return;
        }
        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteractables();
    }
}
