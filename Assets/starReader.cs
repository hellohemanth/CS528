using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class starReader : MonoBehaviour
{
    public GameObject O_prefab, B_prefab, A_prefab, F_prefab, G_prefab, K_prefab, M_prefab, line;

    public Dictionary<string, GameObject> starPrefabs;

    void Start()
    {
        InitializeStarPrefabs();
        StartCoroutine(LoadCSVFile());
        // Sculptor constellation consisting of three stars
        DrawLineBetweenStars(4577, 115102);
        DrawLineBetweenStars(115102, 116231);
    }

    void InitializeStarPrefabs()
    {
        starPrefabs = new Dictionary<string, GameObject>
        {
            { "O", O_prefab },
            { "B", B_prefab },
            { "A", A_prefab },
            { "F", F_prefab },
            { "G", G_prefab },
            { "K", K_prefab },
            { "M", M_prefab }
        };
    }
    //create a dict
    IEnumerator LoadCSVFile()
    {
        string csvPath = "Assets/updatedStarData.csv";
        CultureInfo culture = new CultureInfo("en-US");
        int count = 0;
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV file not found at path: {csvPath}");
            yield break;
        }

        using (StreamReader reader = new StreamReader(csvPath))
        {
            while (!reader.EndOfStream)
            {
            
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                //Debug.Log(values.Length);
                float x0, y0, z0;
                float hip, dist, absMag, mag, vx, vy, vz;
                string spect = "";
                Vector3 pos = new Vector3();
                Vector3 vel = new Vector3();

                if (float.TryParse(values[2], NumberStyles.Float, culture, out x0) && float.TryParse(values[3], NumberStyles.Float, culture, out y0) && float.TryParse(values[4], NumberStyles.Float, culture, out z0))
                {
                    pos = new Vector3(x0, y0, z0);
                    float.TryParse(values[0], NumberStyles.Float, culture, out hip);
                    float.TryParse(values[1], NumberStyles.Float, culture, out dist);
                    float.TryParse(values[5], NumberStyles.Float, culture, out absMag);
                    float.TryParse(values[6], NumberStyles.Float, culture, out mag);
                    spect = values[10];
                    
                    if (float.TryParse(values[7], NumberStyles.Float, culture, out vx) && float.TryParse(values[8], NumberStyles.Float, culture, out vy) && float.TryParse(values[9], NumberStyles.Float, culture, out vz))
                    {
                        vel = new Vector3(vx, vy, vz);
                    }
                    //Debug.Log(pos + " " + hip + " " + dist + " " + absMag + " " + mag + " " + spect + " " + vel);
                    count++;
                }
                else { Debug.LogError("star position not parsed"); }
                
                SpawnStar(spect, pos, count);
            }
        }
    }

    void SpawnStar(string spect, Vector3 position, int id)
    {
        if (starPrefabs.TryGetValue(spect, out GameObject prefab))
        {
            GameObject star = Instantiate(prefab, position, Quaternion.identity);
            AddStarToDictionary(id, star);
        }
        else
        {
            Debug.LogWarning($"Prefab for spectral type '{spect}' not found.");
        }
    }

    private Dictionary<int, GameObject> starDictionary = new Dictionary<int, GameObject>();

    private void AddStarToDictionary(int id, GameObject starObject)
    {
        if (!starDictionary.ContainsKey(id))
        {
            starDictionary.Add(id, starObject);
        }
        else
        {
            Debug.LogError("Duplicate star ID detected: " + id);
        }
    }

    private GameObject GetStarById(int id)
    {
        if (starDictionary.TryGetValue(id, out GameObject starObject))
        {
            return starObject;
        }
        else
        {
            Debug.LogError("Star ID not found: " + id);
            return null;
        }
    }

    void DrawLineBetweenStars(int id1, int id2)
    {
        GameObject star1 = GetStarById(id1);
        GameObject star2 = GetStarById(id2);
        GameObject lineObj = Instantiate(line, Vector3.zero, Quaternion.identity);
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

        if (star1 == null || star2 == null || lineRenderer == null) return;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, star1.transform.position);
        lineRenderer.SetPosition(1, star2.transform.position);
    }



}
