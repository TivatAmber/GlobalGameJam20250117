using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Enemies;
using UnityEngine;

namespace Tools
{
    public class MonsterPool : Singleton<MonsterPool>
    {
        [SerializeField]
        private List<MonsterPoolItem> monsterPrefabs = new List<MonsterPoolItem>();
    
        // 对象池字典
        private Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();
    
        // 对象池配置字典，查找某个类型的配置
        private Dictionary<string, MonsterPoolItem> poolConfigs = new Dictionary<string, MonsterPoolItem>();
    
        // 用于存放池中对象的容器
        private Transform poolContainer;

        private System.Random random = new System.Random();

        private void Awake()
        {
            InitializePools();
        }

        // 随机获取一个类型的怪物
        public GameObject GetRandomMonster()
        {
            // 获取所有可用的怪物类型
            var availableTypes = poolConfigs.Values.ToList();
            if (availableTypes.Count == 0)
                return null;
            var sum = availableTypes.Sum(item => item.weight);
            var randomValue = random.Next(0, sum);
            // var randomValue = 1;
            // Debug.Log(randomValue);

            var randomType = availableTypes.Count - 1;
            for (var i = 0; i < availableTypes.Count; i++)
            {
                randomValue -= availableTypes[i].weight;
                if (randomValue < 0)
                {
                    randomType = i;
                    break;
                }
            }

            return GetMonster(availableTypes[randomType].monsterType);
        }

        private void InitializePools()
        {
            poolContainer = new GameObject("Monster Pool Container").transform;
            poolContainer.SetParent(transform);

            // 初始化
            foreach (var item in monsterPrefabs)
            {
                if (string.IsNullOrEmpty(item.monsterType) || item.prefab == null)
                    continue;

                // 记录配置
                poolConfigs[item.monsterType] = item;

                // 创建该类型的对象列表
                pools[item.monsterType] = new List<GameObject>();

                // 预实例化对象
                for (int i = 0; i < item.initialPoolSize; i++)
                {
                    CreateNewInstance(item.monsterType);
                }
            }
        }

        private GameObject CreateNewInstance(string monsterType)
        {
            if (!poolConfigs.ContainsKey(monsterType))
                return null;

            var config = poolConfigs[monsterType];
            var instance = Instantiate(config.prefab, poolContainer);
            instance.SetActive(false);
            pools[monsterType].Add(instance);
        
            return instance;
        }

        public GameObject GetMonster(string monsterType)
        {
            if (!pools.ContainsKey(monsterType))
                return null;

            var pool = pools[monsterType];
            GameObject instance = null;

            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    instance = pool[i];
                    break;
                }
            }

            if (instance == null && poolConfigs[monsterType].canExpand)
            {
                instance = CreateNewInstance(monsterType);
            }

            if (instance != null)
            {
                instance.SetActive(true);
            
                var monster = instance.GetComponent<BaseMonster>();
                if (monster != null)
                {
                    GameManager.Instance.monsters.Add(monster);
                }
            }
            return instance;
        }

        public void ReturnMonster(GameObject monster)
        {
            if (monster == null)
                return;
            
            if (monster.TryGetComponent<BaseMonster>(out var component))
                GameManager.Instance.monsters.Remove(component);
            monster.SetActive(false);
            monster.transform.SetParent(poolContainer);
        }

        public void ReturnAllMonsters()
        {
            foreach (var pool in pools.Values)
            {
                foreach (var monster in pool)
                {
                    if (monster.activeInHierarchy)
                    {
                        ReturnMonster(monster);
                    }
                }
            }
        }

        public bool HasPool(string monsterType)
        {
            return pools.ContainsKey(monsterType);
        }

        public int GetActiveCount(string monsterType)
        {
            if (!pools.ContainsKey(monsterType))
                return 0;

            int count = 0;
            foreach (var monster in pools[monsterType])
            {
                if (monster.activeInHierarchy)
                    count++;
            }
            return count;
        }

        public int GetTotalCount(string monsterType)
        {
            if (!pools.ContainsKey(monsterType))
                return 0;

            return pools[monsterType].Count;
        }
    }
}