using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public class WorldLabelUI
    {
        public readonly Transform Parent;
        public readonly GameObject Prefab;
        
        private List<GameObject> pool;
        private List<TextMeshProUGUI> labels;
        private int activeGameObjectsLength;
        

        public WorldLabelUI(GameObject prefab, Transform parent)
        {
            Prefab = prefab;
            Parent = parent;
            
            const int N = 30;

            pool = new List<GameObject>(N);
            labels = new List<TextMeshProUGUI>(N);

            for (int i = 0; i < N; i++)
                AddLabel();
        }

        /// <summary>
        /// Draws the labels on the planets positions
        /// </summary>
        /// <param name="planetNames"></param>
        /// <param name="planetPositions"></param>
        public void DrawLabelsOnPlanets(string[] planetNames, Vector3[] planetPositions)
        {
            int n = planetNames.Length;
            AdjustPool(n);

            // Draw labels on planets
            for (int i = 0; i < n; i++)
            {
                Vector3 position = new Vector3(planetPositions[i].x, planetPositions[i].y - .5f, 0);
                pool[i].transform.position = position;
                labels[i].text = planetNames[i];
                
            }
        }

        /// <summary>
        /// Makes sure that pool has the right amount of active and enabled elements
        /// </summary>
        /// <param name="n"></param>
        public void AdjustPool(int n)
        {
            int poolDiff = n - labels.Count;
            int activeDiff = n - activeGameObjectsLength;

            // Increase pool size if too many
            for (int i = 0; i < poolDiff; i++)
                AddLabel();

            // Set active if too many or deactivate some if too few
            for (int i = 0; i < activeDiff; i++)
                EnableLabel();

            // Deactivate if too few
            for (int i = 0; i < -activeDiff; i++)
                DisableLabel();
        }

        /// <summary>
        /// Creates a new label gameObject
        /// </summary>
        void AddLabel()
        {
            GameObject go = Object.Instantiate(Prefab, Parent);
            pool.Add(go);
            labels.Add(go.GetComponent<TextMeshProUGUI>());
            go.SetActive(false);
        }

        /// <summary>
        /// Disable label from pool
        /// </summary>
        void DisableLabel()
        {
            pool[activeGameObjectsLength--].SetActive(true);
        }

        
        void EnableLabel()
        {
            pool[activeGameObjectsLength++].SetActive(true);
        }
    }
}