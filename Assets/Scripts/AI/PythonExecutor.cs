using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using UnityEngine;

namespace AI {
public class PythonExecutor {
	private static PythonExecutor instance;
	private readonly dynamic env;

	private PythonExecutor() {
		ScriptEngine engine = Python.CreateEngine();
		env = engine.ExecuteFile(Application.dataPath + @"\StreamingAssets" + @"\ai.py");
	}

	public dynamic getPythonEnv() {
		return env;
	}

	public static PythonExecutor getInstance() {
		return instance ??= new PythonExecutor();
	}
}
}