using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapChangeTest : MonoBehaviour
{
    public enum MapType
    {
        Field,
        Forest
    }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmField;
    [SerializeField] private AudioClip bgmForest;

    private MapType mapType;
    [SerializeField] private GameObject imageF1;
    [SerializeField] private GameObject imageF2;

    void Awake()
    {
        ChangeMap(MapType.Field);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            mapType = MapType.Field;
            ChangeMap(mapType);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            mapType = MapType.Forest;
            ChangeMap(mapType);
        }
    }

    void ChangeMap(MapType mt)
    {
        mapType = mt;

        if (imageF1 != null) imageF1.SetActive(mt == MapType.Field);
        if (imageF2 != null) imageF2.SetActive(mt == MapType.Forest);

        AudioClip currentClip = (mapType == MapType.Field) ? bgmField : bgmForest;

        if (bgmSource == null || currentClip == null) return;
        if (bgmSource.clip == currentClip && bgmSource.isPlaying) return;

        bgmSource.clip = currentClip;
        bgmSource.Play();

        Debug.Log($"ÇöÀç ¸Ê: {mt}");
    }
}
