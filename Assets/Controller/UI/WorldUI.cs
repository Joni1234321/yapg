using System.Collections.Generic;
using Bserg.Controller.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bserg.Controller.UI
{
    public class WorldUI
    {
        public OrbitController OrbitController;
            
        public readonly Transform Parent;
        public readonly GameObject ParentPrefab, LabelPrefab, ImagePrefab, IconPrefab;
        
        private List<GameObject> pool;
        private List<TextMeshProUGUI> labels;
        private List<RawImage> images, icons;
        
        private int activeGameObjectsLength;
        

        public WorldUI(OrbitController orbitController,  Transform parent)
        {
            OrbitController = orbitController;
            
            ParentPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetParent");
            LabelPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetLabel");
            ImagePrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetImage");
            IconPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetIcon");
            
            Parent = parent;
            
            const int N = 30;

            
            pool = new List<GameObject>(N);
            labels = new List<TextMeshProUGUI>(N);
            images = new List<RawImage>(N);
            icons = new List<RawImage>(N);

            for (int i = 0; i < N; i++)
                AddPlanetLabel();
        }

        /// <summary>
        /// Draws the labels on the planets positions
        /// </summary>
        /// <param name="planetPositions"></param>
        /// <param name="planets"></param>
        public void DrawUI(Vector3[] planetPositions, List<Model.Space.Planet> planets)
        {
            int n = planets.Count;
            AdjustPool(n);

            // Draw labels on planets
            for (int i = 0; i < n; i++)
            {
                Vector3 position = new Vector3(planetPositions[i].x, planetPositions[i].y, 0);
                Vector3 iconSize = OrbitController.SystemGenerator.GetIconPlanetSize(planets[i].Size);
                pool[i].transform.position = position;
                labels[i].text = planets[i].Name;
                icons[i].transform.localScale = iconSize;
                icons[i].color = planets[i].Color;
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
                AddPlanetLabel();


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
        void AddPlanetLabel()
        {
            GameObject go = Object.Instantiate(ParentPrefab, Parent);
            pool.Add(go);
            Transform above = go.transform.GetChild(0);
            Transform center = go.transform.GetChild(1);
            Transform below = go.transform.GetChild(2);

            GameObject labelGo = Object.Instantiate(LabelPrefab, below);
            labels.Add(labelGo.GetComponent<TextMeshProUGUI>());
            
            GameObject imageGo = Object.Instantiate(ImagePrefab, below);
            images.Add(imageGo.GetComponent<RawImage>());
            imageGo.SetActive(false);
            
            GameObject iconGo = Object.Instantiate(IconPrefab, center);
            icons.Add(iconGo.GetComponent<RawImage>());
            
            //iconGo.SetActive(false);
            
            go.SetActive(false);
        }

        /// <summary>
        /// Disable label from pool
        /// </summary>
        void DisableLabel()
        {
            pool[--activeGameObjectsLength].SetActive(false);
        }

        
        void EnableLabel()
        {
            pool[activeGameObjectsLength++].SetActive(true);
        }
    }

}