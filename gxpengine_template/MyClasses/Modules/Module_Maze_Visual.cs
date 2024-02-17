using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Maze_Visual : GameObject
    {
        readonly Module_Maze _mazeLogic;

        public AnimationSprite[] _pieces;

        public Module_Maze_Visual(Module_Maze mazeLogic, TiledObject data)
        {
            _mazeLogic = mazeLogic;

            _pieces = new AnimationSprite[_mazeLogic.Pieces.Length];
            int ssColumns = data.GetIntProperty("MazePiecesSpriteSheet_Columns");
            int ssRows = data.GetIntProperty("MazePiecesSpriteSheet_Rows");
            string pieceSpriteSheetPath = data.GetStringProperty("MazePiecesSpriteSheet");

            for (int i = 0; i < _pieces.Length; i++)
            {
                _pieces[i] = PieceFactory(_mazeLogic.Pieces[i].Type, pieceSpriteSheetPath, ssColumns, ssRows);
                //position Piece
                var piece = _pieces[i];
                piece.SetXY(piece.height * (i % _mazeLogic.Columns), piece.width * (i / _mazeLogic.Columns));
                piece.SetOrigin(piece.width/2,piece.height/2);
            }
        }

        void RotatePiece(MazePiece piece)
        {
            _pieces[piece.Index].Turn(90);
        }

        AnimationSprite PieceFactory(PieceType type, string pieceSpriteSheetPath, int ssColumns, int ssRows)
        {
            var piece = new AnimationSprite(pieceSpriteSheetPath, ssColumns, ssRows, -1, true, false);
            AddChild(piece);
            switch (type)
            {
                case PieceType.Cross:
                    piece.SetFrame(0);
                break;
                case PieceType.Line:
                    piece.SetFrame(1);
                break;
                case PieceType.T:
                    piece.SetFrame(2);
                break;
                case PieceType.Corner:
                    piece.SetFrame(3);
                break;

            }

            return piece;
        }

    }
}
