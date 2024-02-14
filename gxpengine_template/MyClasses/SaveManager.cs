
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace gxpengine_template.MyClasses
{
    public abstract class FileData 
    {
        public FileData() 
        {
            Console.WriteLine("typeName " + GetType());
        }
        public abstract string SerializeSelf();
        protected abstract void DeserializeSelf(string json);

    }

    public class PlayerData : FileData
    {
        public int Health { get; set; } = 5;
        public string Name { get; set; } = "blower";



        public override string SerializeSelf()
        {
            return JsonSerializer.Serialize(this);
        }

        protected override void DeserializeSelf(string json)
        {
            PlayerData data = JsonSerializer.Deserialize<PlayerData>(json);
            Health = data.Health;
            Name = data.Name;
        }
    }

    public class SaveManager
    {
        public List<FileData> _fileData = new List<FileData>();
        public SaveManager() 
        {
        
        }
        void LoadFromFile(string format)
        {
            
        }
        public void SaveOnFile(FileData data)
        {
            if (!_fileData.Contains(data))
            {
                _fileData.Add(data);
            }
        }

        public void LoadOnFile()
        {
            string txt = "[";
            foreach (FileData data in _fileData)
            {
                txt += data.SerializeSelf() + ",";
            }
            txt = txt.TrimEnd(',') + "]";
            Console.WriteLine(txt);
        }
    }

}
