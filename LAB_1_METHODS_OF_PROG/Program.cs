using LAB_1_METHODS_OF_PROG;

VirtualMemory Memory = new("VM.bin", 35, 4, 4);
int element = 0;

Console.WriteLine("\t\t---Запись значений с индексами: 0, 1, 19, 34, 5000000---");
Console.WriteLine("---Пояснение: True - операция была выполнена, False - операция не была выполнена---\n");

Console.WriteLine($"Результат операции: {Memory.SetElement(0, 2123232)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(1, 13589)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(19, 323)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(34, 5464)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(5000000, 135)}");

Console.WriteLine("\n\t\t---Вывод значений с индексами: 0, 1, 19, 5000000---");
Console.WriteLine("---Пояснение: True - операция была выполнена, False - операция не была выполнена---\n");

Console.WriteLine($"Результат операции: {Memory.GetElement(0, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(1, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(19, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(34, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(5000000, ref element)}, element: {element}");
Memory.Dispose();