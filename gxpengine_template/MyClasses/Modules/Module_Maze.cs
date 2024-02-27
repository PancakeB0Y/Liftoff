using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections.Generic;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public enum PieceType
    {
        Cross,
        Line,
        T,
        Corner
    }
    public class Module_Maze : Module
    {
        public event Action<MazePiece> PieceRotated;
        public MazePiece[] Pieces => _mPieces;
        public int Columns => _mColumns;
        readonly int _mColumns;
        public int Rows => _mRows;
        readonly int _mRows;

        List<int> _searchList = new List<int>();
        readonly MazePiece[] _mPieces;
        readonly MazePiece[] _mPiecesPrototypes;

        TiledObject _data;

        public Module_Maze(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            moduleType = ModuleTypes.Dpad;

            _mColumns = data.GetIntProperty("ModuleColumns", 3);
            _mRows = data.GetIntProperty("ModuleRows", 2);
            _mPieces = new MazePiece[_mColumns * _mRows];

            _mPiecesPrototypes = new MazePiece[]
            {
                new MazePiece(data.GetFloatProperty("CornerChance", 0.7f), new bool[] { false,true,true,false }, PieceType.Corner),
                new MazePiece(data.GetFloatProperty("CrossChance", .3f), new bool[] { true,true,true,true }, PieceType.Cross),
                new MazePiece(data.GetFloatProperty("LineChance", .6f), new bool[] { true,false,true,false }, PieceType.Line),
                new MazePiece(data.GetFloatProperty("TChance", .5f), new bool[] { true,false,true,true }, PieceType.T)
            };

            do
            {
                CreateRandomPieces(_mPiecesPrototypes);
            }
            while (IsPossiblePath());

            var selector = new Module_Maze_Selector(this);
            var visual = new Module_Maze_Visual(this, data, selector);
            AddChild(visual);
            AddChild(selector);

            _data = data;
        }

        override public object Clone()
        {
            var clone = new Module_Maze(texture.filename, _cols, _rows, _data);

            return clone;
        }

        public void RotatePiece(int index)
        {
            MazePiece piece = _mPieces[index];
            piece.RotateRight();
            PieceRotated?.Invoke(piece);

        }

        public void CheckPath()
        {
            if (IsPossiblePath())
                RaiseSuccesEvent();
        }

        protected override void OnTimeEnd()
        {
            RaiseFailEvent();
        }

        bool IsPossiblePath()
        {
            _searchList.Clear();

            return _mPieces[0].Exits[0] && _mPieces[_mPieces.Length - 1].Exits[2] && _mPieces[0].SearchForEnd(_searchList);
        }

        void CreateRandomPieces(MazePiece[] piecesPrototype)
        {
            Random randomGenerator = new Random(Time.time);

            Array.Clear(_mPieces, 0, _mPieces.Length);

            for (int i = 0; i < _mPieces.Length; i++)
            {
                //do every piece except line
                while (_mPieces[i] == null)//to guarantee that every piece is set
                {
                    var protoPiece = piecesPrototype[randomGenerator.Next(0, piecesPrototype.Length)];

                    if (randomGenerator.NextDouble() >= protoPiece.Chance) continue;

                    if (_mPieces.IndexIsOnGridCorner(_mColumns, i))
                    {
                        if ((i == 0 || i == _mPieces.Length - 1) && protoPiece.Type == PieceType.Corner) continue;
                        if (protoPiece.Type == PieceType.Line) continue;

                        _mPieces[i] = protoPiece.Clone();
                        break;
                    }
                    else if (_mPieces.Length % 2 != 0 && i == (int)(_mPieces.Length / 2f))
                    {
                        if (protoPiece.Type == PieceType.Corner) continue;

                        _mPieces[i] = protoPiece.Clone();
                        break;
                    }
                    else
                    {
                        _mPieces[i] = protoPiece.Clone();
                        break;
                    }

                }
                _mPieces[i].Index = i;

            }

            _mPieces[_mPieces.Length - 1].IsEnd = true;

            SetNeighboursOfPieces();

        }

        void SetNeighboursOfPieces()
        {
            for (int i = 0; i < _mPieces.Length; i++)
            {
                MazePiece[] neighbours = new MazePiece[4];

                if (i > 0 && i % _mColumns != 0)//left neighbour
                    neighbours[0] = _mPieces[i - 1];

                if (i >= _mColumns)//up neighbour
                    neighbours[1] = _mPieces[i - _mColumns];

                if (i < _mPieces.Length - 1 && (i + 1) % _mColumns != 0)//right neighbour
                    neighbours[2] = _mPieces[i + 1];

                if (i < _mPieces.Length - _mColumns)//down neighbour
                    neighbours[3] = _mPieces[i + _mColumns];


                _mPieces[i].SetNeighbours(neighbours);
            }
        }

    }
}
