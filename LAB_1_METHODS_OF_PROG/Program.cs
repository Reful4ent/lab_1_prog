// See https://aka.ms/new-console-template for more information
using LAB_1_METHODS_OF_PROG;

VirtualMemory Memory = new("dsfsd.bin", 21,4,4);
int element = 0;
Console.WriteLine($"Result: {Memory.GetElement(0, ref element)}, element: {element}");
Memory.SetElement(0, 2123232);
Console.WriteLine($"Result: {Memory.GetElement(0, ref element)}, element: {element}");
Console.WriteLine($"Result: {Memory.GetElement(19, ref element)}, element: {element}");
Memory.SetElement(19, 323);
Console.WriteLine($"Result: {Memory.GetElement(19, ref element)}, element: {element}");
Console.WriteLine($"Result: {Memory.GetElement(11, ref element)}, element: {element}");
Memory.SetElement(11, 432);
Console.WriteLine($"Result: {Memory.GetElement(11, ref element)}, element: {element}");
Console.WriteLine($"Result: {Memory.GetElement(19, ref element)}, element: {element}");
Memory.SetElement(19, 867);
Console.WriteLine($"Result: {Memory.GetElement(19, ref element)}, element: {element}");
Memory.Dispose();

