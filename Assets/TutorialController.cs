using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject[] tutorialSteps;

    [Header("Reference")]
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] Chain chain;
    [SerializeField] SwingableObject player1;
    [SerializeField] SwingableObject rock;

    bool animationComplete;
    bool currentStepComplete;
    int currentStep;

    // Step 1
    bool jButton;
    bool lButton;

    // Step 2
    bool player1Swapped;
    bool rockSwapped;
    void Start()
    {
        PlayTextAnimation();
    }

    void Update()
    {
        UpdateCurrentStep();
    }

    void UpdateCurrentStep()
    {
        if (currentStep < tutorialSteps.Length)
        {
            CheckIfStepCompleted();

            if (currentStepComplete)
            {
                currentStep++;
                animationComplete = false;
                currentStepComplete = false;
                PlayTextAnimation();
            }
        }
    }
    void PlayTextAnimation()
    {
        if (currentStep != 0)
        {
            tutorialSteps[currentStep - 1].GetComponent<RectTransform>().DOAnchorPosX(-2100, 2).SetEase(Ease.InOutQuad).OnComplete(() => 
                tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-800, 2).SetEase(Ease.InOutQuad).OnComplete(() => animationComplete = true));
        }
        else
        {
            tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-800, 2).SetEase(Ease.InOutQuad).OnComplete(() => animationComplete = true);
        }
    }

    void CheckIfStepCompleted()
    {
        switch (currentStep)
        {
            case 0:
                CheckRotationInput();
                break;
            case 1:
                CheckAnchorSwitch();
                break;
        }
    }

    void CheckRotationInput()
    {
        if (animationComplete)
        {
            if (player1.swingVelocity > 4 || rock.swingVelocity > 4)
            {
                jButton = true;
                Transform JButton = tutorialSteps[currentStep].transform.GetChild(1);
                JButton.gameObject.GetComponent<Image>().color = Color.green;

            }
            if (player1.swingVelocity < -4 || rock.swingVelocity < -4)
            {
                lButton = true;
                Transform LButton = tutorialSteps[currentStep].transform.GetChild(3);
                LButton.gameObject.GetComponent<Image>().color = Color.green;
            }
            if (jButton && lButton)
            {
                currentStepComplete = true;
            }
        }
    }

    void CheckAnchorSwitch()
    {
        if (animationComplete)
        {
            if (chain.anchorStatus == Chain.AnchorStatus.PLAYER && Mathf.Abs(rock.swingVelocity) > 4)
            {
                player1Swapped = true;
            }
            if (chain.anchorStatus == Chain.AnchorStatus.ROCK && Mathf.Abs(player1.swingVelocity) > 4)
            {
                rockSwapped = true;
            }
            if (player1Swapped && rockSwapped)
            {
                Transform Button = tutorialSteps[currentStep].transform.GetChild(1);
                Button.gameObject.GetComponent<Image>().color = Color.green;
                currentStepComplete = true;
            }
        }
    }
}
