using GXPEngine;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_SimonSays : Module
    {
        private enum EquationType
        {
            Mult,
            Div,
            Add,
            Subst
        }

        public event Action<int, int> OrderChanged;
        public ReadOnlyCollection<string> Results { get; }
        readonly string[] _results = new string[3];
        readonly int[] _randomNumbers = new int[3];

        int _mult1;
        int _mult2;
        int _adder1;
        int _adder2;

        TiledObject _data;
        public Module_SimonSays(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            moduleType = ModuleTypes.ThreeButtons;
            _data = data;
            Results = Array.AsReadOnly(_results);

            var minVal = data.GetIntProperty("MinValue", 0);
            var maxVal = data.GetIntProperty("MaxValue", 11);
            _mult1 = data.GetIntProperty("EquationMultMin", 2);
            _mult2 = data.GetIntProperty("EquationMultMax", 4);
            _adder1 = data.GetIntProperty("EquationAdderMin", 4);
            _adder2 = data.GetIntProperty("EquationAdderMax", 10);

            float[] chancePerDifficulty = data.GetStringProperty("ChancePerDifficultyCSV").Split(',').Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();

            do
            {
                UniqueRandomNumbersGenerator(_randomNumbers, minVal, maxVal);
            }
            while (IsInAscendingOrder(_randomNumbers));

            MapToEquations(chancePerDifficulty[0]);

            alpha = 0;
            var selector = new Module_SimonSays_Selector(this);
            AddChild(selector);

            var visual = new Module_SimonSays_Visual(this, selector, data);
            AddChild(visual);
        }
        override public object Clone()
        {
            var clone = new Module_SimonSays(texture.filename, _cols, _rows, _data);
            clone.width = 314;
            clone.height = 88;

            return clone;
        }

        public void ChangeOrder(int fromIndex, int toIndex)
        {
            (_randomNumbers[fromIndex], _randomNumbers[toIndex]) = (_randomNumbers[toIndex], _randomNumbers[fromIndex]);
            OrderChanged?.Invoke(fromIndex, toIndex);
        }

        public void CheckSucces()
        {
            if (IsInAscendingOrder(_randomNumbers)) RaiseSuccesEvent();
        }

        void MapToEquations(float chance)
        {
            int i = 0;
            foreach (var num in _randomNumbers)
            {
                if (Utils.Random(0f, 1) > chance)
                {
                    _results[i] = num.ToString();
                }
                else
                {
                    string equation = null;
                    while (equation == null)
                        equation = GetEquationFromResult((EquationType)Utils.Random(0, 4), num);

                    _results[i] = equation;

                }
                i++;
            }

        }

        string GetEquationFromResult(EquationType ecuationType, int result)
        {
            switch (ecuationType)
            {
                case EquationType.Mult:

                    if (result.IsPrime(out int smallestDiv) || smallestDiv == -1) return null;

                    return $"{result / smallestDiv} * {smallestDiv}";

                case EquationType.Div:
                    if (result <= 0) return null;
                    //mult1, mult2
                    int multiplicator = Utils.Random(_mult1, _mult2);
                    return $"{result * multiplicator} / {multiplicator}";

                case EquationType.Add:
                    if (result < 1) return null;

                    int decrementer = Utils.Random(1, result);
                    return $"{result - decrementer} + {decrementer}";

                case EquationType.Subst:
                    int adder = Utils.Random(_adder1, _adder2);
                    return $"{result + adder} - {adder}";

                default:
                    return null;

            }
        }

        void UniqueRandomNumbersGenerator(int[] randomNums, int minVal, int maxVal)
        {
            if (minVal >= maxVal - 1) throw new Exception("min value is bigger or equal than max value");

            for (int i = 0; i < randomNums.Length; i++)
            {

                int newRandomNum;
                do
                {
                    newRandomNum = Utils.Random(minVal, maxVal);
                    randomNums[i] = newRandomNum;

                }
                while (ValueAlreadyExists(randomNums, i, newRandomNum));
            }
        }

        bool ValueAlreadyExists(int[] array, int checkUntill, int valueToCheck)
        {
            for (int j = 0; j < checkUntill; j++)
                if (array[j] == valueToCheck)
                {
                    return true;
                }
            return false;
        }

        bool IsInAscendingOrder(int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i - 1] > arr[i])
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnTimeEnd()
        {
            RaiseFailEvent();
        }
    }
}
