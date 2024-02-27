using GXPEngine;
using GXPEngine.Core;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Maze_Visual : GameObject
    {

        readonly Module_Maze _mazeLogic;
        readonly Module_Maze_Selector _mazeSelector;
        readonly AnimationSprite[] _pieces;
        readonly Func<float, float> _easeFunc;
        readonly Pivot _container;
        readonly int _rotationTimeMs;
        Tween _rotationTween;

        public Module_Maze_Visual(Module_Maze mazeLogic, TiledObject data, Module_Maze_Selector mazeSelector)
        {
            _mazeLogic = mazeLogic;
            _mazeSelector = mazeSelector;

            int ssColumns = data.GetIntProperty("MazePiecesSpriteSheet_Columns");
            int ssRows = data.GetIntProperty("MazePiecesSpriteSheet_Rows");
            string pieceSpriteSheetPath = data.GetStringProperty("MazePiecesSpriteSheet");
            _easeFunc = EaseFuncs.Factory(data.GetStringProperty("RotationEaseFunc", "EaseOutBack"));
            _rotationTimeMs = data.GetIntProperty("RotationTimeMs", 500);

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            _pieces = new AnimationSprite[_mazeLogic.Pieces.Length];

            _mazeLogic.PieceRotated += RotatePiece;
            _mazeSelector.SelectionChanged += OnSelectionChange;

            AddChild(new Coroutine(Init(pieceSpriteSheetPath, ssColumns,ssRows, data)));

        }

        IEnumerator Init(string pieceSpriteSheetPath, int ssColumns, int ssRows, TiledObject data)
        {
            yield return null;

            var paddingL = data.GetFloatProperty("PaddingL",20) * _mazeLogic.width;
            var paddingR = data.GetFloatProperty("PaddingR",20) * _mazeLogic.width;
            var paddingY = data.GetFloatProperty("PaddingY",20) * _mazeLogic.height;
            var spacingX = data.GetFloatProperty("SpacingX",3) * _mazeLogic.width;
            var spacingY = data.GetFloatProperty("SpacingY",3) * _mazeLogic.height;

            Vector2 pos = new Vector2(_mazeLogic.x,_mazeLogic.y);

            _mazeLogic.SetOrigin(0, 0);
            _mazeLogic.SetXY(pos.x, pos.y);
            _mazeLogic.alpha = 1f;
            _container.SetXY(_mazeLogic.x, _mazeLogic.y);

            int mazeColumns = _mazeLogic.Columns;
            int pieceW = Mathf.Floor((_mazeLogic.width  - paddingL - paddingR - (spacingX * (mazeColumns - 1))) / mazeColumns);
            int pieceH = Mathf.Floor((_mazeLogic.height - paddingY - (spacingY * (_mazeLogic.Rows - 1))) / _mazeLogic.Rows );
            Vector2 offset = new Vector2(pieceW * .5f + paddingL, (pieceH + paddingY) * .5f);
            Vector2 currSpacing = new Vector2();

            for (int i = 0; i < _pieces.Length; i++)
            {
                var piece = PieceFactory(_mazeLogic.Pieces[i].Type, pieceSpriteSheetPath, ssColumns, ssRows);
                piece.SetOrigin(piece.width / 2, piece.height / 2);
                _container.AddChild(piece);
                _pieces[i] = piece;

                piece.width = pieceW;
                piece.height = pieceH;

                currSpacing.x = spacingX * (i % mazeColumns);
                currSpacing.y = spacingY * (i / mazeColumns);
                piece.SetXY(pieceW * (i % mazeColumns) + offset.x + currSpacing.x, pieceH * (i / mazeColumns) + offset.y + currSpacing.y);
            }

        }
        void Update()
        {
            //_container.SetXY(_mazeLogic.x, _mazeLogic.y);
        }
        private void OnSelectionChange(int prev, int curr)
        {
            _pieces[prev].SetColor(1, 1, 1);
            _pieces[curr].SetColor(0, .7f, 0);
        }

        protected override void OnDestroy()
        {
            _mazeLogic.PieceRotated -= RotatePiece;
            _mazeSelector.SelectionChanged -= OnSelectionChange;
            _container.Destroy();
        }

        void RotatePiece(MazePiece piece)
        {
            var visualOfPiece = _pieces[piece.Index];
            _rotationTween?.Destroy();
            var copy = visualOfPiece.rotation;

            _rotationTween = new Tween(TweenProperty.rotation, _rotationTimeMs, 90, _easeFunc).
                OnCompleted(
                () =>
                {
                    _mazeLogic.CheckPath();
                }
                ).
                OnExit(
                ()=>
                    visualOfPiece.rotation = copy + 90
                );
            visualOfPiece.AddChild(_rotationTween);
        }

        AnimationSprite PieceFactory(PieceType type, string pieceSpriteSheetPath, int ssColumns, int ssRows)
        {
            var piece = new AnimationSprite(pieceSpriteSheetPath, ssColumns, ssRows, -1, true, false);
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
