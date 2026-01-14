using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This component manages a quiz system that combines
/// multiple-choice questions and physical challenges using the SquatPhysicsController.
/// </summary>
public class QuizManager : MonoBehaviour
{
    [Header("References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;

    [Header("Buttons")]
    public GameObject buttonContainer;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;

    private SquatPhysicsController physicsController;
    private int currentQuestionIndex = 0;

    private bool isAnswering = false; // Locks the game while waiting for the next question

    [System.Serializable]
    public class Question
    {
        public string text;
        public QuestionType type;
        public string[] options;
        public int correctOptionIndex;
        public ChallengeType challengeType;
        public float targetValueMin;
        public float targetValueMax;
    }

    public enum QuestionType { MultipleChoice, PhysicalChallenge }
    public enum ChallengeType { None, JumpLow, JumpHigh, HitTargetRange }

    private List<Question> questions = new List<Question>();

    public void StartQuiz()
    {
        physicsController = FindFirstObjectByType<SquatPhysicsController>();

        if (physicsController == null)
        {
            Feedback("Error: Spawn the ball first!");
            return;
        }

        quizPanel.SetActive(true);
        SetupDefaultQuestions();

        // Reset everything
        isAnswering = false;
        ShowQuestion(0);
    }

    void Update()
    {
        // If we are already transitioning to the next question dont check physics
        if (isAnswering) return;

        if (quizPanel.activeSelf && physicsController != null)
        {
            if (questions.Count > currentQuestionIndex &&
                questions[currentQuestionIndex].type == QuestionType.PhysicalChallenge)
            {
                CheckPhysicalChallenge();
            }
        }
    }

    void ShowQuestion(int index)
    {
        // Answering lock
        isAnswering = false;

        currentQuestionIndex = index;
        if (index >= questions.Count)
        {
            FinishQuiz();
            return;
        }

        Question q = questions[index];
        questionText.text = $"Q{index + 1}: {q.text}";
        feedbackText.text = "";

        if (q.type == QuestionType.MultipleChoice)
        {
            buttonContainer.SetActive(true);
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = q.options[i];
                int btnIndex = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(btnIndex));
            }
        }
        else
        {
            buttonContainer.SetActive(false);
            feedbackText.text = "Perform the action with the ball...";
        }
    }

    public void OnAnswerClicked(int index)
    {
        // Stop double clicks or phantom clicks
        if (isAnswering) return;

        if (index == questions[currentQuestionIndex].correctOptionIndex)
        {
            // Lock the game
            isAnswering = true;

            Feedback("Correct!");
            Invoke("NextQuestion", 1.5f);
        }
        else
        {
            Feedback("Wrong, try again.");
        }
    }

    void CheckPhysicalChallenge()
    {
        // Stop checking if we already succeeded
        if (isAnswering) return;

        float currentMaxHeight = physicsController.GetMaxHeight();
        Rigidbody rb = physicsController.GetComponent<Rigidbody>();

        // Check if landed
        if (rb.useGravity && Mathf.Abs(rb.linearVelocity.y) < 0.1f && currentMaxHeight > 0.05f)
        {
            bool success = false;
            Question q = questions[currentQuestionIndex];

            switch (q.challengeType)
            {
                case ChallengeType.JumpLow:
                    if (currentMaxHeight < q.targetValueMax) success = true;
                    break;
                case ChallengeType.JumpHigh:
                    if (currentMaxHeight > q.targetValueMin) success = true;
                    break;
                case ChallengeType.HitTargetRange:
                    if (currentMaxHeight >= q.targetValueMin && currentMaxHeight <= q.targetValueMax) success = true;
                    break;
            }

            if (success)
            {
                // Lock the game
                isAnswering = true;

                Feedback("Great Jump!");
                physicsController.ResetMarker();
                Invoke("NextQuestion", 2.0f);
            }
        }
    }

    void NextQuestion() => ShowQuestion(currentQuestionIndex + 1);
    void Feedback(string msg) => feedbackText.text = msg;

    void FinishQuiz()
    {
        questionText.text = "Quiz Complete!";
        feedbackText.text = "You are a Physics Master.";
        buttonContainer.SetActive(false);
        Invoke("HidePanel", 3f);
    }

    void HidePanel() => quizPanel.SetActive(false);

    void SetupDefaultQuestions()
    {
        questions.Clear();

        questions.Add(new Question
        {
            text = "As the ball flies <color=yellow><b>UP</b></color>, what happens to its energy?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "KE turns into PE", "PE turns into KE", "Energy is lost" },
            correctOptionIndex = 0
        });

        questions.Add(new Question
        {
            text = "MISSION: Tiny Hop! Jump <color=#FF00FF>LOWER</color> than 0.30 meters.",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.JumpLow,
            targetValueMax = 0.30f,
            targetValueMin = 0.05f
        });

        questions.Add(new Question
        {
            text = "At the <color=orange>MAXIMUM</color> height of the jump (the very top), what is the speed of the ball?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "Maximum", "0 m/s", "9.81 m/s" },
            correctOptionIndex = 1
        });

        questions.Add(new Question
        {
            text = "MISSION: Super Jump! Blast <color=red>Higher</color> than 0.70 meters!",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.JumpHigh,
            targetValueMin = 0.70f
        });

        questions.Add(new Question
        {
            text = "Which variable does <color=#00FFFF>NOT</color> affect the Potential Energy (PE = m · g · h)?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "Mass", "Height", "Velocity" },
            correctOptionIndex = 2
        });

        questions.Add(new Question
        {
            text = "MISSION: Bullseye! Jump <color=#E73E9F>BETWEEN</color> 0.40m and 0.60m.",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.HitTargetRange,
            targetValueMin = 0.40f,
            targetValueMax = 0.60f
        });

        questions.Add(new Question
        {
            text = "If you jump <color=#8470FF>TWICE</color> as high, what happens to your Potential Energy?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "It stays the same", "It Doubles", "It Quadruples" },
            correctOptionIndex = 1
        });
    }
}