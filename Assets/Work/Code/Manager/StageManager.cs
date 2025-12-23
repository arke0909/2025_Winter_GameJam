using System;
using System.Collections.Generic;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Work.Code.Events;
using Work.Code.UI;

namespace Work.Code.Manager
{
    [Serializable]
    public struct Stage
    {
        public string StageName;
        public bool IsClear;
    }
    
    [Serializable]
    public struct StageData
    {
        public List<Stage> mapClearData;
    }
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance;

        [SerializeField] private EventChannelSO gameChannel;
        [SerializeField] private List<Stage> defaultList;
        private StageData _stageData;
        private string _path = string.Empty;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            Instance = this;
            
            _stageData = new StageData
            {
                mapClearData = new List<Stage>()
            };
            _path = Application.persistentDataPath + "/stageData.json";
            if (!System.IO.File.Exists(_path))
            {
                foreach (Stage stage in defaultList)
                {
                    _stageData.mapClearData.Add(stage);
                }
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(_stageData));
            }
            else
            {
                string json = System.IO.File.ReadAllText(_path);
                StageData data = JsonUtility.FromJson<StageData>(json);
                foreach (var stage in data.mapClearData)
                {
                    _stageData.mapClearData.Add(stage);
                }
            }
            gameChannel.AddListener<GameEndEvent>(HandleGameEnd);
        }
        
        private void OnDestroy()
        {
            gameChannel.RemoveListener<GameEndEvent>(HandleGameEnd);
        }
        
        private void HandleGameEnd(GameEndEvent evt)
        {
            if(!evt.IsSuccess) return;
            for (int i = 0; i < _stageData.mapClearData.Count; i++)
            {
                if (_stageData.mapClearData[i].StageName == evt.StageName)
                    _stageData.mapClearData[i] = new Stage { StageName = evt.StageName, IsClear = true };
            }
            System.IO.File.WriteAllText(_path, JsonUtility.ToJson(_stageData));
        }

        [ContextMenu("Clear Map")]
        private void Test()
        {
            SceneManager.LoadScene("StageSelectScene");
        }

        public List<Stage> GetStageClearData()
        {
            return _stageData.mapClearData;
        }
    }
}