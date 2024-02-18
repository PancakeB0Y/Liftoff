using GXPEngine;
using System;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_SimonSays_Selector : GameObject
    {
        public event Action<int> Selected;
        public event Action<int> Deselected;

        public bool CanUse { get; set; }

        readonly int[] selects = new int[2];
        readonly Module_SimonSays _moduleLogic;

        public Module_SimonSays_Selector(Module_SimonSays logic)
        {
            _moduleLogic = logic;
            DeselectAll();
        }

        void Update()
        {
            if (!CanUse) return;

            if (Input.GetKeyDown(Key.H))
            {
                Perfrom(0);
            }
            else if (Input.GetKeyDown(Key.J))
            {
                Perfrom(1);
            }
            else if (Input.GetKeyDown(Key.K))
            {
                Perfrom(2);
            }
        }

        void DeselectAll()
        {
            for (int i = 0; i < selects.Length; i++)
            {
                if (selects[i] == -1) continue;

                Deselected?.Invoke(selects[i]);
                selects[i] = -1;
            }
        }

        void Perfrom(int ballIndex)
        {
            for (int i = 0; i < selects.Length; i++)
            {
                if (selects[i] == -1)
                {
                    //select
                    selects[i] = ballIndex;
                    Selected?.Invoke(ballIndex);

                    if (selects[0] != -1 && selects[1] != -1)// perform swap
                    {
                        _moduleLogic.ChangeOrder(selects[0], selects[1]);

                        DeselectAll();
                    }
                    break;
                }
                else if (selects[i] == ballIndex)
                {
                    //deselect
                    selects[i] = -1;
                    Deselected?.Invoke(ballIndex);
                    break;
                }


            }
        }

    }
}
