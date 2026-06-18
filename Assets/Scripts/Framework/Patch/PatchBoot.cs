using System.Collections;
using UnityEngine;

namespace Framework.Patch
{
    public class PatchBoot : MonoBehaviour
    {
        public static PatchBoot Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public Coroutine RunCoroutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
    }
}
