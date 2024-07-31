using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _tutPages;
    private int tutPage;

    private void Start()
    {
        int seenTutorial = PlayerPrefs.GetInt("TutorialSeen", 0);

        if(seenTutorial == 0)
            GameGrid.Instance.SetInTutorial(true);
        else
            gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tutPage++;

            if (tutPage >= _tutPages.Count)
                StartCoroutine(CloseTutorial());
            else
                GoToNextPage();
        }
    }

    private void GoToNextPage()
    {
        foreach (GameObject page in _tutPages)
        {
            page.SetActive(false);
        }

        _tutPages[tutPage].SetActive(true);
    }

    private IEnumerator CloseTutorial()
    {
        yield return null; // Wait a frame so space wont close tut and swap
        gameObject.SetActive(false);
        GameGrid.Instance.SetInTutorial(false);
        PlayerPrefs.SetInt("TutorialSeen", 1);
    }

    public void OpenFromStart()
    {
        gameObject.SetActive(true);
        GameGrid.Instance.SetInTutorial(true);
        tutPage = 0;

        foreach (GameObject page in _tutPages)
        {
            page.SetActive(false);
        }

        _tutPages[tutPage].SetActive(true);
    }
}
