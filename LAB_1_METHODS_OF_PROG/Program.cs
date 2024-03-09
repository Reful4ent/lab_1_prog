using LAB_1_METHODS_OF_PROG;

// Создание экземпляра класса (файл) с заданными значениями 
VirtualMemory Memory = new("VM.bin", 10000, 4, 128);
int element = 0;

Console.WriteLine("\t\t---Запись значений с индексами: 0, 898, 1500, 5000000---");
Console.WriteLine("---Пояснение: True - операция была выполнена, False - операция не была выполнена---\n");

Console.WriteLine($"Результат операции: {Memory.SetElement(0, 21)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(1500, 323)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(898, 5464)}");
Console.WriteLine($"Результат операции: {Memory.SetElement(5000000, 135)}");

Console.WriteLine("\n\t\t---Вывод значений с индексами: 0, 898, 1500, 5000000---");
Console.WriteLine("---Пояснение: True - операция была выполнена, False - операция не была выполнена---\n");

Console.WriteLine($"Результат операции: {Memory.GetElement(898, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(1500, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(0, ref element)}, element: {element}");
Console.WriteLine($"Результат операции: {Memory.GetElement(5000000, ref element)}, element: {element}");
Memory.Dispose();