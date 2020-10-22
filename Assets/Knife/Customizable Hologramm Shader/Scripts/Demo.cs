using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Knife.HologramEffect
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private GameObjectsGroup[] groups;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        private int currentGroup;

        private void Start()
        {
            currentGroup = 0;
            OpenCurrent();

            previousButton.onClick.AddListener(Previous);
            nextButton.onClick.AddListener(Next);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                Next();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Previous();
            }
        }

        private void Next()
        {
            currentGroup++;
            if (currentGroup >= groups.Length)
            {
                currentGroup = 0;
            }

            OpenCurrent();
        }

        private void Previous()
        {
            currentGroup--;
            if (currentGroup < 0)
            {
                currentGroup = groups.Length - 1;
            }

            OpenCurrent();
        }

        private void OpenCurrent()
        {
            foreach (var g in groups)
            {
                g.SetActive(false);
            }
            groups[currentGroup].SetActive(true);
        }

        [System.Serializable]
        private class GameObjectsGroup
        {
            [SerializeField] private GameObject[] gameObjects;

            public void SetActive(bool enabled)
            {
                foreach(var g in gameObjects)
                {
                    g.SetActive(enabled);
                }
            }
        }
    }
}