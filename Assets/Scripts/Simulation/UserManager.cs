using UnityEngine;

namespace Simulation {
public static class UserManager {
	private const string usernameCode = "username";
	private const string controlTypeCode = "control_type";

	public static string username {
		get => PlayerPrefs.GetString(usernameCode);
		set {
			PlayerPrefs.SetString(usernameCode, value);
			PlayerPrefs.Save();
		}
	}
	
	public static bool userControl {
		get => PlayerPrefs.GetInt(controlTypeCode) == 1;
		set {
			PlayerPrefs.SetInt(controlTypeCode, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}
}
}