using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickProfileButton()
    {
        SceneManager.LoadScene("Scenes/Game Design/Screen Navigation/jr/Profile Screen");
    }
}
