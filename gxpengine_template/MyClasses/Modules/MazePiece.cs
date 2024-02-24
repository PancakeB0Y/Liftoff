using System;
using System.Collections.Generic;

namespace gxpengine_template.MyClasses.Modules
{
    public class MazePiece
    {
        public bool[] Exits { get; }
        public float Chance { get; }
        public PieceType Type { get; }
        public bool IsEnd { get; set; }

        MazePiece[] _neighbours;
        public int Index { get; set; }
        public MazePiece(float chance, bool[] exits, PieceType type)
        {
            Chance = chance;
            Exits = exits;
            Type = type;
        }

        private MazePiece(MazePiece prototype) 
        {
            Exits = (bool[])prototype.Exits.Clone();
            Chance = prototype.Chance;
            _neighbours = (MazePiece[])prototype._neighbours?.Clone();
            Type = prototype.Type;
        }

        public void SetNeighbours(MazePiece[] neighbours)
        {
            _neighbours = neighbours;
        }
        public void TestCheckConnections()
        {
            //check neighbours
            //check exits
            //check type
            
            for (int i = 0; i < Exits.Length; i++)
            {

                int oppositeExitIndex = i + (i < 2 ? 2 : -2);
                if (Exits[i] && _neighbours[i] != null && _neighbours[i].Exits[oppositeExitIndex])
                {
                    if (i == 0)
                        Console.WriteLine("connected left");
                    else if (i == 1)
                        Console.WriteLine("connected up");
                    else if (i == 2)
                        Console.WriteLine("connected right");
                    else
                        Console.WriteLine("connected down");
                }
            }
        }
        public void RotateRight()
        {
            var lastexit = Exits[3];
            Exits[3] = Exits[2];
            Exits[2] = Exits[1];
            Exits[1] = Exits[0];
            Exits[0] = lastexit;
        }

        public bool SearchForEnd(List<int> passedIndexes)
        {
            passedIndexes.Add(Index);

            for (int i = 0; i < Exits.Length; i++)
            {
                int oppositeExitIndex = i + (i < 2 ? 2 : -2);
                if ( Exits[i] && _neighbours[i] != null && _neighbours[i].Exits[oppositeExitIndex] )
                {
                    if (passedIndexes.Contains(_neighbours[i].Index)) continue;

                    if (_neighbours[i].IsEnd) return true;

                    if (_neighbours[i].SearchForEnd(passedIndexes))
                        return true;
                }
            }
            return false;
        }

        public MazePiece Clone()
        {
            return new MazePiece(this);
        }

    }
}
