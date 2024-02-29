using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Maze_Selector : GameObject
    {
        public event Action<int, int> SelectionChanged;
        readonly Module_Maze _moduleLogic;
        int _currentSelection = 0;

        public Module_Maze_Selector(Module_Maze logic) 
        {
            _moduleLogic = logic;
            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;
            ChangeSelection(0);

        }

        void Update()
        {
            if (Input.GetKeyDown(Key.W) &&
                _currentSelection >= _moduleLogic.Columns)
            {
                ChangeSelection(-_moduleLogic.Columns);
            }
            else if (Input.GetKeyDown(Key.S) && 
                _currentSelection < _moduleLogic.Pieces.Length - _moduleLogic.Columns)
            {
                ChangeSelection(_moduleLogic.Columns);
            }
            else if (Input.GetKeyDown(Key.D) && 
                _currentSelection < _moduleLogic.Pieces.Length - 1 && 
                (_currentSelection + 1) % _moduleLogic.Columns != 0)
            {
                ChangeSelection(1);
            }
            else if (Input.GetKeyDown(Key.A) &&
                _currentSelection > 0 && _currentSelection % _moduleLogic.Columns != 0)
            {
                ChangeSelection(-1);
            }
            else if (Input.GetKeyDown(Key.SPACE))
            {
                _moduleLogic.RotatePiece(_currentSelection);
            }
        }

        void ChangeSelection(int add)
        {
            int prevValue = _currentSelection;
            _currentSelection += add;
            SelectionChanged?.Invoke(prevValue, _currentSelection);
        }
    }
}
