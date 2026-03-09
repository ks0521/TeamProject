using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMChanger : MonoBehaviour
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
    [SerializeField] private GameObject F1Field;
    [SerializeField] private GameObject F2Forest;

    void Awake()
    {
        ChangeMap(MapType.Field);
    }

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

    void ChangeMap(MapType currentMap)
    {
        mapType = currentMap;

        if (F1Field != null) F1Field.SetActive(currentMap == MapType.Field);
        if (F2Forest != null) F2Forest.SetActive(currentMap == MapType.Forest);

        AudioClip currentClip = (mapType == MapType.Field) ? bgmField : bgmForest;

        if (bgmSource == null || currentClip == null) return;
        if (bgmSource.clip == currentClip && bgmSource.isPlaying) return;

        bgmSource.clip = currentClip;
        bgmSource.Play();

        Debug.Log($"ÇöÀç ¸Ê: {currentMap}");
    }
}
