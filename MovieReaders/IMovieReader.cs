using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    interface IMovieReader : IEnumerable<InputFrame>
    {
        void Close();

        MovieProperties MovieProperties { get; }
    }

    public struct MovieProperties
    {
        public readonly int FramerateNum;
        public readonly int FramerateDen;
        public readonly int Length;
        public readonly bool IsVariableFramerate;
        public MovieProperties(
            int framerateNum,
            int framerateDen, 
            int length,
            bool isVariableFramerate)
        {
            FramerateNum = framerateNum;
            FramerateDen = framerateDen;
            Length = length;
            IsVariableFramerate = isVariableFramerate;
        }
    }

    public struct InputFrame
    {
        public readonly List<string> Inputs;
        public readonly Fraction TimeDelta;

        public InputFrame(List<string> inputs, Fraction timeDelta)
        {
            Inputs = inputs;
            TimeDelta = timeDelta;
        }
    }
}
