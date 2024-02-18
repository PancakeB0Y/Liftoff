using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TiledMapParser;

namespace gxpengine_template.MyClasses
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
        public event Action<MazePiece> OnPieceRotate;
        public MazePiece[] Pieces => _mPieces;
        public int Columns => _mColumns;
        readonly int _mColumns;
        public int Rows => _mRows;
        readonly int _mRows;

        readonly MazePiece[] _mPieces;
        readonly MazePiece[] _mPiecesPrototypes;
        readonly TiledObject _data;

        public Module_Maze(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            _mColumns = data.GetIntProperty("ModuleColumns", 3);
            _mRows = data.GetIntProperty("ModuleRows", 2);
            _data = data;
            _mPieces = new MazePiece[_mColumns * _mRows];
            alpha = 0;

            _mPiecesPrototypes = new MazePiece[]
            {
                new MazePiece(data.GetFloatProperty("CornerChance", 1f), new bool[] { false,true,true,false }, PieceType.Corner),
                new MazePiece(data.GetFloatProperty("CrossChance", 0f), new bool[] { true,true,true,true }, PieceType.Cross),
                new MazePiece(data.GetFloatProperty("LineChance", 1f), new bool[] { true,false,true,false }, PieceType.Line),
                new MazePiece(data.GetFloatProperty("TChance", 1f), new bool[] { true,false,true,true }, PieceType.T)
            };

            do
            {
                CreateRandomPieces(_mPiecesPrototypes);
            } 
            while (IsPossiblePath());


            var visual = new Module_Maze_Visual(this, data);
            
            AddChild(visual);
            AddChild(new Coroutine(TestCreateMaze()));

        }

        public bool IsPossiblePath()
        {
            //list can be used to color the found path
            return _mPieces[0].SearchForEnd(new List<int>());
        }

        public void RotatePieceAndCheckPath(int index)
        {
            MazePiece piece = _mPieces[index];
            piece.RotateRight();
            OnPieceRotate?.Invoke(piece);

            if ( IsPossiblePath() )
                RaiseSuccesEvent();
        }
        protected override void OnTimeEnd()
        {
            RaiseFailEvent();
        }

        #region Testing

        IEnumerator TestCreateMaze()
        {
            while (true)
            {
                

                //CreateRandomPieces(_mPiecesPrototypes);
                ////CreatePieces(_mPiecesPrototypes);

                //var visual = new Module_Maze_Visual(this, _data);
                //AddChild(visual);
                //Console.WriteLine("beffore rotation " + IsPossiblePath());

                ////_mPieces[4].TestCheckConnections();
                //visual._pieces[1].color = (uint)Color.Red.ToArgb();
                //RotatePiece(1);
                //visual._pieces[1].Turn(90);

                //Console.WriteLine("after " + IsPossiblePath());

                yield return new WaitForSeconds(5f);

                Console.WriteLine("Repeat");

                foreach (var item in GetChildren())
                {
                    if (!(item is Coroutine))
                        item.Destroy();
                }
                Array.Clear(_mPieces, 0, _mPieces.Length);
                CreateRandomPieces( _mPiecesPrototypes );
                //CreatePieces(_mPiecesPrototypes);
                Console.WriteLine(IsPossiblePath());

                var visual = new Module_Maze_Visual(this, _data);
                AddChild(visual);

                RotatePieceAndCheckPath(1);
                visual._pieces[1].color = (uint)Color.Red.ToArgb();
                visual._pieces[1].Turn(90);

                Console.WriteLine(IsPossiblePath());
            }
        }
        //corner cross line t
        void CreatePieces(MazePiece[] pieces)
        {
            _mPieces[0] = pieces[0].Clone();
            _mPieces[1] = pieces[3].Clone();
            _mPieces[2] = pieces[3].Clone();
            _mPieces[3] = pieces[0].Clone();
            _mPieces[4] = pieces[3].Clone();
            _mPieces[5] = pieces[0].Clone();

            _mPieces[5].IsEnd = true;
            for (int i = 0; i < _mPieces.Length; i++)
            {
                _mPieces[i].Index = i;
            }

            SetNeighboursOfPieces();
        }
        #endregion

        void CreateRandomPieces(MazePiece[] piecesPrototype)
        {
            Random randomGenerator = new Random(Time.time);

            for (int i = 0; i < _mPieces.Length; i++)
            {
                //do every piece except line
                while (_mPieces[i] == null)//to guarantee that every piece is set
                {
                    var protoPiece = piecesPrototype[randomGenerator.Next(0, piecesPrototype.Length)];

                    if (randomGenerator.NextDouble() >= protoPiece.Chance) continue;

                    if (_mPieces.IndexIsOnGridCorner(_mColumns, i))
                    {
                        if (protoPiece.Type != PieceType.Line)
                        {
                            _mPieces[i] = protoPiece.Clone();
                            break;
                        }
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
