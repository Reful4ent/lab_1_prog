using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_1_METHODS_OF_PROG
{
    public class VirtualMemory : IDisposable
    {
        protected FileStream? virtualFile { get; set; } = null;
        protected Page[] pages;
        protected string fileName=string.Empty;
        protected long arraySize;

        public VirtualMemory(string fileName, long arraySize)
        {
            this.fileName=fileName;
            this.arraySize=arraySize;
            if(!File.Exists(fileName)) 
            {
                virtualFile = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter stream = new StreamWriter(fileName);
                stream.Write('V');
                stream.Write('M');
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
