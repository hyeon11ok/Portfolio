using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDropData
{
    [SerializeField] private GameObject itemPrefab;     // 드랍될 아이템 프리팹
    [Range(0f, 1f)]
    [SerializeField] private float dropChance;          // 이 아이템이 드랍될 확률 (0~1)

    public GameObject ItemPrefab => itemPrefab;
    public float DropChance => dropChance;
}

public class ItemDropper : MonoBehaviour
{
    [Header("전체 드랍 확률")]
    [Range(0f, 1f)]
    [SerializeField] private float dropRate = 0.5f;

    [Header("드랍 가능한 아이템 목록")]
    [SerializeField] private ItemDropData[] dropItems;

    private Transform container;

    private void Start()
    {
        container = GetComponentInParent<Room>().transform;
    }

    public void TryDropItem()
    {
        // 전체 드랍 확률 체크
        if(Random.value > dropRate)
            return;

        // 드랍할 아이템 선정
        float total = 0f;
        foreach(ItemDropData item in dropItems)
            total += item.DropChance;

        float randomPoint = Random.value * total;
        float current = 0f;

        foreach(ItemDropData item in dropItems)
        {
            current += item.DropChance;
            if(randomPoint <= current)
            {

                Instantiate(item.ItemPrefab, transform.position, Quaternion.identity, container);
                break;
            }
        }
    }
}
