using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LAB_1_METHODS_OF_PROG
{
    public class VirtualMemory : IDisposable
    {
        private readonly int bufferSize;
        private readonly int pageLenght;
        private readonly int pageBlock;
        private const int offset = 2;
        protected FileStream? virtualFile { get; set; } = null;
        protected Page[] MemoryBuffer;
        protected string fileName=string.Empty;
        protected long arraySize;
        public VirtualMemory(string fileName, long arraySize, int bufferSize = 4, int pageLenght=128)
        {
            this.fileName = fileName;
            this.arraySize = arraySize;
            this.bufferSize = bufferSize;
            this.pageLenght = pageLenght;
            this.pageBlock = pageLenght + pageLenght * sizeof(int);

            long pageCount = (long)Math.Ceiling((decimal)this.arraySize/ this.pageLenght);
            long size = pageCount * pageBlock;
            Console.WriteLine($"{size},{arraySize},{this.pageLenght},{pageCount},{pageBlock}");
            if (!File.Exists(fileName)) 
            {
                virtualFile = new FileStream(fileName,FileMode.CreateNew,FileAccess.ReadWrite);
                virtualFile.Write(Encoding.UTF8.GetBytes("VM"));
                byte[] byteArray = new byte[size];
                virtualFile.Write(byteArray,0, byteArray.Length);
                virtualFile.Flush();
            }
            else
                virtualFile = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            Page page = new(0,Array.Empty<byte>(),Array.Empty<int>());
            Array.Resize(ref MemoryBuffer, this.bufferSize);
            for (int i = 0; i < bufferSize; i++)
                MemoryBuffer[i] = page;
            for (int i = 0; i <= bufferSize; i++)
            {
                GetPageByElIndex(i * pageLenght);
            }
        }
        private void SavePage(ref Page page)
        {
            
            virtualFile.Position = page._pageIndex * pageBlock + offset;
            virtualFile?.Write(page._bitMap, 0, page._bitMap.Length);
            byte[] pageElements = new byte[page._bitMap.Length * sizeof(int)];
            Buffer.BlockCopy(page._valuesModelArray,0,pageElements,0,pageElements.Length);
            virtualFile?.Write(pageElements, 0, pageElements.Length);
            virtualFile?.Flush();
            page._recordInMemTime = DateTime.Now;
            page._pageMode = false;
        }
        public void SaveVirtualMemory()
        {
            for(int i=0; i< MemoryBuffer.Length; i++)
                if (MemoryBuffer[i]._pageMode)
                    SavePage(ref MemoryBuffer[i]);
        }
        private void ReadPage(Page oldestPage,ref byte[] bitMap, ref byte[] valuesArrayByte)
        {
            virtualFile.Position = oldestPage._pageIndex * pageBlock + offset;
            virtualFile?.Read(bitMap, 0, bitMap.Length);
            virtualFile?.Read(valuesArrayByte, 0, valuesArrayByte.Length);
            virtualFile?.Flush();
        }

        private int[] ByteToInt(byte[] bytes)
        {
            int[] array = new int[bytes.Length/sizeof(int)];
            int j = 0;
            for(int i=0; i < bytes.Length; i+=4)
            {
                byte[] temp = [bytes[i], bytes[i+1], bytes[i+2], bytes[i+3]];
                array[j] = BitConverter.ToInt32(temp, 0);
                j++;
            }
            return array;
        }

        private long? GetPageByElIndex(long elIndex)
        {
            if (elIndex < 0 || elIndex >= arraySize)
                return null;

            long pageNumber = elIndex / pageLenght;
            Page oldestPage = MemoryBuffer[0];
            int indexOldestPage = 0;
            byte[] bitMap = new byte[pageLenght];
            byte[] valuesArrayByte = new byte[pageLenght * sizeof(int)];

            for (int index = 0; index < MemoryBuffer.Length; index++)
            {
                if (MemoryBuffer[index]._pageIndex == pageNumber)
                    return index;
            }
            for (int index = 1; index < MemoryBuffer.Length; index++) 
            {
                if (MemoryBuffer[index]._recordInMemTime < oldestPage._recordInMemTime)
                {
                    indexOldestPage = index;
                    oldestPage = MemoryBuffer[index];
                }
            }
            if (oldestPage._pageMode)
                SavePage(ref oldestPage);
            ReadPage(oldestPage, ref bitMap,ref valuesArrayByte);
            int[] valuesArray = ByteToInt(valuesArrayByte);
            MemoryBuffer[indexOldestPage] = new Page(pageNumber, bitMap, valuesArray);
            return indexOldestPage;
        }

        public bool SetElement(int index, int item)
        {
            long pageIndex = GetPageByElIndex(index) ?? -1;
            if (pageIndex < 0)
                return false;
            int address = index % pageLenght;
            MemoryBuffer[pageIndex]._valuesModelArray[address] = item;
            MemoryBuffer[pageIndex]._bitMap[address] = 1;
            MemoryBuffer[pageIndex]._recordInMemTime = DateTime.Now;
            MemoryBuffer[pageIndex]._pageMode = true;
            return true;
        }

        public bool GetElement(int index, ref int element) 
        {
            long pageIndex = GetPageByElIndex(index) ?? -1;
            if (pageIndex < 0)
                return false;
            int address = index % pageLenght;
            element = MemoryBuffer[pageIndex]._valuesModelArray[address];
            return true;
        }

        public void Dispose()
        {
            SaveVirtualMemory();

            virtualFile?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
