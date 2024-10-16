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
    public List<List<GameObject>> letterSlots = new List<List<GameObject>>(); // Pour stocker les cases de la grille
    public string currentGuess = "";

    private bool gameEnded = false;

    void Start()
    {
        InitializeGame();
        InitializeKeyboard();
        CreateGrid();
    }

    void InitializeGame()
    {
        gameEnded = false;
        targetWord = GetRandomWord().ToUpper();
        currentGuess = "";
        currentAttempt = 0;
        wordLength = targetWord.Length;
        attemptsText.text = "Tentatives restantes : " + (maxAttempts - currentAttempt);
    }
    void InitializeKeyboard()
    {
        foreach (Button button in keyboardButtons)
        {
            string letter = button.GetComponentInChildren<TMP_Text>().text;
            button.onClick.AddListener(() => OnKeyboardButtonPressed(letter));
        }
    }
    public void RestartGame()
    {
        // Deselect any selected game object to prevent keyboard input on the restart button
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        InitializeGame();
        //Destroy current grid
        foreach (List<GameObject> row in letterSlots)
        {
            foreach (GameObject slot in row)
            {
                Destroy(slot);
            }
        }
        letterSlots.Clear();
        CreateGrid();
        UpdateGridDisplay();
    }

    void CreateGrid()
    {

        //change constraints of the grid layout group
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = wordLength;
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
        if (Input.anyKeyDown && !gameEnded)
        {
            string inputString = Input.inputString.ToUpper();

            foreach (char c in inputString)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    OnKeyboardButtonPressed(c.ToString());
                }
                else if (c == '\n' || c == '\r') // Touche Entrée
                {
                    OnEnterButtonPressed();
                }
                else if (c == '\b') // Touche Retour Arrière
                {
                    OnBackspaceButtonPressed();
                }
            }
        }
    }

    public void OnKeyboardButtonPressed(string letter)
    {
        if (currentGuess.Length < wordLength && !gameEnded)
        {
            currentGuess += letter;
            UpdateGridDisplay();
        }
    }

    public void OnEnterButtonPressed()
    {
        if (currentGuess.Length == wordLength && !gameEnded)
        {
            CheckGuess();
            if (gameEnded)
            {
                return;
            }
            currentGuess = "";
            currentAttempt++;
            attemptsText.text = "Tentatives restantes : " + (maxAttempts - currentAttempt);

            if (currentAttempt >= maxAttempts)
            {
                attemptsText.text = "Jeu terminé ! Le mot était : " + targetWord;
                gameEnded = true;
            }
        }
    }

    void UpdateGridDisplay()
    {
        // Ensure currentAttempt is within bounds
        if (currentAttempt < letterSlots.Count)
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
        if (currentGuess == targetWord)
        {
            attemptsText.text = "Bravo ! Vous avez trouvé le mot : " + targetWord;
            gameEnded = true;
        }
    }

    public void OnBackspaceButtonPressed()
    {
        if (currentGuess.Length > 0 && !gameEnded)
        {
            currentGuess = currentGuess.Substring(0, currentGuess.Length - 1);
            UpdateGridDisplay();
        }
    }
}
