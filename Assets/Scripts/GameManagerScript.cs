using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    // So every class can use this class as static without creating object
    public static GameManagerScript instance = null;

    // Everytime a ball is selected by mouse it will make this object itself
    static public GameObject SelectedBall = null;

    // Classic square map variables
    [SerializeField] GameObject Maps;
    public GameObject[] ballRows;
    public GameObject[] tileRows;
    private GameObject[][] Balls;
    private GameObject[][] Tiles;


    // Hexagon map variables
    [SerializeField] GameObject HexagonMap;
    public GameObject hexagonBallRow;
    public GameObject hexagonTileRow;
    private GameObject[][] hexagonBalls;
    private GameObject[][] hexagonTiles;

    // For computing score
    private int activeCount;
  
    // Sound Effects 
    public AudioSource ballSound;
    public AudioSource winSound;
    public AudioSource loseSound;

    // For changing/activating/deactivating GameObjects for different maps or ending
    public GameObject Panel;
    public Text panelText;
    public GameObject playTurnButton;
    public GameObject SaveButton;
    public GameObject SaveNameInput;
    public Text SaveNameText;
    public GameObject GameSavedPanel;
    public Text GameSavedText;
    public InputField SaveNameInputField;

    // Normal ball color for changing balls color to back from BallScript.cs
    public Color ballColor;

    // For computing all possible moves while playing with computer
    private List<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>> moveList;


    private void Awake() {  
        instance = this;    
    }

    private void Start() {
        activeCount = 0;  

        if (!EnctranceManagerScript.instance.AIChoice) // Turn Computer's play button on
            playTurnButton.SetActive(true);


        // For all square maps and hexagon map create variables, fill it with objects through inspector, if loading a game refresh balls activeness
        if (EnctranceManagerScript.instance.levelChoice != 6) { 
            HexagonMap.SetActive(false);

            Balls = new GameObject[9][];
            Tiles = new GameObject[9][];
            for (int i = 0;i < 9;i++) {
                Balls[i] = new GameObject[9];
                Tiles[i] = new GameObject[9];
            }

            for (int i = 0;i < 9;i++) {
                for (int j = 0;j < 9;j++) {
                    Balls[i][j] = ballRows[i].transform.GetChild(j).gameObject;
                    Tiles[i][j] = tileRows[i].transform.GetChild(j).gameObject;
                }
            }
            ballColor = Balls[0][0].GetComponent<Renderer>().material.GetColor("_Color");


            for (int i = 0;i < 9;i++) {
                for (int j = 0;j < 9;j++)
                    if (!EnctranceManagerScript.instance.Tiles[j][i]) {
                        Tiles[j][i].SetActive(false);
                        Balls[j][i].SetActive(false);
                    }

            }

            if (EnctranceManagerScript.instance.isLoaded) {
                for (int i = 0;i < 9;i++) {
                    for (int j = 0;j < 9;j++) {
                        if (!Tiles[j][i].activeSelf)                           
                            continue;
                        if (EnctranceManagerScript.instance.loadedBalls[j][i]) {
                            Tiles[j][i].GetComponent<TileScript>().setIsEmpty(false);
                            Balls[j][i].SetActive(true);
                        }
                        else {
                            Tiles[j][i].GetComponent<TileScript>().setIsEmpty(true);
                            Balls[j][i].SetActive(false);
                        }
                    }
                }
            }
            else {
                Balls[EnctranceManagerScript.instance.EmptySpace.Key][EnctranceManagerScript.instance.EmptySpace.Value].SetActive(false);
                Tiles[EnctranceManagerScript.instance.EmptySpace.Key][EnctranceManagerScript.instance.EmptySpace.Value].GetComponent<TileScript>().setIsEmpty(true);
            }
            EnctranceManagerScript.instance.isLoaded = false;
        }
        else {
            Maps.SetActive(false);
            hexagonBalls = new GameObject[5][];
            hexagonTiles = new GameObject[5][];
            int k = 0;
            for (int i = 0;i < 5;i++) {
                hexagonBalls[i] = new GameObject[i + 1];
                hexagonTiles[i] = new GameObject[i + 1];
                for (int j = 0;j < i + 1;j++, k++) {
                    hexagonBalls[i][j] = hexagonBallRow.transform.GetChild(k).gameObject;
                    hexagonTiles[i][j] = hexagonTileRow.transform.GetChild(k).gameObject;
                }
            }
            ballColor = hexagonBalls[0][0].GetComponent<Renderer>().material.GetColor("_Color");
            if (EnctranceManagerScript.instance.isLoaded) {
                for (int i = 0;i < 5;i++) {
                    for (int j = 0;j < i + 1;j++) {
                        if (EnctranceManagerScript.instance.loadedBalls[i][j]) {
                            hexagonTiles[i][j].GetComponent<HexagonScript>().setIsEmpty(false);
                            hexagonBalls[i][j].SetActive(true);

                        }
                        else {
                            hexagonTiles[i][j].GetComponent<HexagonScript>().setIsEmpty(true);
                            hexagonBalls[i][j].SetActive(false);
                        }
                    }
                }
            }
            else {
                hexagonBalls[0][0].SetActive(false);
                hexagonTiles[0][0].GetComponent<HexagonScript>().setIsEmpty(true);
            }
            EnctranceManagerScript.instance.isLoaded = false;
        }

        // If computer plays compute first move
        if (!EnctranceManagerScript.instance.AIChoice)
            computeMoves();
    }

    // For square maps through SelectedBall's and SelectedTile's position, compute if scenario is possible, if yes then do it, play ball sound and calculate if game is ended
    public void isValidMove(GameObject SelectedTile) {
        if (SelectedBall == null)
            return;
 

        Vector3 ballPosition = SelectedBall.transform.position;
        int ballX = (int)SelectedBall.transform.position.x;
        int ballZ = (int)SelectedBall.transform.position.z;

        Vector3 tilePosition = SelectedTile.transform.position;
        int tileX = (int)SelectedTile.transform.position.x;
        int tileZ = (int)SelectedTile.transform.position.z;

        if (ballZ == tileZ) {
            if (ballX - tileX == 2 && !Tiles[ballZ][ballX - 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[ballZ][ballX - 2].GetComponent<TileScript>().GetIsEmpty()) {
                Tiles[ballZ][ballX - 1].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX - 1].SetActive(false);
                Balls[ballZ][ballX - 2].SetActive(true);
                Tiles[ballZ][ballX - 2].GetComponent<TileScript>().setIsEmpty(false);
                Tiles[ballZ][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX].SetActive(false);
                ballSound.Play();
                isEnd();
            }
            else if (ballX - tileX == -2 && !Tiles[ballZ][ballX + 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[ballZ][ballX + 2].GetComponent<TileScript>().GetIsEmpty()) {
                Tiles[ballZ][ballX + 1].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX + 1].SetActive(false);
                Balls[ballZ][ballX + 2].SetActive(true);
                Tiles[ballZ][ballX + 2].GetComponent<TileScript>().setIsEmpty(false);
                Tiles[ballZ][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX].SetActive(false);
                ballSound.Play();
                isEnd();
            }

        }
        else if (ballX == tileX) {
            if (ballZ - tileZ == 2 && !Tiles[ballZ - 1][ballX].GetComponent<TileScript>().GetIsEmpty() && Tiles[ballZ - 2][ballX].GetComponent<TileScript>().GetIsEmpty()) {
                Tiles[ballZ - 1][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ - 1][ballX].SetActive(false);
                Balls[ballZ - 2][ballX].SetActive(true);
                Tiles[ballZ - 2][ballX].GetComponent<TileScript>().setIsEmpty(false);
                Tiles[ballZ][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX].SetActive(false);
                ballSound.Play();
                isEnd();
            }
            else if (ballZ - tileZ == -2 && !Tiles[ballZ + 1][ballX].GetComponent<TileScript>().GetIsEmpty() && Tiles[ballZ + 2][ballX].GetComponent<TileScript>().GetIsEmpty()) {
                Tiles[ballZ + 1][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ + 1][ballX].SetActive(false);
                Balls[ballZ + 2][ballX].SetActive(true);
                Tiles[ballZ + 2][ballX].GetComponent<TileScript>().setIsEmpty(false);
                Tiles[ballZ][ballX].GetComponent<TileScript>().setIsEmpty(true);
                Balls[ballZ][ballX].SetActive(false);
                ballSound.Play();
                isEnd();
            }
        }
    }

    // For hexagon map through SelectedBall's and SelectedTile's position, compute if scenario is possible, if yes then do it, play ball sound and calculate if game is ended
    public void isValidMoveHexagon(GameObject SelectedTile) {
        if (SelectedBall == null)
            return;

        int ballColumn = 0, ballRow = 0;
        for (ballRow = 0;ballRow < 5;ballRow++) {
            bool endLoop = false;
            for (ballColumn = 0;ballColumn < ballRow + 1;ballColumn++) {
                if (SelectedBall.Equals(hexagonBalls[ballRow][ballColumn])) {
                    endLoop = true;
                    break;
                }
            }
            if (endLoop)
                break;
        }

        
        int tileColumn = 0, tileRow= 0;
        for (tileRow = 0;tileRow < 5;tileRow++) {
            bool endLoop = false;
            for (tileColumn = 0;tileColumn < tileRow + 1;tileColumn++) {
                if (SelectedTile.Equals(hexagonTiles[tileRow][tileColumn])) {
                    endLoop = true;
                    break;
                }
            }
            if (endLoop)
                break;
        }


        if(tileColumn == ballColumn) {
            if (tileRow - ballRow == 2 && !hexagonTiles[ballRow + 1][ballColumn].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow + 2][ballColumn].GetComponent<HexagonScript>().GetIsEmpty()) {
                hexagonTiles[ballRow + 1][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow + 1][ballColumn].SetActive(false);
                hexagonBalls[ballRow + 2][ballColumn].SetActive(true);
                hexagonTiles[ballRow + 2][ballColumn].GetComponent<HexagonScript>().setIsEmpty(false);
                hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn].SetActive(false);
                ballSound.Play();
                isEndHexagon();
            }
            else if (tileRow - ballRow == -2 && !hexagonTiles[ballRow - 1][ballColumn].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow - 2][ballColumn].GetComponent<HexagonScript>().GetIsEmpty()) {
                hexagonTiles[ballRow - 1][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow - 1][ballColumn].SetActive(false);
                hexagonBalls[ballRow - 2][ballColumn].SetActive(true);
                hexagonTiles[ballRow - 2][ballColumn].GetComponent<HexagonScript>().setIsEmpty(false);
                hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn].SetActive(false);
                ballSound.Play();
                isEndHexagon();
            }
        }
        else if(tileRow == ballRow) {
            if (tileColumn - ballColumn == 2 && !hexagonTiles[ballRow][ballColumn + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow][ballColumn + 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                hexagonTiles[ballRow][ballColumn + 1].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn + 1].SetActive(false);
                hexagonBalls[ballRow][ballColumn + 2].SetActive(true);
                hexagonTiles[ballRow][ballColumn + 2].GetComponent<HexagonScript>().setIsEmpty(false);
                hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn].SetActive(false);
                ballSound.Play();
                isEndHexagon();
            }
            else if (tileColumn - ballColumn == -2 && !hexagonTiles[ballRow][ballColumn - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow][ballColumn - 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                hexagonTiles[ballRow][ballColumn - 1].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn - 1].SetActive(false);
                hexagonBalls[ballRow][ballColumn - 2].SetActive(true);
                hexagonTiles[ballRow][ballColumn - 2].GetComponent<HexagonScript>().setIsEmpty(false);
                hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
                hexagonBalls[ballRow][ballColumn].SetActive(false);
                ballSound.Play();
                isEndHexagon();
            }

        }
        else if((tileColumn - ballColumn == 2) && (tileRow - ballRow == 2) && !hexagonTiles[ballRow + 1][ballColumn + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow + 2][ballColumn + 2].GetComponent<HexagonScript>().GetIsEmpty()) {
            hexagonTiles[ballRow + 1][ballColumn + 1].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[ballRow + 1][ballColumn + 1].SetActive(false);
            hexagonBalls[ballRow + 2][ballColumn + 2].SetActive(true);
            hexagonTiles[ballRow + 2][ballColumn + 2].GetComponent<HexagonScript>().setIsEmpty(false);
            hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[ballRow][ballColumn].SetActive(false);
            ballSound.Play();
            isEndHexagon();
        }
        else if ((tileColumn - ballColumn == -2) && (tileRow - ballRow == -2) && !hexagonTiles[ballRow - 1][ballColumn - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[ballRow - 2][ballColumn - 2].GetComponent<HexagonScript>().GetIsEmpty()) {
            hexagonTiles[ballRow - 1][ballColumn - 1].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[ballRow - 1][ballColumn - 1].SetActive(false);
            hexagonBalls[ballRow - 2][ballColumn - 2].SetActive(true);
            hexagonTiles[ballRow - 2][ballColumn - 2].GetComponent<HexagonScript>().setIsEmpty(false);
            hexagonTiles[ballRow][ballColumn].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[ballRow][ballColumn].SetActive(false);
            ballSound.Play();
            isEndHexagon();
        }
    }


    // For square maps look if any of balls scenarios is not possible to move then game is not ended, return false;
    // else set EndPanel active, display score, play sound and disable computer play button to prevent possible bugs
    private bool isEnd() {
        activeCount = 0;
        for (int i = 0;i < 9;i++) {
            for (int j = 0;j < 9;j++) {
                if (!Tiles[j][i].activeSelf | !Balls[j][i].activeSelf)
                    continue;
                activeCount++;
                if (i + 2 < 9 && !Tiles[j][i + 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[j][i + 2].GetComponent<TileScript>().GetIsEmpty())
                    return false;
                if (i - 2 > -1 && !Tiles[j][i - 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[j][i - 2].GetComponent<TileScript>().GetIsEmpty())
                    return false;
                if (j + 2 < 9 && !Tiles[j + 1][i].GetComponent<TileScript>().GetIsEmpty() && Tiles[j + 2][i].GetComponent<TileScript>().GetIsEmpty())
                    return false;
                if (j - 2 > -1 && !Tiles[j - 1][i].GetComponent<TileScript>().GetIsEmpty() && Tiles[j - 2][i].GetComponent<TileScript>().GetIsEmpty())
                    return false;
            }
        }


        for (int i = 0;i < 9;i++) {
            for (int j = 0;j < 9;j++) {
                if (Tiles[j][i].activeSelf)
                    Tiles[j][i].GetComponent<BoxCollider>().enabled = false;
                if (Balls[j][i].activeSelf)
                    Balls[j][i].GetComponent<BoxCollider>().enabled = false;
            }
        }

        Panel.SetActive(true);
        panelText.text = "Game Ended\n\n  Score:" + activeCount;
       
        if (activeCount == 1)
            winSound.Play();
        else
            loseSound.Play();
        playTurnButton.SetActive(false);
        return true;
    }

    // For hexagon map look if any of balls scenarios is not possible to move then game is not ended, return false;
    // else set EndPanel active, display score, play sound and disable computer play button to prevent possible bugs
    private bool isEndHexagon() {
        activeCount = 0;
        for (int i = 0;i < 5;i++) {
            for (int j = 0;j < i + 1;j++) {
                if (!hexagonBalls[i][j].activeSelf)
                    continue;
                activeCount++;
                if (i + 2 < 5 && !hexagonTiles[i + 1][j].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i + 2][j].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
                if (j + 2 < 5 && i + 2 < 5 && !hexagonTiles[i + 1][j + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i + 2][j + 2].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
                if (i - 2 > -1 && j <= i - 2 && !hexagonTiles[i - 1][j].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i - 2][j].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
                if (j - 2 > -1 && i - 2 > -1 && !hexagonTiles[i - 1][j - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i - 2][j - 2].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
                if (j - 2 > -1 && !hexagonTiles[i][j - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i][j - 2].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
                if (j + 2 <= i && !hexagonTiles[i][j + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i][j + 2].GetComponent<HexagonScript>().GetIsEmpty())
                    return false;
            }
        }

        Panel.SetActive(true);
        panelText.text = "Game Ended\n\n  Score:" + activeCount;

        if (activeCount == 1)
            winSound.Play();
        else
            loseSound.Play();
        playTurnButton.SetActive(false);
        return true;
    }


    // Works similiar with isEnd functions, finds possible moves and adds all as a list to moveList of 2 pairs which are balls and tiles current positions as int
    public void computeMoves() {
        moveList = new List<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>>();

        if (Maps.activeSelf) { 
            for (int i = 0;i < 9;i++) {
                for (int j = 0;j < 9;j++) {
                    if (Balls[j][i].activeSelf) {
                        KeyValuePair<int, int> ballPair = new KeyValuePair<int, int>(j, i);
                        if (i + 2 < 9 && !Tiles[j][i + 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[j][i + 2].GetComponent<TileScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(j, i + 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (i - 2 > -1 && !Tiles[j][i - 1].GetComponent<TileScript>().GetIsEmpty() && Tiles[j][i - 2].GetComponent<TileScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(j, i - 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j + 2 < 9 && !Tiles[j + 1][i].GetComponent<TileScript>().GetIsEmpty() && Tiles[j + 2][i].GetComponent<TileScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(j + 2, i);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j - 2 > -1 && !Tiles[j - 1][i].GetComponent<TileScript>().GetIsEmpty() && Tiles[j - 2][i].GetComponent<TileScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(j - 2, i);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                    }
                }
            }
        }
        else {
            for (int i = 0;i < 5;i++) {
                for (int j = 0;j < i + 1;j++) {
                    if (hexagonBalls[i][j].activeSelf) {
                        KeyValuePair<int, int> ballPair = new KeyValuePair<int, int>(i, j);
                        if (i + 2 < 5 && !hexagonTiles[i + 1][j].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i + 2][j].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i + 2, j);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j + 2 < 5 && i + 2 < 5 && !hexagonTiles[i + 1][j + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i + 2][j + 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i + 2, j + 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (i - 2 > -1 && j <= i - 2 && !hexagonTiles[i - 1][j].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i - 2][j].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i - 2, j);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j - 2 > -1 && i - 2 > -1 && !hexagonTiles[i - 1][j - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i - 2][j - 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i - 2, j - 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j - 2 > -1 && !hexagonTiles[i][j - 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i][j - 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i, j - 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                        if (j + 2 <= i && !hexagonTiles[i][j + 1].GetComponent<HexagonScript>().GetIsEmpty() && hexagonTiles[i][j + 2].GetComponent<HexagonScript>().GetIsEmpty()) {
                            KeyValuePair<int, int> tilePair = new KeyValuePair<int, int>(i, j + 2);
                            moveList.Add(new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(ballPair, tilePair));
                        }
                    }
                }
            }
        }

    }


    // From moveLists elements chooses random one of them and plays it according to map type and computes another move for next time
    public void playTurn() {
        int rand = Random.Range(0, moveList.Count);
        int ballX = moveList[rand].Key.Key;
        int ballZ = moveList[rand].Key.Value;
        int tileX = moveList[rand].Value.Key;
        int tileZ = moveList[rand].Value.Value;

        ballSound.Play();
        if (Maps.activeSelf) { 
            Tiles[(tileX + ballX) / 2][(tileZ + ballZ) / 2].GetComponent<TileScript>().setIsEmpty(true);
            Balls[(tileX + ballX) / 2][(tileZ + ballZ) / 2].SetActive(false);
            Balls[tileX][tileZ].SetActive(true);
            Tiles[tileX][tileZ].GetComponent<TileScript>().setIsEmpty(false);
            Tiles[ballX][ballZ].GetComponent<TileScript>().setIsEmpty(true);
            Balls[ballX][ballZ].SetActive(false);
            isEnd();
        }
        else {
            hexagonTiles[(tileX + ballX) / 2][(tileZ + ballZ) / 2].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[(tileX + ballX) / 2][(tileZ + ballZ) / 2].SetActive(false);
            hexagonBalls[tileX][tileZ].SetActive(true);
            hexagonTiles[tileX][tileZ].GetComponent<HexagonScript>().setIsEmpty(false);
            hexagonTiles[ballX][ballZ].GetComponent<HexagonScript>().setIsEmpty(true);
            hexagonBalls[ballX][ballZ].SetActive(false);
            isEndHexagon();
        }

        computeMoves();
    }

    public void openOrClosePanel(bool openChoice) {
        if (openChoice) {
            SaveNameInputField.text = "";
            SaveNameInput.SetActive(true);
            SaveButton.SetActive(false);
        }
        else {
            SaveButton.SetActive(true);
            SaveNameInput.SetActive(false);
        }
    }



    // Returns to main menu
    public void backToMenu() {
        SceneManager.LoadScene("Entrance");
    }

    IEnumerator GameSaved() {
        GameSavedPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        GameSavedPanel.SetActive(false);
    }


    // Saves games level type, AI type and balls positions to "deneme.save" file for loading it another time
    public void saveGame() {
        if (SaveNameText.text.Length > 0) {
            if (HexagonMap.activeSelf)
                SaveLoadManager.SaveGame(SaveNameText.text, new saveData(EnctranceManagerScript.instance.levelChoice, EnctranceManagerScript.instance.AIChoice, hexagonBalls));
            else
                SaveLoadManager.SaveGame(SaveNameText.text, new saveData(EnctranceManagerScript.instance.levelChoice, EnctranceManagerScript.instance.AIChoice, Balls));
            StartCoroutine(GameSaved());
            openOrClosePanel(false);
        }
    }
}
