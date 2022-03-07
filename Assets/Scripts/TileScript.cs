using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TileScript : MonoBehaviour
{
    private bool isEmpty;

    // This indicates if there is a ball on top of Tile
    public bool GetIsEmpty() {
        return isEmpty;
    }

    // If false that means we can shouldn't able to select tile so we deactivate BoxCollider
    public void setIsEmpty(bool choice) {
        if (choice) {
            isEmpty = true;
            GetComponent<BoxCollider>().enabled = true;
        }
        else {
            isEmpty = false;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    // If Tile is selected and SelectedBall is not null we look if that move is possible
    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) // When panel is open, is can't be click to tile
            return;
        GameManagerScript.instance.isValidMove(gameObject);
    }
}
