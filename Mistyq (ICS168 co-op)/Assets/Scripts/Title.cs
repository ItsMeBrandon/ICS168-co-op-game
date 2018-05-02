using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Title : MonoBehaviour {

	public Sprite title;
	public Sprite noTitle;
	private Image im;


	// Use this for initialization
	void Start () {
	im = GetComponent<Image>();
	
	im.sprite = title;
	}


	public void Appear()
	{ im.sprite = title; }
	
	public void Disappear()
	{ im.sprite = noTitle; }

	
	// Update is called once per frame
	void Update () {
		
	}
}
