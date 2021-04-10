using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrawController : MonoBehaviour
{
    public GameObject drawPoint;
    public LayerMask groundLayer;
    public Camera mainCamera;

    const int TouchPoints = 10;
    const int MouseTouchIndex = 20;
    GameObject[] touchPointObjects;
    DrawPoint[] drawPoints;

    Dictionary<int, int> touchToIdx;
    HashSet<int> freeIdx;
    static readonly int ColorProp = Shader.PropertyToID("_Color");

    void Start()
    {
        touchPointObjects = new GameObject[TouchPoints];
        drawPoints = new DrawPoint[TouchPoints];
        for (int i = 0; i < TouchPoints; i++)
        {
            GameObject drawPointInstance = Instantiate(this.drawPoint, new Vector3(10000,0,0), Quaternion.identity);
            Renderer sphereRenderer = drawPointInstance.GetComponent<Renderer>();
            sphereRenderer.material.SetColor(ColorProp, new Color(
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f), 
                    Random.Range(0f, 1f)));
            drawPointInstance.transform.parent = transform.parent;
            drawPointInstance.transform.localScale = new Vector3(1, 1, 1)* 1.8f;
            touchPointObjects[i] = drawPointInstance;
            drawPoints[i] = drawPointInstance.GetComponent<DrawPoint>();
            freeIdx.Add(i);
        }
    }

    void OnEnable()
    {
        touchToIdx = new Dictionary<int, int>();
        freeIdx = new HashSet<int>(Enumerable.Range(0, TouchPoints));
    }

    void OnDisable()
    {
        for (int i = 0; i < TouchPoints; i++)
        {
            drawPoints[i].Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInput();
        
        if (Input.touchCount != 0) return;
        
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddTouch(MouseTouchIndex);
        }

        if (Input.GetMouseButtonUp(0))
        {
            RemoveTouch(MouseTouchIndex);
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateTouch(MouseTouchIndex, Input.mousePosition);
        }
    }

    void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    AddTouch(touch.fingerId);
                    break;
                case TouchPhase.Ended:
                    RemoveTouch(touch.fingerId);
                    continue;
            }

            UpdateTouch(touch.fingerId, touch.position);
        }
    }

    void AddTouch(int idx)
    {
        if (freeIdx.Count == 0) return;
        
        touchToIdx[idx] = freeIdx.First();
        freeIdx.Remove(touchToIdx[idx]);
        drawPoints[touchToIdx[idx]].Activate();
    }

    void RemoveTouch(int idx)
	{
        drawPoints[touchToIdx[idx]].Disable();
        freeIdx.Add(touchToIdx[idx]);
    }

    void UpdateTouch(int idx, Vector2 touchPosition)
	{
        // Construct a ray from the current touch coordinates
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        bool hit = Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, groundLayer);
        if (!hit) return;
        
        Transform t = touchPointObjects[touchToIdx[idx]].transform;
        t.forward = info.normal;
        t.position = new Vector3(info.point.x, info.point.y, info.point.z) + t.forward*0.25f;
    }
}
