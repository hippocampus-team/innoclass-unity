def activation_func(x):
    return x / (abs(x) + 1)


def calculate(input, weights, bias):
    output = 0
    for i in range(len(input)):
        output += input[i] * weights[i] 
    return activation_func(output + bias)
