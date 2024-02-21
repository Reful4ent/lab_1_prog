using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace LAB_1_METHODS_OF_PROG
{
    public struct Page
    {
        public long PageIndex { get; set; }
        public bool PageMode { get; set; }
        public DateTime RecordInMemTime { get; set; }
        public byte[] BitMap { get; set; }
        public int[] ValuesModelArray { get; set; }

        public Page(long pageIndex, byte[] bitArray, int[] valuesModelArray)
        {
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