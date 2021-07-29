using System.IO;
using General;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Simulation;

namespace AI {
public class PythonExecutor {
	private static PythonExecutor instance;
	private readonly dynamic env;

	private PythonExecutor() {
		if (!File.Exists(Paths.aiCodeTargetPath)) PythonCodeSyncer.sync();
		ScriptEngine engine = Python.CreateEngine();
		env = engine.ExecuteFile(Paths.aiCodeTargetPath);
	}

	public dynamic getPythonEnv() {
		return env;
	}

	public static PythonExecutor getInstance() {
		return instance ??= new PythonExecutor();
	}
}
}