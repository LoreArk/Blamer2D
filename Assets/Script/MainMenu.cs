using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    AudioSource buttonAudio;
    public AudioSource soundtrackAudio;
    Animator animator;
    EventSystem eventSystem;
    GameObject lastSelected;

    private void Awake()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        animator = GetComponent<Animator>();
        buttonAudio = GetComponentInChildren<AudioSource>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(lastSelected != eventSystem.currentSelectedGameObject && lastSelected != null)
        {
            AudioManager.instance.PlayButtonSelectionSound(buttonAudio);
        }

        lastSelected = eventSystem.currentSelectedGameObject;
    }


    public void StartGame()
    {
        soundtrackAudio.volume = 0;
        AudioManager.instance.PlayStartButtonSound(buttonAudio);
        animator.Play("fadeout");
        StartCoroutine(WaitBeforeStart());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitBeforeStart()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Level_Prototype");
    }

    public void SetSelectedButton(GameObject gameObject)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
