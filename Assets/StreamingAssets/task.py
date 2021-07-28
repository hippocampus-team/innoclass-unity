# Функция активации
def activation_func(x):
    return x / (abs(x) + 1)


def calculate(input, weights, bias):
    output = 0
    # TODO: посчитайте output (вывод нейронной сети)
    return output


weights = [0.1, 0.2, 0]  # Веса
bias = 0.4  # Смещение нейрона

input = [8.5, 0.65, 1.2]  # Входные значения
output = calculate(input, weights, bias)  # Вывод

print('Вывод нейронной сети:', output)
