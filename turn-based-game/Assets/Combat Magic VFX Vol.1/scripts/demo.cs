using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour {
	GameObject[] effectObjects;
	int effectIndex;
	void Start () {
		int i;
		effectObjects = Resources.LoadAll<GameObject>("");
		i = 0;
		while (i < effectObjects.Length) {
			effectObjects[i] = Instantiate(effectObjects[i]);
			effectObjects[i].transform.position = new Vector3(0, 0, 0);
			effectObjects[i].SetActive(false);
			i++;
		}
		effectIndex = 0;
		showCurrent();
		playCurrent();
	}
	void playCurrent(){
		if (getSystem(effectIndex).isPlaying)
			getSystem(effectIndex).Stop();
		getSystem(effectIndex).Play();
	}
	void stopCurrent(){
		getSystem(effectIndex).Stop();
	}
	void showCurrent(){
		effectObjects[effectIndex].SetActive(true);
	}
	void hideCurrent(){
		effectObjects[effectIndex].SetActive(false);
	}
	void nextEffect(){
		hideCurrent();
		effectIndex++;
		if (effectIndex == effectObjects.Length)
			effectIndex = 0;
		showCurrent();
		playCurrent();
	}
	void previousEffect(){
		hideCurrent();
		effectIndex--;
		if (effectIndex < 0)
			effectIndex = effectObjects.Length - 1;
		showCurrent();
		playCurrent();
	}
	ParticleSystem getSystem(int i){
		return effectObjects[i].transform.GetChild(0).GetComponent<ParticleSystem>();
	}
	void Update () {
		playerInput();
	}
	void playerInput(){
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			nextEffect();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			previousEffect();
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			Camera.main.transform.Translate(5 * Time.deltaTime * Camera.main.transform.forward);
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			Camera.main.transform.Translate(-5 * Time.deltaTime * Camera.main.transform.forward);
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			playCurrent();
		}
	}
}
