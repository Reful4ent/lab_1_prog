namespace LAB_1_METHODS_OF_PROG
{
    /// <summary>
    /// Структура страницы, находящейся в памяти.
    /// </summary>
    public struct Page
    {
        /// <summary>
        /// Абсолютный номер страницы.
        /// </summary>
        public long PageIndex { get; set; }

        /// <summary>
        /// Статус страницы.
        /// </summary>
        public bool PageMode { get; set; }

        /// <summary>
        /// Время записи страницы в память.
        /// </summary>
        public DateTime RecordInMemTime { get; set; }

        /// <summary>
        /// Битовая карта страницы.
        /// </summary>
        public byte[] BitMap { get; set; }

        /// <summary>
        /// Массив значений моделируемого массива, находящихся на странице.
        /// </summary>
        public int[] ValuesModelArray { get; set; }

        public Page(long pageIndex, byte[] bitArray, int[] valuesModelArray)
        {
            // Проверка на неотрицательный номер страницы.
            if (pageIndex < 0)
                throw new Exception("Страница не может быть меньше 0");
            PageIndex = pageIndex;
            PageMode = false;
            RecordInMemTime = DateTime.Now;
            BitMap = bitArray;
            ValuesModelArray = valuesModelArray;
        }
    }
}