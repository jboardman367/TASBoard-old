using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    class LibTASReader : IMovieReader
    {
        public IEnumerable<List<string>> Frames => throw new NotImplementedException();

        public MovieSettings MovieSettings => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public LibTASReader(string fname)
        {

        }
    }
}
