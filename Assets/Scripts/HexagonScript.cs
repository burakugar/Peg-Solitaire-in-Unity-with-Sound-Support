using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexagonScript : MonoBehaviour
{
    private bool isEmpty;

    // This indicates if there is a ball on top of Tile
    public bool GetIsEmpty() {
        return isEmpty;
    }

    // If false that means we can shouldn't able to select tile so we deactivate MeshCollider
    public void setIsEmpty(bool choice) {
        if (choice) {
            isEmpty = true;
            GetComponent<MeshCollider>().enabled = true;
        }
        else {
            isEmpty = false;
            GetComponent<MeshCollider>().enabled = false;
        }
    }

    // If Tile is selected and SelectedBall is not null we look if that move is possible
    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) // When panel is open, is can't be click to tile
            return;
        GameManagerScript.instance.isValidMoveHexagon(gameObject);
    }
}
