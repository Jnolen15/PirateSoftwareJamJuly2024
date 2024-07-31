using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
        GameGrid.Instance.SetInTutorial(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(CloseTutorial());
    }

    private IEnumerator CloseTutorial()
    {
        yield return null; // Wait a frame so space wont close tut and swap
        gameObject.SetActive(false);
        GameGrid.Instance.SetInTutorial(false);
    }
}
