using MLAPI.Transports.UNET;
using Unity.RemoteConfig;
using UnityEngine;

namespace Networking {
public class NetworkManagerConfigUpdater : MonoBehaviour {
    private bool isInitializingConfig;
    private bool isFetchingConfig;
    
    private struct UserAttributes { }
    private struct AppAttributes { }
    
    private void Awake() {
        updateNetworkSettings();
    }
    
    private void OnValidate() {
        updateNetworkSettings();
    }

    private void updateNetworkSettings() {
        Debug.Log("Yep it");
        UNetTransport transport = GetComponent<UNetTransport>();
        string hostIp = ConfigManager.appConfig.GetString("host_ip");
        if (hostIp.Length > 0) transport.ConnectAddress = hostIp;
        else initRemoteConfig();
        fetchRemoteConfig();
    }

    private void initRemoteConfig() {
        if (isInitializingConfig) return;
        isInitializingConfig = true;
        ConfigManager.FetchCompleted += applyRemoteSettings;
        fetchRemoteConfig();
    }

    private void fetchRemoteConfig() {
        if (isFetchingConfig) return;
        isFetchingConfig = true;
        ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
    }

    private void applyRemoteSettings(ConfigResponse configResponse) {
        isInitializingConfig = false;
        isFetchingConfig = false;
        updateNetworkSettings();
    }
}
}
