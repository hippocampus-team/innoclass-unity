import math


def softsign(x):
    return x / (abs(x) + 1)


class Layer:
    def __init__(self, neurons_count, output_count):
        self.size = neurons_count
        self.next_size = output_count
        self.neurons = [0] * neurons_count
        self.biases = [0] * neurons_count
        self.weights = list()

        for neuron in range(neurons_count):
            self.weights.append([0] * output_count)

    def set_weights(self, weights):
        counter = 0
        for neuron in range(self.size):
            for next_neuron in range(self.next_size):
                self.weights[neuron][next_neuron] = weights[counter]
                counter += 1

        for i in range(counter, len(weights)):
            self.biases[i - counter] = weights[i]

    def set_values(self, values):
        self.neurons = values


class NeuralNetwork:
    def __init__(self, size):
        self.network = list()
        for layer in range(len(size) - 1):
            self.network.append(Layer(size[layer], size[layer + 1]))
        self.network.append(Layer(size[-1], 0))

    def calculate(self, input):
        self.network[0].set_values(input)
        for layer in range(1, len(self.network)):
            for current_neuron in range(self.network[layer].size):
                self.network[layer].neurons[current_neuron] = 0
                for previous_neuron in range(self.network[layer - 1].size):
                    self.network[layer].neurons[current_neuron] += self.network[layer - 1].weights[previous_neuron][
                                                                       current_neuron] * \
                                                                   self.network[layer - 1].neurons[previous_neuron]
                self.network[layer].neurons[current_neuron] = softsign(self.network[layer].neurons[current_neuron] + self.network[layer].biases[current_neuron])
        return self.network[-1].neurons
