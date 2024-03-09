using System.Text;

namespace LAB_1_METHODS_OF_PROG
{
    /// <summary>
    /// Класс для управления виртуальной памятью.
    /// </summary>
    public class VirtualMemory : IDisposable
    {

        private readonly int bufferSize;
        private readonly int pageLength;

        /// <summary>
        /// Размер страницы с битовой картой в файле.
        /// </summary>
        private readonly int pageBlock;

        /// <summary>
        /// Константа отступа в файле для записи сигнатуры.
        /// </summary>
        private const int offset = 2;

        /// <summary>
        /// Поток ввода.
        /// </summary>
        private FileStream? virtualFile { get; set; } = null;
        private Page[] memoryBuffer;
        private string fileName = string.Empty;
        private long arraySize;

        public VirtualMemory(string fileName, long arraySize, int bufferSize = 4, int pageLength = 128)
        {
            this.fileName = fileName;
            this.arraySize = arraySize;
            this.bufferSize = bufferSize;
            this.pageLength = pageLength;
            this.pageBlock = pageLength + pageLength * sizeof(int);
            // Округление количества страниц (чтобы файл не обрезался).
            long pageCount = (long)Math.Ceiling((decimal)this.arraySize / this.pageLength);
            long size = pageCount * pageBlock;
            
            // Проверка на существование файла.
            if (!File.Exists(fileName)) 
            {
                virtualFile = new FileStream(fileName,FileMode.CreateNew,FileAccess.ReadWrite);
                // Запись сигнатуры в файл.
                virtualFile.Write(Encoding.UTF8.GetBytes("VM"));
                byte[] byteArray = new byte[size];
                // Заполнение файла нулями.
                virtualFile.Write(byteArray, 0, byteArray.Length);
                virtualFile.Flush();
            }
            else
                virtualFile = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            Page page = new(0,Array.Empty<byte>(),Array.Empty<int>());

            // Расширение буфера памяти по размеру заданного буфера.
            Array.Resize(ref memoryBuffer, this.bufferSize);

            // Заполнение буфера пустыми страницами.
            for (int i = 0; i < bufferSize; i++)
                memoryBuffer[i] = page;
            for (int i = 0; i <= bufferSize; i++)
                GetPageByElIndex(i * pageLength);
        }

        /// <summary>
        /// Сохранение страницы.
        /// </summary>
        /// <param name="page"> Страница. </param>
        private void SavePage(ref Page page)
        {
            // Получение элементов на странице из буфера.
            byte[] pageElements = new byte[page.BitMap.Length * sizeof(int)];
            Buffer.BlockCopy(page.ValuesModelArray, 0, pageElements, 0, pageElements.Length);

            // Запись страницы в файл.
            virtualFile.Position = page.PageIndex * pageBlock + offset;
            virtualFile?.Write(page.BitMap, 0, page.BitMap.Length);
            virtualFile?.Write(pageElements, 0, pageElements.Length);
            virtualFile?.Flush();

            // Изменение свойств страницы.
            page.RecordInMemTime = DateTime.Now;
            page.PageMode = false;
        }

        /// <summary>
        /// Сохранение всех страниц.
        /// </summary>
        public void SaveVirtualMemory()
        {
            for(int i=0; i< memoryBuffer.Length; i++)
                if (memoryBuffer[i].PageMode)
                    SavePage(ref memoryBuffer[i]);
        }

        /// <summary>
        /// Чтение данных со страницы из файла.
        /// </summary>
        /// <param name="oldestPage"> Самая старая страница, которая была изменена. </param>
        /// <param name="bitMap"> Битовая карта. </param>
        /// <param name="valuesArrayByte"> Массив значений моделируемого массива, находящихся на странице. </param>
        private void ReadPage(Page oldestPage, ref byte[] bitMap, ref byte[] valuesArrayByte)
        {
            virtualFile.Position = oldestPage.PageIndex * pageBlock + offset;
            virtualFile?.Read(bitMap, 0, bitMap.Length);
            virtualFile?.Read(valuesArrayByte, 0, valuesArrayByte.Length);
            virtualFile?.Flush();
        }

        /// <summary>
        /// Перевод массива байтов в массив целочисленных значений.
        /// </summary>
        /// <param name="bytes"> Массив байтов. </param>
        /// <returns> Массив целочисленных значений. </returns>
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

        /// <summary>
        /// Определение номера (индекса) страницы в буфере страниц, где находится элемент массива с заданным индексом.
        /// </summary>
        /// <param name="elIndex"> Индекс элемента. </param>
        /// <returns> Индекс страницы. </returns>
        private long? GetPageByElIndex(long elIndex)
        {
            // Проверка индекса элемента на выход за диапазон.
            if (elIndex < 0 || elIndex >= arraySize)
                return null;

            long pageNumber = elIndex / pageLength;
            Page oldestPage = memoryBuffer[0];
            int indexOldestPage = 0;
            byte[] bitMap = new byte[pageLength];
            byte[] valuesArrayByte = new byte[pageLength * sizeof(int)];

            for (int index = 0; index < memoryBuffer.Length; index++)
            {
                if (memoryBuffer[index].PageIndex == pageNumber)
                    return index;
            }
            for (int index = 1; index < memoryBuffer.Length; index++) 
            {
                if (memoryBuffer[index].RecordInMemTime < oldestPage.RecordInMemTime)
                {
                    indexOldestPage = index;
                    oldestPage = memoryBuffer[index];
                }
            }

            if (oldestPage.PageMode)
                SavePage(ref oldestPage);
            ReadPage(oldestPage, ref bitMap, ref valuesArrayByte);

            int[] valuesArray = ByteToInt(valuesArrayByte);
            memoryBuffer[indexOldestPage] = new Page(pageNumber, bitMap, valuesArray);
            return indexOldestPage;
        }

        /// <summary>
        /// Запись заданного значения в элемент массива с указанным индексом.
        /// </summary>
        /// <param name="index"> Индекс элемента. </param>
        /// <param name="item"> Элемент. </param>
        /// <returns> True - удачное завершение. False - неудачное завершение. </returns>
        public bool SetElement(int index, int item)
        {
            long pageIndex = GetPageByElIndex(index) ?? -1;

            if (pageIndex < 0)
                return false;

            int address = index % pageLength;
            memoryBuffer[pageIndex].ValuesModelArray[address] = item;
            memoryBuffer[pageIndex].BitMap[address] = 1;
            memoryBuffer[pageIndex].RecordInMemTime = DateTime.Now;
            memoryBuffer[pageIndex].PageMode = true;
            return true;
        }

        /// <summary>
        /// Чтение значения элемента массива с заданным индексом в указанную переменную.
        /// </summary>
        /// <param name="index"> Индекс элемента. </param>
        /// <param name="element"> Элемент. </param>
        /// <returns></returns>
        public bool GetElement(int index, ref int element) 
        {
            long pageIndex = GetPageByElIndex(index) ?? -1;

            if (pageIndex < 0)
                return false;

            int address = index % pageLength;
            element = memoryBuffer[pageIndex].ValuesModelArray[address];
            return true;
        }

        /// <summary>
        /// Освобождение неуправляемой памяти.
        /// </summary>
        public void Dispose()
        {
            SaveVirtualMemory();
            virtualFile?.Close();
            virtualFile?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
