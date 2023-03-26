using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Model.Space;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Bserg.Controller.WorldRenderer
{
    /// <summary>
    /// Renders the ui for the planets
    /// it uses a pool system
    /// </summary>
    public class PlanetUIDrawer
    {
        public readonly Transform Parent;
        public readonly GameObject ParentPrefab, LabelPrefab, ImagePrefab, IconPrefab;
        
        private List<GameObject> pool;
        //private List<RectTransform> rectTransforms;
        private List<TextMeshProUGUI> labels;
        private List<RawImage> images, icons;
        private List<PlanetIDScript> idScripts;
        
        private int activeGameObjectsLength;
        

        public PlanetUIDrawer()
        {
            Parent = GameObject.Find("PlanetLabels").transform;
            
            ParentPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetParent");
            LabelPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetLabel");
            ImagePrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetImage");
            IconPrefab = Resources.Load<GameObject>("View/Custom/Labels/PlanetIcon");
            
            
            const int N = 30;

            
            pool = new List<GameObject>(N);
            labels = new List<TextMeshProUGUI>(N);
            images = new List<RawImage>(N);
            icons = new List<RawImage>(N);
            idScripts = new List<PlanetIDScript>(N);

            for (int i = 0; i < N; i++)
                AddPlanetLabel();
        }

        /// <summary>
        /// Draws the labels on the planets positions
        /// </summary>
        /// <param name="planetPositions"></param>
        /// <param name="planets"></param>
        /// <param name="ids"></param>
        public void Draw(float3[] planetPositions, PlanetOld[] planets, List<int> ids)
        {
            int n = ids.Count;
            AdjustPool(n);

            // Draw labels on planets
            for (int i = 0; i < n; i++)
            {
                PlanetOld planetOld = planets[ids[i]];
                idScripts[i].planetID = ids[i];
                Vector3 position = new Vector3(planetPositions[i].x, planetPositions[i].y, 0);
                Vector3 iconSize = SystemGenerator.GetIconPlanetSize(planetOld.Size);
                pool[i].transform.position = Camera.main.WorldToScreenPoint(position);
                labels[i].text = planetOld.Name;
                icons[i].transform.localScale = iconSize;
                icons[i].color = planetOld.Color;
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

        // ReSharper disable Unity.PerformanceAnalysis
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
            
            GameObject imageGo = Object.Instantiate(ImagePrefab, above);
            RawImage imageImage = imageGo.GetComponent<RawImage>();
            imageImage.rectTransform.offsetMax = new Vector2(imageImage.rectTransform.offsetMax.x, 0);
            images.Add(imageImage);
            
            GameObject iconGo = Object.Instantiate(IconPrefab, center);
            icons.Add(iconGo.GetComponent<RawImage>());
            
            idScripts.Add(iconGo.GetComponent<PlanetIDScript>());
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