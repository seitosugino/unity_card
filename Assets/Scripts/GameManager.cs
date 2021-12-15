using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform plsyerHandTransform;
    [SerializeField] GameObject cardPrefab;
    void Start()
    {
        CreateCard(plsyerHandTransform);
    }
    void CreateCard(Transform hand)
    {
        Instantiate(cardPrefab, hand, false);
    }
}
