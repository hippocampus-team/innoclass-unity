using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using General;

namespace Simulation {
public static class PythonCodeSyncer {
	public static void sync() {
		string source = File.ReadAllText(Paths.aiCodeSourcePath);
		File.WriteAllText(Paths.aiCodeTargetPath, getCleanedFetchedFile(source));
	}
	
	private static string getCleanedFetchedFile(string text) {
		MatchCollection matches = Regex.Matches(text, "def (.*)\n( {4}.*\n)+");
		return "# coding: utf-8 \n\n" + 
			   matches.Cast<Match>().Aggregate("", (current, match) => current + match.Value + "\n");
	}
	
	public static void removeSyncedFile() {
		File.Delete(Paths.aiCodeTargetPath);
	}
}
}