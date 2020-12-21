using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowSnapper.Helpers
{
    internal static class ProcessInfoHelper
    {
        public static List<List<Tuple<int, int>>> diagonals = new List<List<Tuple<int, int>>>
        {
            { new List<Tuple<int, int>>{ Tuple.Create(-1, +1), Tuple.Create(+1, -1) } },
            { new List<Tuple<int, int>>{ Tuple.Create(-1, -1), Tuple.Create(+1, +1) } }
        };

        public static bool SelectedThreeQuadrants(this List<Tuple<int, int>> coords)
        {
            return coords.Count == 3;
        }
    }
}
