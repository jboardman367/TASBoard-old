using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    interface IMovieReader
    {
        void Close();

        IEnumerable<List<string>> Frames { get; }

        MovieSettings MovieSettings { get; }
    }

    struct MovieSettings
    {
        int Num;
        int Den;
        public MovieSettings(int num, int den)
        {
            Num = num;
            Den = den;
        }
    }
}
