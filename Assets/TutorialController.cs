using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject[] tutorialStepsKeyboard;
    [SerializeField] GameObject[] tutorialStepsController;

    [Header("Reference")]
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] SwingableObject player1;
    [SerializeField] SwingableObject rock;
    [SerializeField] Chain chain;
    [SerializeField] DoorNextLevel doorNextLevel;

    GameObject[] tutorialSteps = new GameObject[5];
    bool animationComplete;
    bool currentStepComplete;
    int currentStep = 0;

    // Step 1
    bool jButton;
    bool lButton;

    // Step 2
    bool player1ChangedAnchor;
    bool rockChangedAnchor;

    // Step 3
    bool swapped;

    // Step 4
    bool iButton;
    bool kButton;
    void Start()
    {
        if (PlayerInputData.inputType == PlayerInputData.InputType.Keyboard || PlayerInputData.inputType == null)
        {
            tutorialSteps = tutorialStepsKeyboard;
        }
        else
        {
            tutorialSteps = tutorialStepsController;
        }
        PlayTextAnimation();
        playerInputData.onDeviceChange += UpdateControllerInput;
    }

    void Update()
    {
        UpdateCurrentStep();
    }

    void UpdateControllerInput()
    {
        if (PlayerInputData.inputType == PlayerInputData.InputType.Keyboard)
        {
            if (currentStep >= 0)
            {
                tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-2100, 2).SetEase(Ease.InOutQuad).OnComplete(() =>
                ResetTutorial());
            }
            tutorialSteps = tutorialStepsKeyboard;
        }
        else if(PlayerInputData.inputType == PlayerInputData.InputType.Gamepad)
        {
            if (currentStep >= 0)
            {
                tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-2100, 2).SetEase(Ease.InOutQuad).OnComplete(() =>
                ResetTutorial());
            }
            tutorialSteps = tutorialStepsController;
        }
    }

    void ResetTutorial()
    {
        currentStep = 0;
        jButton = false;
        lButton = false;
        player1ChangedAnchor = false;
        rockChangedAnchor = false;
        swapped = false;
        iButton = false;
        kButton = false;
        PlayTextAnimation();
    }

    void UpdateCurrentStep()
    {
        if (currentStep < tutorialSteps.Length)
        {
            CheckIfStepCompleted();

            if (currentStepComplete && currentStep != tutorialSteps.Length - 1)
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
                tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-900, 2).SetEase(Ease.InOutQuad).OnComplete(() => animationComplete = true));
        }
        else
        {
            tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-900, 2).SetEase(Ease.InOutQuad).OnComplete(() => animationComplete = true);
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
            case 2:
                CheckSwapInput();
                break;
            case 3:
                CheckChainLenght();
                break;
            case 4:
                currentStepComplete = true;
                doorNextLevel.OpenNextLevel();
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
                player1ChangedAnchor = true;
            }
            if (chain.anchorStatus == Chain.AnchorStatus.ROCK && Mathf.Abs(player1.swingVelocity) > 4)
            {
                rockChangedAnchor = true;
            }
            if (player1ChangedAnchor && rockChangedAnchor)
            {
                Transform Button = tutorialSteps[currentStep].transform.GetChild(1);
                Button.gameObject.GetComponent<Image>().color = Color.green;
                currentStepComplete = true;
            }
        }
    }

    void CheckSwapInput()
    {
        if (animationComplete)
        {
            if (Time.time - player1.lastSwapTime < 1 || Time.time - rock.lastSwapTime < 1)
            {
                swapped = true;
            }
            if (swapped)
            {
                Transform Button = tutorialSteps[currentStep].transform.GetChild(1);
                Button.gameObject.GetComponent<Image>().color = Color.green;
                currentStepComplete = true;
            }
        }
    }

    void CheckChainLenght()
    {
        if (animationComplete)
        {
            if (playerInputData.chainExtendInput > 0)
            {
                iButton = true;
                Transform IButton = tutorialSteps[currentStep].transform.GetChild(1);
                IButton.gameObject.GetComponent<Image>().color = Color.green;

            }
            if (playerInputData.chainExtendInput < 0)
            {
                kButton = true;
                Transform KButton = tutorialSteps[currentStep].transform.GetChild(3);
                KButton.gameObject.GetComponent<Image>().color = Color.green;
            }
            if (iButton && kButton)
            {
                currentStepComplete = true;
            }
        }
    }
}
