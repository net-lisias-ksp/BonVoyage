/*
	This file is part of Bon Voyage /L
		© 2024 LisiasT : http://lisias.net <support@lisias.net>
		© 2018-2024 Maja
		© 2016-2018 RealGecko

	Bon Voyage /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Bon Voyage /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Bon Voyage /L. If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections.Generic;

namespace BonVoyage
{
    internal class Hex : IHasNeighbours<Hex>
    {
        internal double Latitude { get; }
        internal double Longitude { get; }
        internal double Altitude { get; }
        internal double Bearing { get; }
        internal int X { get; }
        internal int Y { get; }

        private PathFinder Parent;

        internal Hex(double latitude, double longitude, double altitude, double bearing, int x, int y, PathFinder parent)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Bearing = bearing;
            X = x;
            Y = y;
            Parent = parent;
        }

        public IEnumerable<Hex> Neighbours
        {
            get
            {
                return Parent.GetNeighbours(X, Y);
            }
        }

    }

}