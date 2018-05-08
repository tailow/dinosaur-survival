using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour {

    AudioSource menuMusic;
    public GameObject generationCanvas;
    float t;

    void Start()
    {
        menuMusic = GameObject.Find("Music").GetComponent<AudioSource>();
    }

    void Update()
    {
        menuMusic.pitch = Mathf.Lerp(0f, 2f, t += Time.deltaTime * 0.05f);
    }

    public void StartGame()
	{
		generationCanvas.SetActive(true);
        GameObject.Find("MenuCanvas").SetActive(false);

		SceneManager.LoadScene("scene_main");
	}
}
