using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace LAB_1_METHODS_OF_PROG
{
    public struct Page
    {
        public long _pageIndex { get; set; }
        public bool _pageMode { get; set; }
        public DateTime _recordInMemTime { get; set; }
        public byte[] _bitMap { get; set; }
        public int[] _valuesModelArray { get; set; }

        public Page(long pageIndex, byte[] bitArray, int[] valuesModelArray)
        {
            if (pageIndex < 0)
                throw new Exception("Страница не может быть меньше 0");
            _pageIndex = pageIndex;
            _pageMode = false;
            _recordInMemTime = DateTime.Now;
            _bitMap = bitArray;
            _valuesModelArray = valuesModelArray;
        }
    }
}