using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonGolf : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadGolf3()
    {
        Golf.maxRounds = 3;
        SceneManager.LoadScene("GolfSolitare");
    }

    public void loadGolf6()
    {
        Golf.maxRounds = 6;
        SceneManager.LoadScene("GolfSolitare");
    }

    public void loadGolf9()
    {
        Golf.maxRounds = 9;
        SceneManager.LoadScene("GolfSolitare");
    }
}
