using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.Datastructures
{
    //Warning : data of type T should not have colliding hashes
    public class SearchGrid<T> : Dictionary<int, Dictionary<int, List<T>>>
    {
        public double Left { get; set; }
        public double Bottom { get; set; }

        public double GridSizeX { get; set; }
        public double GridSizeY { get; set; }

        public int CellCountX { get; set; }
        public int CellCountY { get; set; }

        public SearchGrid(double left, double bottom, double gridSizeX, double gridSizeY, int cellCountX, int cellCountY) : base()
        {
            Left = left;
            Bottom = bottom;
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            CellCountX = cellCountX;
            CellCountY = cellCountY;

            //Initialize dictionary entries
            for (int i = 0; i < cellCountX; i++)
            {
                this[i] = new Dictionary<int, List<T>>();
                for (int j = 0; j < cellCountY; j++)
                {
                    this[i][j] = new List<T>();
                }
            }

        }

        public int[] GetGridCellOfPoint(double x, double y)
        {
            if (!(this.Bottom < y) || !(y < (this.Bottom + CellCountY * GridSizeY)) ||
                !(this.Left < x) || !(x < (this.Left + CellCountX * GridSizeX)))
            {
                throw new ArgumentException("Values outside of grid range");
            }

            int gridCellY = (int)((y - Bottom) / GridSizeY);
            int gridCellX = (int)((x - Left) / GridSizeX);
            return new int[2] { gridCellX, gridCellY };
        }

        public List<T> GetNearbyValues(double queryX, double queryY)
        {
            // TO DO: Assumes hashes don't collide for type T. Should implement IEquatable?
            var result = new HashSet<T>();
            int[] gridcell = GetGridCellOfPoint(queryX, queryY);
            List<int[]> cells = GetSurroundingCells(gridcell, CellCountX, CellCountY);
            foreach (var cell in cells)
            {
                var valuesInCell = this[cell[0]][cell[1]];
                result.UnionWith(valuesInCell);
            }
            return result.ToList();
        }

        private List<int[]> GetSurroundingCells(int[] cell, int gridDimensionX, int gridDimensionY)
        {
            List<int[]> result = new List<int[]>();
            int x = cell[0];
            int y = cell[1];
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (-1 < i && i < gridDimensionX && -1 < j && j < gridDimensionY)
                    {
                        result.Add(new int[] { i, j });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Finds all cells lying on the straight line connecting a pair of cells in a grid.
        /// [ I stole this code online ]
        /// </summary>
        /// <param name="x0"> x co-ordinate of the first cell </param>
        /// <param name="y0"> y co-ordinate of the first cell </param>
        /// <param name="x1"> x co-ordinate of the second cell</param>
        /// <param name="y1"> y co-ordinate of the second cell</param>
        /// <remarks> Increasing y goes from top to bottom. Increasing x goes from left to right.
        /// </remarks>
        /// <returns> List of cells lying on the straight line connecting the two cells. </returns>
        public List<int[]> Bresenham(int x0, int y0, int x1, int y1)
        {
            //TODO: verify that parameters specify valid cell
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            List<int[]> returnList = new List<int[]>();
            for (int x = x0; x <= x1; x++)
            {
                //Point((steep ? y : x), (steep ? x : y));
                returnList.Add(new int[2] { steep ? y : x, steep ? x : y });
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            return returnList;
        }
    }
}
