using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimages : MonoBehaviour
{
    public GameObject ImagePrefab;
    public int imageNumber = 1;
    public List<GameObject> shadow;
    public List<Transform> mainTransforms;
    public List<List<Transform>> shadowTransforms;
    public List<Vector3> imagePositions;
    public List<Quaternion> imageRotations;
    public List<float> delayAmountPerShadow;
    public bool UseRootTransform = false;
    private float lastCopy = 0f;
    private bool transformsSet = false;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        shadow = new List<GameObject>();
        shadowTransforms = new List<List<Transform>>();
        for (int i = 0; i < imageNumber; i++)
            shadowTransforms.Add(new List<Transform>());
        if (UseRootTransform)
            mainTransforms.Add(transform);
        GetMainTransforms(transform);
        for (int i = 0; i < imageNumber; i++)
        {
            GameObject obj = Instantiate(ImagePrefab, transform.position, transform.rotation);
            shadow.Add(obj);
        }
        for (int i = 0; i < imageNumber; i++)
        {
            if (UseRootTransform)
                shadowTransforms[i].Add(shadow[i].transform.GetChild(0));
            GetShadowTransforms(shadow[i].transform.GetChild(0), i);
        }
    }

    void GetMainTransforms(Transform t)
    {
        foreach (Transform c in t)
        {
            mainTransforms.Add(c);
            if (c.childCount > 0)
                GetMainTransforms(c);
        }
    }
    void GetShadowTransforms(Transform t, int index)
    {
        foreach (Transform c in t)
        {
            shadowTransforms[index].Add(c);
            if (c.childCount > 0)
                GetShadowTransforms(c, index);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > lastCopy + delayAmountPerShadow[count] || lastCopy == 0f)
        {
            SetFramePositions();
            for (int j = 0; j < imagePositions.Count; j++)
            {
                shadowTransforms[count][j].position = imagePositions[j];
                shadowTransforms[count][j].rotation = imageRotations[j];
            }
            count++;
            imagePositions.Clear();
            imageRotations.Clear();
            if (count == imageNumber)
            {
                count = 0;
                lastCopy = Time.time;
            }
        }
    }
    void SetFramePositions()
    {
        foreach (Transform t in mainTransforms)
        {
            imagePositions.Add(t.position);
            imageRotations.Add(t.rotation);
        }
    }
}
