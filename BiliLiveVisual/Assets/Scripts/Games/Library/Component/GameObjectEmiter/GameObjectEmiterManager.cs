using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace BLVisual
{
    public class GameObjectEmiterManager : MonoSingleton<GameObjectEmiterManager>
    {
        private Dictionary<string, GameObjectEmiter> _emiterDict = new Dictionary<string, GameObjectEmiter>();
        public GameObjectEmiter GetOrCreate(string name = null)
        {
            if (string.IsNullOrEmpty(name))
                return CreateEmiter(name);

            if (!_emiterDict.TryGetValue(name, out var emiter))
            {
                emiter = CreateEmiter(name);
                _emiterDict[name] = emiter;
            }

            return emiter;
        }

        public void DestroyEmiter(GameObjectEmiter emiter)
        {
            _emiterDict.Remove(emiter.name);
            Object.Destroy(emiter.gameObject);
        }

        private GameObjectEmiter CreateEmiter(string name)
        {
            GameObject emiterGo = new GameObject(name);
            var emiterComp = emiterGo.GetComponent<GameObjectEmiter>() ?? emiterGo.AddComponent<GameObjectEmiter>();
            emiterComp.name = name;
            emiterGo.transform.SetParent(transform);
            return emiterComp;
        }
    }

}
