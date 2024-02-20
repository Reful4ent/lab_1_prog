using System;
using System.Collections.Generic;
using System.Linq;
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
        protected Page[] Buffer;
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
            long size = pageCount * pageLenght;

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
            Array.Resize(ref Buffer, bufferSize);
            for (int i = 0; i < bufferSize; i++)
                Buffer[i] = page;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
