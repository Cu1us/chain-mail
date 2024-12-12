using DG.Tweening;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject[] tutorialSteps;

    [Header("Reference")]
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] SwingableObject player1;
    [SerializeField] SwingableObject rock;


    bool currentStepComplete;
    int currentStep;

    void Start()
    {
        PlayTextAnimation();
    }

    void Update()
    {
        if (currentStep < tutorialSteps.Length)
        {
            CheckIfStepCompleted();

            if (currentStepComplete)
            {
                currentStep++;
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
                tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-600, 2).SetEase(Ease.InOutQuad));
        }
        else
        {
            tutorialSteps[currentStep].GetComponent<RectTransform>().DOAnchorPosX(-600, 2).SetEase(Ease.InOutQuad);
        }
    }

    void CheckIfStepCompleted()
    {
        switch (currentStep)
        {
            case 0:
                if (Mathf.Abs(player1.swingVelocity) > 0 || Mathf.Abs(rock.swingVelocity) > 0)
                {
                    currentStepComplete = true;
                    Debug.Log("Test");
                }
                break;
            case 1:

                break;
        }
    }
}
