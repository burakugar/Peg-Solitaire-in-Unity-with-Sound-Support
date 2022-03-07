using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // For Serializing class as binary
public class saveData {
    
    public static saveData instance;
    public bool AIChoice;
    public int choice;
    public bool[][] Balls;

    // Takes balls as GameObjects and stores true or false according to
    // their activeness in game scene and use it as indicator for loading
    public saveData(int levelChoice, bool isHuman, GameObject[][] allBalls) {
        AIChoice = isHuman;
        choice = levelChoice;
        if (choice != 6) {
            Balls = new bool[9][];
            for (int i = 0;i < 9;i++) {
                Balls[i] = new bool[9];
                for (int j = 0;j < 9;j++) {
                    Balls[i][j] = allBalls[i][j].activeSelf;
                }
            }
        }
        else {
            Balls = new bool[5][];
            for (int i = 0;i < 5;i++) {
                Balls[i] = new bool[i + 1];
                for (int j = 0;j < i + 1;j++) {
                    Balls[i][j] = allBalls[i][j].activeSelf;
                }
            }
        }

    }
    
}
