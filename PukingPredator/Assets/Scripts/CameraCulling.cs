using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    /// <summary>
    /// Object to track the what should be culled.
    /// </summary>
    [SerializeField]
    private CollisionTracker collisionTracker;

    /// <summary>
    /// The lists of culling data for each object that has been culled.
    /// </summary>
    private Dictionary<GameObject, List<CullingData>> allCullingData = new();

    private class CullingData
    {
        public float alpha => material.GetAlpha();
        private float alphaMultiplier = 0.15f;
        private float alphaRate = 8f;
        private bool hasBeenReset = false;
        private float initialAlpha;
        private SurfaceType initialSurfaceType;
        public bool isFullOpacity => (initialAlpha - alpha) < 0.2f * initialAlpha;
        private Material material;

        public CullingData(Material material)
        {
            this.material = material;
            initialAlpha = material.GetAlpha();
            initialSurfaceType = material.GetSurfaceType();

            if (initialSurfaceType != SurfaceType.transparent)
            {
                material.SetSurfaceTypeTransparent();
            }
        }

        ~CullingData()
        {
            Reset();
        }

        public void Reset()
        {
            if (hasBeenReset) { return; }

            material.SetAlpha(initialAlpha);
            if (initialSurfaceType != SurfaceType.transparent)
            {
                material.SetSurfaceType(initialSurfaceType);
            }
        }

        public void Update(bool isObstructingView)
        {
            var currentAlpha = material.GetAlpha();
            var targetAlpha = isObstructingView ? initialAlpha * alphaMultiplier : initialAlpha;
            var currAlphaRate = alphaRate * Time.deltaTime;

            material.SetAlpha(Mathf.Lerp(currentAlpha, targetAlpha, currAlphaRate));
        }
    }



    private void LateUpdate()
    {
        var currentlyObstructingView = collisionTracker.collisions;
        var previouslyCulledObjects = allCullingData.Keys;

        var noLongerObstructingObjects = previouslyCulledObjects.Where(o => !currentlyObstructingView.Contains(o));
        var newObstructions = currentlyObstructingView.Where(o => !previouslyCulledObjects.Contains(o));

        //setup the culling data for new obstructions
        foreach (var newObstruction in newObstructions)
        {
            allCullingData[newObstruction] = CreateCullingList(newObstruction);
        }

        //start fading back to 100% alpha and then remove the culling data if
        //everything has reached its full opacity.
        List<GameObject> toRemove = new();
        foreach (var nonObstructing in noLongerObstructingObjects)
        {
            var cullingData = allCullingData[nonObstructing];
            foreach (CullingData cd in cullingData)
            {
                cd.Update(isObstructingView: false);
            }

            //if all full opacity, clear culling data
            if (cullingData.All(cd => cd.isFullOpacity))
            {
                foreach (CullingData cd in cullingData)
                {
                    cd.Reset();
                }
                toRemove.Add(nonObstructing);
                
            }
        }
        foreach (var r in toRemove)
        {
            allCullingData.Remove(r);
        }

        foreach (var obstructing in currentlyObstructingView)
        {
            foreach (CullingData cd in allCullingData[obstructing])
            {
                cd.Update(isObstructingView: true);
            }
        }
    }



    private List<CullingData> CreateCullingList(GameObject rootObject)
    {
        List<CullingData> result = new();

        var objects = rootObject.GetDescendantsAndSelf();
        foreach (var obj in objects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) { continue; }

            //get the materials
            var materials = renderer.materials
                            .Where(m => !m.name.Contains("Outline"))
                            .ToList();

            //add the culling data for each relevant material for this object
            result.AddRange(
                materials
                .Where(m => m.HasProperty("_Color"))
                .Select(m => new CullingData(m))
            );

            //reset the materials so it doesnt break QuickOutline
            renderer.materials = materials.ToArray();
        }

        return result;
    }
}