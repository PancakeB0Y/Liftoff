using GXPEngine;
using System;
using System.Collections;

namespace gxpengine_template.MyClasses.Coroutines
{
    public class Coroutine : GameObject
    {
        public bool Done { get; private set; } = false;

        readonly IEnumerator enumerator; // The enumerator that keeps track of the state of the coroutine function
        object item = null; // The last item returned by the enumerator

        public Coroutine(IEnumerator pEnumerator)
        {
            enumerator = pEnumerator;
        }

        // This Step method executes the next stage of the coroutine function, until the next yield return statement,
        //  or until we arrive at the end of the function.
        void Step()
        {
            if (!enumerator.MoveNext())
            {
                Destroy();
                Done = true;
            }
            else
            {
                item = enumerator.Current;
            }
        }

        public void Update()
        {
            if (parent == null) return; // In case the Coroutine is destroyed this frame - Recall that Update may still be called once!

            if (item == null)
            {
                Step();
            }
            else if (item is ICoroutineStepper stepper)
            {
                if (stepper.IsDone()) Step();
            }
            else
            { // Unsupported type returned: Just print warning, ignore and continue
                Console.WriteLine("Coroutine error: return type not supported: " + item.GetType());
                Step();
            }
        }
    }
}
