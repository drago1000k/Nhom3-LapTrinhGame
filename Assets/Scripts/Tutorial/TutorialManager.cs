using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Text tutorialText;
    public float stepDuration = 5f;

    void Start()
    {
        // Bắt đầu tutorial khi vào main game
        StartCoroutine(ShowTutorialSteps());
    }

    IEnumerator ShowTutorialSteps()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "WASD For Movement, E For Special Skill";
            yield return new WaitForSeconds(stepDuration);

            tutorialText.text = "Kill Enemies To Level Up And Choose Upgrade";
            yield return new WaitForSeconds(stepDuration);

            tutorialText.text = "Try To Survive Until Wave 20";
            yield return new WaitForSeconds(stepDuration);

            tutorialText.gameObject.SetActive(false);
        }
    }
}