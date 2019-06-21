using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
namespace FantasyErrand
{
    public class LevelManagerSelector : MonoBehaviour
    {
        public GameObject[] levelManagerList;
       
        // Use this for initialization
        void Start()
        {
            if (MainMenuManager.difficultyLevel.Equals("easy"))
                levelManagerList[0].SetActive(true);
            else if (MainMenuManager.difficultyLevel.Equals("normal"))
                levelManagerList[1].SetActive(true);
            else if (MainMenuManager.difficultyLevel.Equals("hard"))
                levelManagerList[2].SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}