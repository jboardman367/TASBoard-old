using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    public static class Extensions
    {
        public static InputFrame Sum(this IEnumerable<InputFrame> inputFrames)
        {
            return inputFrames.Aggregate((a, b) => a + b);
        }
    }
}
