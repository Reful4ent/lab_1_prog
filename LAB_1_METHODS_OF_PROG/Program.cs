// See https://aka.ms/new-console-template for more information
using LAB_1_METHODS_OF_PROG;

VirtualMemory mem = new("dsfsd.bin", 21,4,4);
mem.SetElement(0, 3);
mem.SetElement(1, 2);
mem.SetElement(5, 4);
mem.SaveVirtualMemory();

