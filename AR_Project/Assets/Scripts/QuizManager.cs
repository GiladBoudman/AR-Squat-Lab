using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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

    // CALL THIS FROM THE "START QUIZ" BUTTON
    public void StartQuiz()
    {
        // 1. Find the ball in the scene
        physicsController = FindFirstObjectByType<SquatPhysicsController>();

        if (physicsController == null)
        {
            Feedback("Error: Spawn the ball first!");
            return;
        }

        // 2. Show UI
        quizPanel.SetActive(true);

        // 3. Load Data
        SetupDefaultQuestions();
        ShowQuestion(0);
    }

    void Update()
    {
        if (quizPanel.activeSelf && physicsController != null)
        {
            // Only check physics if we are on a Challenge Question
            if (questions.Count > currentQuestionIndex &&
                questions[currentQuestionIndex].type == QuestionType.PhysicalChallenge)
            {
                CheckPhysicalChallenge();
            }
        }
    }

    void ShowQuestion(int index)
    {
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
            buttonContainer.SetActive(false); // Hide buttons for physical tasks
            feedbackText.text = "Perform the action with the ball...";
        }
    }

    public void OnAnswerClicked(int index)
    {
        if (index == questions[currentQuestionIndex].correctOptionIndex)
        {
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
        // Get height from the ball script
        float currentMaxHeight = physicsController.GetMaxHeight();

        // Only check if ball has landed (velocity near 0)
        Rigidbody rb = physicsController.GetComponent<Rigidbody>();
        if (rb.useGravity && Mathf.Abs(rb.linearVelocity.y) < 0.1f && currentMaxHeight > 0.05f)
        {
            bool success = false;
            Question q = questions[currentQuestionIndex];

            switch (q.challengeType)
            {
                case ChallengeType.JumpLow: // Max < 0.5m
                    if (currentMaxHeight < q.targetValueMax) success = true;
                    break;
                case ChallengeType.JumpHigh: // Max > 1.0m
                    if (currentMaxHeight > q.targetValueMin) success = true;
                    break;
                case ChallengeType.HitTargetRange: // 1.5m < Max < 2.0m
                    if (currentMaxHeight >= q.targetValueMin && currentMaxHeight <= q.targetValueMax) success = true;
                    break;
            }

            if (success)
            {
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

        // Q1: Concept
        questions.Add(new Question
        {
            text = "As the ball travels UP, what happens to its energy?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "KE turns into Potential", "PE turns into Kinetic", "Energy is lost" },
            correctOptionIndex = 0
        });

        // Q2: Action (Low Jump)
        // Challenge: Tap the button quickly. Don't let it charge much.
        questions.Add(new Question
        {
            text = "ACTION: Perform a 'Baby Jump'. Keep Max Height BELOW 0.30 meters.",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.JumpLow,
            targetValueMax = 0.30f
        });

        // Q3: Concept
        questions.Add(new Question
        {
            text = "At the exact top of the jump (Max Height), what is the velocity?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "Maximum", "0 m/s", "9.81 m/s" },
            correctOptionIndex = 1
        });

        // Q4: Action (High Jump)
        // Challenge: Hold the charge almost to the max! (Max is ~0.87m)
        questions.Add(new Question
        {
            text = "ACTION: Generate High Energy! Jump HIGHER than 0.70 meters.",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.JumpHigh,
            targetValueMin = 0.70f
        });

        // Q5: Concept
        questions.Add(new Question
        {
            text = "Which variable does NOT affect the Potential Energy (PE = mgh)?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "Mass", "Height", "Velocity" },
            correctOptionIndex = 2
        });

        // Q6: Action (Precision Target)
        // Challenge: A medium charge. Not too low, not too high.
        questions.Add(new Question
        {
            text = "ACTION: Precision Test! Land exactly between 0.40m and 0.60m.",
            type = QuestionType.PhysicalChallenge,
            challengeType = ChallengeType.HitTargetRange,
            targetValueMin = 0.40f,
            targetValueMax = 0.60f
        });

        // Q7: Concept (Final)
        questions.Add(new Question
        {
            text = "If you double the Jump Height, what happens to the Potential Energy?",
            type = QuestionType.MultipleChoice,
            options = new string[] { "It stays the same", "It Doubles", "It Quadruples" },
            correctOptionIndex = 1 // Linear relationship
        });
    }
}