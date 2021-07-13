using UnityEngine;

namespace Simulation {
public static class UserManager {
	private const string usernameCode = "username";

	public static string username {
		get => PlayerPrefs.GetString(usernameCode);
		set {
			PlayerPrefs.SetString(usernameCode, value);
			PlayerPrefs.Save();
		}
	}
}
}