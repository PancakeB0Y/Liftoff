using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses.UI
{
    public class TextMesh : GameObject
    {

        #region Properties
        public int Width
        {
            get => _canvas.width;
            set => _canvas.width = value;
        }
        public int Height
        {
            get => _canvas.height;
            set => _canvas.height = value;
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                Draw();
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Draw();
            }
        }

        public CenterMode HorizontalAlign
        {
            get => _horizontalAlign;
            set
            {
                _horizontalAlign = value;
                Draw();
            }
        }

        public CenterMode VerticalAlign
        {
            get => _verticalAlign;
            set
            {
                _verticalAlign = value;
                Draw();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Draw();
            }
        }

        public int TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;
                Draw();
            }
        }
        #endregion

        string _text;
        Color _backgroundColor = Color.Transparent;
        Color _textColor = Color.Black;
        CenterMode _horizontalAlign;
        CenterMode _verticalAlign;
        int _textSize;
        Font _font;

        readonly EasyDraw _canvas;

        public TextMesh(string content, int width, int height,
            //optional params
            CenterMode horizontalAlign = CenterMode.Center, CenterMode verticalAlign = CenterMode.Center, int textSize = 10
            , string fontFileName = null, FontStyle fontStyle = FontStyle.Regular, int fontSize = 12
        )
        {
            _canvas = new EasyDraw(width, height, false);
            _canvas.SetOrigin(width / 2, height / 2);
            AddChild(_canvas);

            _text = content;
            _horizontalAlign = horizontalAlign;
            _verticalAlign = verticalAlign;
            _textSize = textSize;
            if (fontFileName != null)
                _font = Utils.LoadFont(fontFileName, textSize, fontStyle);
            Draw();
        }

        public TextMesh(string content, int width, int height, Color txtColr, Color bgColr,
            //optional params
            CenterMode horizontalAlign = CenterMode.Center, CenterMode verticalAlign = CenterMode.Center, int textSize = 10
            , string fontFileName = null, FontStyle fontStyle = FontStyle.Regular, int fontSize = 12
        )
        {
            _canvas = new EasyDraw(width, height, false);
            _canvas.SetOrigin(width / 2, height / 2);
            AddChild(_canvas);

            _text = content;
            _textColor = txtColr;
            _backgroundColor = bgColr;
            _horizontalAlign = horizontalAlign;
            _verticalAlign = verticalAlign;
            _textSize = textSize;
            if (fontFileName != null)
                _font = Utils.LoadFont(fontFileName, textSize, fontStyle);
            Draw();
        }

        public void Draw()
        {
            _canvas.Clear(_backgroundColor);
            _canvas.TextAlign(_horizontalAlign, _verticalAlign);
            _canvas.Fill(_textColor);
            _canvas.TextSize(_textSize);
            if(_font != null)
                _canvas.TextFont(_font);
            _canvas.Text(_text);
        }

        public void SetOrigin(int x, int y)
        {
            _canvas.SetOrigin(x, y);
            //_canvas.SetXY(0, 0);
        }

    }
}
