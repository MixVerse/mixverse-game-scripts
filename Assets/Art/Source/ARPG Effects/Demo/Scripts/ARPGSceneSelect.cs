using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ARPGFX
{

public class ARPGSceneSelect : MonoBehaviour
{
	public bool GUIHide = false;
	public bool GUIHide2 = false;
	
	public void LoadSceneDemo1() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo01");}
	public void LoadSceneDemo2() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo02");}
	public void LoadSceneDemo3() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo03");}
	public void LoadSceneDemo4() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo04");}
	public void LoadSceneDemo5() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo05");}
	public void LoadSceneDemo6() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo06");}
	public void LoadSceneDemo7() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo07");}
	public void LoadSceneDemo8() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo08");}
	public void LoadSceneDemo9() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo09");}
	public void LoadSceneDemo10() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo10");}
	public void LoadSceneDemo11() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo11");}
	public void LoadSceneDemo12() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo12");}
	public void LoadSceneDemo13() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo13");}
	public void LoadSceneDemo14() { UnityEngine.SceneManagement.SceneManager.LoadScene("ARPGDemo14");}

	void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("Canvas2").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas2").GetComponent<Canvas> ().enabled = true;
         }
     }
	      if(Input.GetKeyDown(KeyCode.K))
	 {
         GUIHide2 = !GUIHide2;
     
         if (GUIHide2)
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = true;
         }
     }
	 }
}
}