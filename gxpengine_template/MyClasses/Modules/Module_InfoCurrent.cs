using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_InfoCurrent : Module
    {
        public bool IsOnWrongPath { get; private set; } = true;
        public (float pos, bool isSpawned)[] GoodFiles { get; private set; }
        public (float pos, bool isSpawned)[] BadFiles { get; private set; }

        public readonly float FileSpeed;

        Module_InfoCurrent_Visual _visual;
        TiledObject _data;

        readonly int _goodFileCount;
        readonly int _badFileCount;
        readonly int _totalFiles;

        readonly int _maxDist = 100;
        readonly int _goodSpawnDist;
        readonly int _badSpawnDist;

        readonly int _requiredGoodCount;

        readonly int _wireMin = 42;
        readonly int _wireMax = 68;

        readonly float _randomSpeedRange;

        int _lastSpawnedGood = 0;
        int _lastSpawnedBad = 0;
        int _collectedFiles = 0;

        int _winScore;
        public Module_InfoCurrent(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            _data = data;
            moduleType = ModuleTypes.Switch;

            _goodFileCount = data.GetIntProperty("GoodFileCount", 5);
            _badFileCount = data.GetIntProperty("BadFileCount", 5);
            _totalFiles = _goodFileCount + _badFileCount;
            FileSpeed = data.GetFloatProperty("Speed", 1);
            _randomSpeedRange = data.GetFloatProperty("RandomSpeedRange", 0.3f);

            _goodSpawnDist = data.GetIntProperty("GoodSpawnDist", 100);
            _badSpawnDist = data.GetIntProperty("BadSpawnDist", 100);

            _requiredGoodCount = data.GetIntProperty("RequiredGoodCount", 3);
            _winScore = data.GetIntProperty("WinScore", 10);

            GoodFiles = new (float pos, bool isSpawned)[_goodFileCount];

            BadFiles = new (float pos, bool isSpawned)[_badFileCount];

            _visual = new Module_InfoCurrent_Visual(this, data);
            AddChild(_visual);
        }

        override public object Clone()
        {
            var clone = new Module_InfoCurrent(texture.filename, _cols, _rows, _data);

            return clone;
        }

        void ChangePath()
        {
            IsOnWrongPath = !IsOnWrongPath;
            _visual.MoveWire();
        }

        void CheckFilesPos()
        {
            for (int i = 0; i < GoodFiles.Length; i++)
            {
                if (!GoodFiles[i].isSpawned) { break; }

                if (GoodFiles[i].pos == -1f) { continue; }

                if (IsOnWire(GoodFiles[i].pos) && IsOnWrongPath)
                {
                    GoodFiles[i].pos = -1f;
                    isComplete();
                    continue;
                }

                if (GoodFiles[i].pos >= _maxDist)
                {
                    _collectedFiles++;
                    GoodFiles[i].pos = -1f;
                    _visual.LightBox(true);
                    isComplete();
                }
            }

            for (int i = 0; i < BadFiles.Length; i++)
            {
                if (!BadFiles[i].isSpawned) { break; }

                if (BadFiles[i].pos == -1f) { continue; }

                if (IsOnWire(BadFiles[i].pos) && IsOnWrongPath)
                {
                    BadFiles[i].pos = -1f;
                    isComplete();
                    continue;
                }

                if (BadFiles[i].pos >= _maxDist)
                {
                    _collectedFiles--;
                    BadFiles[i].pos = -1f;
                    _visual.LightBox(false);
                    isComplete();
                }
            }
        }

        void FileSpawnerGood()
        {
            if (!CheckCanSpawn(false)) { return; }

            for (int i = 0; i < GoodFiles.Length; i++)
            {
                if (GoodFiles[i].isSpawned == false && CheckCanSpawn(true))
                {
                    GoodFiles[i].isSpawned = true;
                    _lastSpawnedGood = i;
                    break;
                }
            }

        }

        void FileSpawnerBad()
        {
            if (!CheckCanSpawn(true)) { return; }

            for (int i = 0; i < BadFiles.Length; i++)
            {
                if (BadFiles[i].isSpawned == false && CheckCanSpawn(false))
                {
                    BadFiles[i].isSpawned = true;
                    _lastSpawnedBad = i;
                    break;
                }
            }

        }

        void FileSpawner()
        {
            if (Utils.Random(0, 2) == 0)
            {
                FileSpawnerGood();
            }
            else
            {
                FileSpawnerBad();
            }
        }

        bool CheckCanSpawn(bool isGood)
        {
            bool canSpawn = false;

            if (isGood)
            {
                if (!GoodFiles[0].isSpawned) { return true; }

                for (int i = 0; i < GoodFiles.Length; i++)
                {
                    if (GoodFiles[_lastSpawnedGood].pos >= _goodSpawnDist || GoodFiles[_lastSpawnedGood].pos == -1)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (!BadFiles[0].isSpawned) { return true; }

                for (int i = 0; i < BadFiles.Length; i++)
                {
                    if (BadFiles[_lastSpawnedBad].pos >= _badSpawnDist || BadFiles[_lastSpawnedBad].pos == -1)
                    {
                        return true;
                    }
                }
            }

            return canSpawn;
        }

        void MoveFiles()
        {
            for (int i = 0; i < GoodFiles.Length; i++)
            {
                if (GoodFiles[i].isSpawned == false || GoodFiles[i].pos == -1)
                {
                    continue;
                }
                float randSpeed = Utils.Random(FileSpeed - _randomSpeedRange, FileSpeed + _randomSpeedRange);

                GoodFiles[i].pos += randSpeed * Time.deltaTime * 0.03f;
                GoodFiles[i].pos = Mathf.Clamp(GoodFiles[i].pos, 0, _maxDist);
            }

            for (int i = 0; i < BadFiles.Length; i++)
            {
                if (BadFiles[i].isSpawned == false || BadFiles[i].pos == -1)
                {
                    continue;
                }
                float randSpeed = Utils.Random(FileSpeed - _randomSpeedRange, FileSpeed + _randomSpeedRange);

                BadFiles[i].pos += randSpeed * Time.deltaTime * 0.03f;
                BadFiles[i].pos = Mathf.Clamp(BadFiles[i].pos, 0, _maxDist);
            }
        }

        bool IsOnWire(float n)
        {
            if (n >= _wireMin && n <= _wireMax) { return true; }

            return false;
        }

        void ReadInputs()
        {
            if (Input.GetKeyDown(Key.B))
            {
                ChangePath();
            }
        }

        void isComplete()
        {
            if (_lastSpawnedGood < _goodFileCount - 1 || _lastSpawnedBad < _badFileCount - 1) { return; }

            if (GoodFiles[_goodFileCount - 1].pos == -1 && BadFiles[_badFileCount - 1].pos == -1)
            {
                if (_collectedFiles >= _requiredGoodCount)
                {
                    RaiseSuccesEvent();
                }
                else
                {
                    RaiseFailEvent();
                }
            }

        }

        protected override void OnTimeEnd()
        {
            if (_collectedFiles >= _requiredGoodCount)
            {
                RaiseSuccesEvent();
            }
            else
            {
                RaiseFailEvent();
            }
        }

        void Update()
        {
            ReadInputs();
            MoveFiles();
            FileSpawner();
            CheckFilesPos();
        }
    }
}
