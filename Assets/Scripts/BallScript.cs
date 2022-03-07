using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallScript : MonoBehaviour
{

    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) // When panel is open, is can't be click to ball
            return;
        if (GameManagerScript.SelectedBall != null)  // changes SelectedBalls's color to normal
            GameManagerScript.SelectedBall.GetComponent<Renderer>().material.color = GameManagerScript.instance.ballColor;
        GetComponent<Renderer>().material.color = Color.red;    // changes current balls color to red
        GameManagerScript.SelectedBall = gameObject;        // indicate current selected ball through game manager
    }
}
