using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class EnctranceManagerScript : MonoBehaviour {

    // So every class can use this class as static without creating object
    public static EnctranceManagerScript instance;

    // Tiles indicates selected maps orientation, AIChoice is true if human is playing else computer is playing
    public bool[][] Tiles;
    public int levelChoice;
    public bool AIChoice;

    // Load Manager variables
    public bool[][] loadedBalls;
    public bool isLoaded;

    // Indicates one empty space for every maps orientation
    public KeyValuePair<int, int> EmptySpace;

    // Level Selecting Sounds
    private AudioSource LevelSelectSound;

    // For changing/activating/deactivating Canvases after pushing buttons
    [SerializeField] private GameObject LevelSelectCanvas;
    [SerializeField] private Text LoadNameText;
    [SerializeField] private GameObject AIChoiceCanvas;
    [SerializeField] private GameObject LoadNameInput;
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameObject EndButton;
    [SerializeField] private GameObject HumanButton;
    [SerializeField] private GameObject ComputerButton;
    [SerializeField] private GameObject WrongInputPanel;
    [SerializeField] private InputField LoadNameInputField;
    [SerializeField] private GameObject SaveListPanel;
    [SerializeField] private Text SaveListText;


 /* public static void SeriouslyDeleteAllSaveFiles()
     {
         string path = Application.persistentDataPath + "/saves/";
         DirectoryInfo directory = new DirectoryInfo(path);
         directory.Delete(true);
         Directory.CreateDirectory(path);
     } 
*/
    void Start()
    {
        listSaves();
     // SeriouslyDeleteAllSaveFiles(); 

        LevelSelectSound = GetComponent<AudioSource>();
        isLoaded = false;
        Tiles = new bool[9][];
        for (int i = 0;i < 9;i++)
            Tiles[i] = new bool[9];

        for (int i = 0;i < 9;i++)
            for (int j = 0;j < 9;j++)
                Tiles[i][j] = true;
    }


    public void SelectMap(int choice) {
        levelChoice = choice;
        LevelSelectSound.Play();
        switch (choice) {
            case 1:
                for (int i = 0;i < 9;i++)
                    Tiles[0][i] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[8][i] = false;
                for(int i = 0;i < 9;i++)
                    Tiles[i][0] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[i][8] = false;
                Tiles[1][1] = false;
                Tiles[1][2] = false;
                Tiles[1][6] = false;
                Tiles[1][7] = false;
                Tiles[7][1] = false;
                Tiles[7][2] = false;
                Tiles[7][6] = false;
                Tiles[7][7] = false;
                Tiles[2][1] = false;
                Tiles[2][7] = false;
                Tiles[6][1] = false;
                Tiles[6][7] = false;
                
                EmptySpace = new KeyValuePair<int, int>(5, 4);
                break;
            case 2:
                for (int i = 0;i < 3;i++)
                    for (int j = 0;j < 3;j++)
                        Tiles[j][i] = false;
                for (int i = 0;i < 3;i++)
                    for (int j = 6;j < 9;j++)
                        Tiles[j][i] = false;
                for (int i = 6;i < 9;i++)
                    for (int j = 0;j < 3;j++)
                        Tiles[j][i] = false;
                for (int i = 6;i < 9;i++)
                    for (int j = 6;j < 9;j++)
                        Tiles[j][i] = false;
                EmptySpace = new KeyValuePair<int, int>(4, 4);
                break;

            case 3:
                for (int i = 0;i < 9;i++)
                    Tiles[i][8] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[8][i] = false;
                for (int i = 0;i < 2;i++)
                    for (int j = 0;j < 2;j++)
                        Tiles[j][i] = false;
                for (int i = 5;i < 8;i++)
                    for (int j = 0;j < 2;j++)
                        Tiles[j][i] = false;
                for (int i = 0;i < 2;i++)
                    for (int j = 5;j < 8;j++)
                        Tiles[j][i] = false;
                for (int i = 5;i < 8;i++)
                    for (int j = 5;j < 8;j++)
                        Tiles[j][i] = false;
                EmptySpace = new KeyValuePair<int, int>(3, 3);
                break;

            case 4:
                for (int i = 0;i < 9;i++)
                    Tiles[i][8] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[i][0] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[0][i] = false;
                for (int i = 0;i < 9;i++)
                    Tiles[8][i] = false;
                for (int i = 1;i < 3;i++)
                    for (int j = 1;j < 3;j++)
                        Tiles[j][i] = false;
                for (int i = 6;i < 8;i++)
                    for (int j = 1;j < 3;j++)
                        Tiles[j][i] = false;
                for (int i = 6;i < 8;i++)
                    for (int j = 6;j < 8;j++)
                        Tiles[j][i] = false;
                for (int i = 1;i < 3;i++)
                    for (int j = 6;j < 8;j++)
                        Tiles[j][i] = false;
                EmptySpace = new KeyValuePair<int, int>(4, 4);
                break;

            case 5:
                for (int i = 0;i < 4;i++)
                    for (int j = 8;j > 4 + i;j--)
                        Tiles[j][i] = false;

                for (int i = 8;i > 4;i--)
                    for (int j = 13 - i;j < 9;j++)
                        Tiles[i][j] = false;

                for (int i = 3;i > -1;i--)
                    for (int j = 0;j < 4 - i;j++)
                        Tiles[i][j] = false;

                for (int i = 0;i < 5;i++)
                    for (int j = i + 5;j < 9;j++)
                        Tiles[i][j] = false;
                EmptySpace = new KeyValuePair<int, int>(4, 4);
                break;
            case 6:
                break;
        }
        SceneManager.LoadScene("InGame");
    }


    public void openOrClosePanel(bool openChoice) {
        if (openChoice) {
            LoadNameInputField.text = "";
            List<string> tempList = listSaves();
            for (int i = 0;i < tempList.Count;i++)
                SaveListText.text += tempList[i];
            SaveListPanel.SetActive(true);
            LoadNameInput.SetActive(true);
            ComputerButton.SetActive(false);
            EndButton.SetActive(false);
            HumanButton.SetActive(false);
            LoadButton.SetActive(false);
        }
        else {
            SaveListText.text = "";
            LoadNameInput.SetActive(false);
            SaveListPanel.SetActive(false);
            ComputerButton.SetActive(true);
            EndButton.SetActive(true);
            HumanButton.SetActive(true);
            LoadButton.SetActive(true);
        }
    }

    public void LoadGame() {
        if (LoadNameText.text.Length <= 0) // Input isn't entered
            return;
        saveData data = SaveLoadManager.LoadGame(LoadNameText.text);
        if(data == null) {                 // There is no such save with such name 
            StartCoroutine(WrongInput());
            return;
        }
        AIChoice = data.AIChoice;
        levelChoice = data.choice;
        if (levelChoice != 6) {
            loadedBalls = new bool[9][];
            for (int i = 0;i < 9;i++) {
                loadedBalls[i] = new bool[9];
                for (int j = 0;j < 9;j++)
                    loadedBalls[i][j] = data.Balls[i][j];
            }
        }
        else {
            loadedBalls = new bool[5][];
            for (int i = 0;i < 5;i++) {
                loadedBalls[i] = new bool[i + 1];
                for (int j = 0;j < i + 1;j++) 
                    loadedBalls[i][j] = data.Balls[i][j];
            }
        }

        isLoaded = true;
        SelectMap(levelChoice);
    }

    IEnumerator WrongInput() {
        WrongInputPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        WrongInputPanel.SetActive(false);
    }

    public void EndGame() {
        Application.Quit();
    }

    public void SelectAI(bool isHuman) {
        LevelSelectSound.Play();
        AIChoice = isHuman;
        LevelSelectCanvas.SetActive(true);
        AIChoiceCanvas.SetActive(false);
    }

    public List<string> listSaves() {
        /*
        var info = new DirectoryInfo(Application.persistentDataPath + "/Saves/");
        var fileInfo = info.GetFiles();
        for (File file in fileInfo) print(file);*/
        List<string> saves = new List<string>();
        
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/Saves/");
        FileInfo[] info = dir.GetFiles("*.save");
        foreach (FileInfo f in info)
            saves.Add(f.Name.Replace(".save", "\n"));
        return saves;
    }

}
