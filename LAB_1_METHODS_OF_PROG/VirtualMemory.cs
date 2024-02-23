using System.Text;

namespace LAB_1_METHODS_OF_PROG
{
    public class VirtualMemory : IDisposable
    {
        private readonly int bufferSize;
        private readonly int pageLength;
        private readonly int pageBlock; //битмап и страницы
        private const int offset = 2; //сдвиг для "VM"
        private FileStream? virtualFile { get; set; } = null;
        private Page[] MemoryBuffer;
        private string fileName = string.Empty;
        private long arraySize;

        public VirtualMemory(string fileName, long arraySize, int bufferSize = 4, int pageLength = 128)
        {
            this.fileName = fileName;
            this.arraySize = arraySize;
            this.bufferSize = bufferSize;
            this.pageLength = pageLength;
            this.pageBlock = pageLength + pageLength * sizeof(int);
            long pageCount = (long)Math.Ceiling((decimal)this.arraySize / this.pageLength);
            long size = pageCount * pageBlock;

            Console.WriteLine($"{arraySize} {bufferSize} {pageLength} {pageBlock} {pageCount} {size}");

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
                GetPageByElIndex(i * pageLength);
        }

        private void SavePage(ref Page page)
        {
            byte[] pageElements = new byte[page.BitMap.Length * sizeof(int)];
            Buffer.BlockCopy(page.ValuesModelArray, 0, pageElements, 0, pageElements.Length);

            virtualFile.Position = page.PageIndex * pageBlock + offset;
            virtualFile?.Write(page.BitMap, 0, page.BitMap.Length);
            virtualFile?.Write(pageElements, 0, pageElements.Length);
            virtualFile?.Flush();

            page.RecordInMemTime = DateTime.Now;
            page.PageMode = false;
        }

        public void SaveVirtualMemory()
        {
            for(int i=0; i< MemoryBuffer.Length; i++)
                if (MemoryBuffer[i].PageMode)
                    SavePage(ref MemoryBuffer[i]);
        }

        private void ReadPage(Page oldestPage, ref byte[] bitMap, ref byte[] valuesArrayByte)
        {
            virtualFile.Position = oldestPage.PageIndex * pageBlock + offset;
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
                byte[] tmp = [bytes[i], bytes[i+1], bytes[i+2], bytes[i+3]];
                array[j] = BitConverter.ToInt32(tmp, 0);
                j++;
            }

            return array;
        }

        private long? GetPageByElIndex(long elIndex)
        {
            if (elIndex < 0 || elIndex >= arraySize)
                return null;

            long pageNumber = elIndex / pageLength;
            Page oldestPage = MemoryBuffer[0];
            int indexOldestPage = 0;
            byte[] bitMap = new byte[pageLength];
            byte[] valuesArrayByte = new byte[pageLength * sizeof(int)];

            for (int index = 0; index < MemoryBuffer.Length; index++)
            {
                if (MemoryBuffer[index].PageIndex == pageNumber)
                    return index;
            }
            for (int index = 1; index < MemoryBuffer.Length; index++) 
            {
                if (MemoryBuffer[index].RecordInMemTime < oldestPage.RecordInMemTime)
                {
                    indexOldestPage = index;
                    oldestPage = MemoryBuffer[index];
                }
            }
            if (oldestPage.PageMode)
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

            int address = index % pageLength;
            MemoryBuffer[pageIndex].ValuesModelArray[address] = item;
            MemoryBuffer[pageIndex].BitMap[address] = 1;
            MemoryBuffer[pageIndex].RecordInMemTime = DateTime.Now;
            MemoryBuffer[pageIndex].PageMode = true;
            return true;
        }

        public bool GetElement(int index, ref int element) 
        {
            long pageIndex = GetPageByElIndex(index) ?? -1;

            if (pageIndex < 0)
                return false;

            int address = index % pageLength;
            element = MemoryBuffer[pageIndex].ValuesModelArray[address];
            return true;
        }

        public void Dispose()
        {
            SaveVirtualMemory();
            virtualFile?.Close();
            virtualFile?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
