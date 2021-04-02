using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Draw : MonoBehaviour
{
    public GameObject DrawPoint;

    const int TOUCH_POINTS = 10;
    const int MOUSE_TOUCH_INDEX = 20;
    GameObject[] touchPointObjects;

    Dictionary<int, int> touchToIdx;
    HashSet<int> freeIdx;

    // Start is called before the first frame update
    void Start()
    {
        touchToIdx = new Dictionary<int, int>();
        freeIdx = new HashSet<int>();
        touchPointObjects = new GameObject[TOUCH_POINTS];
        for (int i = 0; i < TOUCH_POINTS; i++)
        {
            GameObject drawPoint = Instantiate(DrawPoint, new Vector3(0,0,0), transform.parent.rotation);
            var sphereRenderer = drawPoint.GetComponent<Renderer>();
            sphereRenderer.material.SetColor("_Color", new Color(
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f)));
            drawPoint.transform.parent = transform.parent;
            touchPointObjects[i] = drawPoint;
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
                AddTouch(touch.fingerId);
            } 
            else if (touch.phase == TouchPhase.Ended)
            {
                RemoveTouch(touch.fingerId);
                continue;
            }

            UpdateTouch(touch.fingerId, touch.position);
        }

        if(Input.GetMouseButtonDown(0))
        {
            AddTouch(MOUSE_TOUCH_INDEX);
        }
        if(Input.GetMouseButtonUp(0))
        {
            RemoveTouch(MOUSE_TOUCH_INDEX);
        }
        else if (Input.GetMouseButton(0))
		{
            UpdateTouch(MOUSE_TOUCH_INDEX, Input.mousePosition);
		}
    }

    void AddTouch(int idx)
	{
        if(freeIdx.Count != 0)
        {
            touchToIdx[idx] = freeIdx.First();
            freeIdx.Remove(touchToIdx[idx]);
        }
    }

    void RemoveTouch(int idx)
	{
        freeIdx.Add(touchToIdx[idx]);
    }

    void UpdateTouch(int idx, Vector2 touchPosition)
	{
        // Construct a ray from the current touch coordinates
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        RaycastHit info;
        if(Physics.Raycast(ray, out info))
        {
            Transform t = touchPointObjects[touchToIdx[idx]].transform;
            t.position = new Vector3(info.point.x, info.point.y, info.point.z);
        }
    }
}
