using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordleGame : MonoBehaviour
{
    public int maxAttempts = 6;
    public int wordLength = 5;
    public string[] wordList;
    public GameObject letterSlotPrefab; // Préfabriqué pour les cases de lettres
    public Transform gridParent; // Parent de la grille dans la hiérarchie Unity
    public Button[] keyboardButtons; // Boutons pour le clavier virtuel
    public TMP_Text attemptsText;

    private string targetWord;
    private int currentAttempt = 0;
    private List<List<GameObject>> letterSlots = new List<List<GameObject>>(); // Pour stocker les cases de la grille
    public string currentGuess = "";

    void Start()
    {
        InitializeGame();
        CreateGrid();
    }

    void InitializeGame()
    {
        targetWord = GetRandomWord().ToUpper();
        attemptsText.text = "Tentatives restantes : " + (maxAttempts - currentAttempt);
    }

    void CreateGrid()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int j = 0; j < wordLength; j++)
            {
                GameObject slot = Instantiate(letterSlotPrefab, gridParent);
                row.Add(slot);
            }
            letterSlots.Add(row);
        }
    }

    string GetRandomWord()
    {
        return wordList[Random.Range(0, wordList.Length)];
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    string keyPressed = keyCode.ToString().ToUpper();

                    // Vérifie si c'est une lettre entre A et Z
                    if (keyPressed.Length == 1 && keyPressed[0] >= 'A' && keyPressed[0] <= 'Z')
                    {
                        OnKeyboardButtonPressed(keyPressed);
                    }
                    else if (keyCode == KeyCode.Return) // Touche Entrée
                    {
                        OnEnterButtonPressed();
                    }
                    else if (keyCode == KeyCode.Backspace) // Touche Retour Arrière
                    {
                        OnBackspaceButtonPressed();
                    }
                }
            }
        }
    }

    public void OnKeyboardButtonPressed(string letter)
    {
        if (currentGuess.Length < wordLength)
        {
            currentGuess += letter;
            UpdateGridDisplay();
        }
    }

    public void OnEnterButtonPressed()
    {
        if (currentGuess.Length == wordLength)
        {
            CheckGuess();
            currentGuess = "";
            currentAttempt++;
            attemptsText.text = "Tentatives restantes : " + (maxAttempts - currentAttempt);

            if (currentAttempt >= maxAttempts)
            {
                attemptsText.text = "Jeu terminé ! Le mot était : " + targetWord;
            }
        }
    }

    void UpdateGridDisplay()
    {
        // Clear all slots in the current attempt row
        for (int i = 0; i < wordLength; i++)
        {
            letterSlots[currentAttempt][i].GetComponentInChildren<Text>().text = "";
        }

        // Update slots with the current guess
        for (int i = 0; i < currentGuess.Length; i++)
        {
            letterSlots[currentAttempt][i].GetComponentInChildren<Text>().text = currentGuess[i].ToString();
        }
    }

    void CheckGuess()
    {
        for (int i = 0; i < wordLength; i++)
        {
            char letter = currentGuess[i];
            GameObject slot = letterSlots[currentAttempt][i];
            Image slotImage = slot.GetComponent<Image>();

            if (letter == targetWord[i])
            {
                slotImage.color = Color.green;
            }
            else if (targetWord.Contains(letter.ToString()))
            {
                slotImage.color = Color.yellow;
            }
            else
            {
                slotImage.color = Color.gray;
            }
        }
    }

    public void OnBackspaceButtonPressed()
    {
        if (currentGuess.Length > 0)
        {
            currentGuess = currentGuess.Substring(0, currentGuess.Length - 1);
            UpdateGridDisplay();
        }
    }
}
