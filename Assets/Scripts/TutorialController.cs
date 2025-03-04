using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] GameObject dummy;

    GameObject[] tutorialSteps = new GameObject[5];
    bool animationComplete;
    bool currentStepComplete;
    int currentStep = 0;

    // Step 1
    bool jButton;
    bool lButton;
    float rotationTimer1;
    float rotationTimer2;

    // Step 2
    bool player1ChangedAnchor;
    bool rockChangedAnchor;
    float switchedAnchorCounter;

    // Step 3
    bool rockBeingSwapped;
    float swappedCounter;

    // Step 4
    bool iButton;
    bool kButton;

    // Step 5
    GameObject newDummy = null;
    bool dummySpawned = false;

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
        else if (PlayerInputData.inputType == PlayerInputData.InputType.PS4 || PlayerInputData.inputType == PlayerInputData.InputType.Xbox)
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

        // Step 1
        jButton = false;
        lButton = false;
        rotationTimer1 = 0;
        rotationTimer2 = 0;

        // Step 2
        player1ChangedAnchor = false;
        rockChangedAnchor = false;
        switchedAnchorCounter = 0;

        // Step 3
        rockBeingSwapped = false;
        swappedCounter = 0;

        // Step 4
        iButton = false;
        kButton = false;

        // Step 5
        if (newDummy != null)
        {
            Destroy(newDummy);
        }
        newDummy = null;
        dummySpawned = false;

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
            case 2:
                CheckSwapInput();
                break;
            case 3:
                CheckChainLenght();
                break;
            case 4:
                CheckDummyKilled();
                break;
            case 5:
                currentStepComplete = true;
                doorNextLevel.OpenNextLevel();
                break;
        }
    }

    void CheckRotationInput()
    {
        if (animationComplete)
        {
            if (player1.swingVelocity > 5 || rock.swingVelocity > 5)
            {
                rotationTimer1 += Time.deltaTime;
            }
            if (rotationTimer1 > 0.8f)
            {
                jButton = true;
                Transform JButton = tutorialSteps[currentStep].transform.GetChild(1);
                JButton.gameObject.GetComponent<Image>().color = Color.green;
            }

            if (player1.swingVelocity < -5 || rock.swingVelocity < -5)
            {
                rotationTimer2 += Time.deltaTime;
            }
            if (rotationTimer2 > 0.8f)
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
            if (chain.anchorStatus == Chain.AnchorStatus.PLAYER && Mathf.Abs(rock.swingVelocity) > 4 && !player1ChangedAnchor)
            {
                player1ChangedAnchor = true;
                rockChangedAnchor = false;
                switchedAnchorCounter++;
                tutorialSteps[currentStep].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = (switchedAnchorCounter - 1).ToString();
            }
            if (chain.anchorStatus == Chain.AnchorStatus.ROCK && Mathf.Abs(player1.swingVelocity) > 4 && !rockChangedAnchor)
            {
                rockChangedAnchor = true;
                player1ChangedAnchor = false;
                switchedAnchorCounter++;
                tutorialSteps[currentStep].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = (switchedAnchorCounter - 1).ToString();
            }
            if (switchedAnchorCounter > 3)
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
            if (rock.beingSwapped && !rockBeingSwapped)
            {
                rockBeingSwapped = true;
                swappedCounter++;
                tutorialSteps[currentStep].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = (swappedCounter).ToString();
            }
            if (rock.beingSwapped == false)
            {
                rockBeingSwapped = false;
            }

            if (swappedCounter >= 3)
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

    void CheckDummyKilled()
    {
        if (animationComplete)
        {
            if (newDummy == null && dummySpawned == false)
            {
                newDummy = Instantiate(dummy, new Vector3(0, 10, 0), Quaternion.identity);
                dummySpawned = true;
            }
            else if (dummySpawned && newDummy == null)
            {
                currentStepComplete = true;
            }
        }
    }
}
