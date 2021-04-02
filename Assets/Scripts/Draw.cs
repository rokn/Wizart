using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Draw : MonoBehaviour
{
    const int TOUCH_POINTS = 10;
    GameObject[] touchPointSpheres;

    Dictionary<int, int> touchToIdx;
    HashSet<int> freeIdx;

    // Start is called before the first frame update
    void Start()
    {
        touchToIdx = new Dictionary<int, int>();
        freeIdx = new HashSet<int>();
        touchPointSpheres = new GameObject[TOUCH_POINTS];
        for (int i = 0; i < TOUCH_POINTS; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var sphereRenderer = sphere.GetComponent<Renderer>();
            sphereRenderer.material.SetColor("_Color", new Color(
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f)));
            touchPointSpheres[i] = sphere;
            freeIdx.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (freeIdx.Count != 0)
                {
                    touchToIdx[touch.fingerId] = freeIdx.First();
                    freeIdx.Remove(touchToIdx[touch.fingerId]);
                }
            } 
            else if (touch.phase == TouchPhase.Ended)
            {
                freeIdx.Add(touchToIdx[touch.fingerId]);
                continue;
            }

            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            RaycastHit info;
            if (Physics.Raycast(ray, out info))
            {
                Transform t = touchPointSpheres[touchToIdx[touch.fingerId]].transform;
                t.position = new Vector3(info.point.x, info.point.y, t.position.z);
            }
        }
    }
}
