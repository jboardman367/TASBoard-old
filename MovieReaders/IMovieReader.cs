using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    interface IMovieReader : IEnumerable
    {
        void Close();

        MovieSettings MovieSettings { get; }
    }

    public struct MovieSettings
    {
        int? Num;
        int? Den;
        public MovieSettings(int? num, int? den)
        {
            Num = num;
            Den = den;
        }
    }
}
