using System.IO;
using System.Text;

namespace General {
    public class SimulationModel {
        public readonly string name;
        public bool isActivated;
        public Genotype genotype;

        private SimulationModel(string name, uint[] topology, bool isActivated = false) : 
            this(name, Genotype.generateRandom(new PyNeuralNetwork(topology).weightCount), isActivated){ }

        private SimulationModel(string name, Genotype genotype, bool isActivated) {
            this.name = name;
            this.isActivated = isActivated;
            this.genotype = genotype;
        }

        public static SimulationModel generateFromTopology(string name, uint[] topology) {
            return new SimulationModel(name, topology);
        }

        public static SimulationModel loadFromFile(string filePath, string name) {
            string[] lines = File.ReadAllLines(filePath);
            return new SimulationModel(name, Genotype.loadFromRaw(lines[0]), lines[1].Trim().Equals("1"));
        }
        
        public void saveToFile(string filePath) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(genotype.getRaw());
            stringBuilder.Append(isActivated ? "1" : "0");
            File.WriteAllText(filePath, stringBuilder.ToString());
        }
    }
}