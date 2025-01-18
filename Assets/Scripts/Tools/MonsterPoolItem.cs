using UnityEngine;

namespace Tools
{
    [System.Serializable]
    public class MonsterPoolItem
    {
        public string monsterType;            // 类型标识
        public GameObject prefab;             // 预制体
        public int initialPoolSize = 10;      // 初始大小
        public bool canExpand = true;         // 允许扩展
        public int weight = 1;
    }
}