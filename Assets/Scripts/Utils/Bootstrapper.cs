using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UrchinGame.Utils {
    public static class Bootstrapper {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitAddressables() => Addressables.InitializeAsync();
    }
}
