using GXPEngine;
using System;
using System.Collections.ObjectModel;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_SimonSays : Module
    {
        public event Action<int,int> OrderChanged;
        public ReadOnlyCollection<int> RandomNumers { get; }
        
        readonly int[] _randomNumbers = new int[3];

        public Module_SimonSays(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            RandomNumers = Array.AsReadOnly(_randomNumbers);

            var minVal = data.GetIntProperty("MinValue", 0);
            var maxVal = data.GetIntProperty("MaxValue", 11);

            do
            {
                UniqueRandomNumbersGenerator(_randomNumbers, minVal, maxVal);
            } 
            while (IsInAscendingOrder(_randomNumbers));
            alpha = 0;
            var selector = new Module_SimonSays_Selector(this);
            AddChild(selector);

            var visual = new Module_SimonSays_Visual(this, selector, data);
            AddChild(visual);

            
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
