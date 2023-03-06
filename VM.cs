using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory2
{
    internal class VM
    {
        string path = "VirtualMemory";    
        byte[] buffer = new byte[512];
        byte[] buffer2= new byte[64];
        List<Page> pages=new List<Page>();
        public void CreateObject()
        {           
            CreateOrOpenFile(path);
        }
        private int CheckValueInByteMap( int position , int NumberList) // проверка значения битовой карты
        {
            position = position - NumberList * 512;
            byte result = 0b1000_0000;
            result >>= position%8; 
            if (Equals(pages[SearchIndexList(NumberList)].map[position/8] & result , 0)) return 1; // ячейка пустая
            return 0; // ячейка занята 
        }
        private void PullInData(int position, int NumberList, byte value) // заполнения ячейки памяти
        {
            position = position - 512 * NumberList;
            pages[SearchIndexList(NumberList)].Data[position] = value;
            pages[SearchIndexList(NumberList)].Time = DateTime.Now;
        }
        private void PullInByteMap(int Index, int NumberList) // заполнение ячейки битовой карты
        {            
            Index = Index - NumberList * 512;
            byte result = 0b1000_0000;
            result >>= Index%8;
            pages[SearchIndexList(NumberList)].map[Index/8] |= result  ;
        }
        private int SearchIndexList(int NumberList) // поиск индекса страницы по номеру страницы
        {
            for (int i = 0; i < pages.Count(); i++) 
            {
                if (pages[i].NumberList == NumberList) return i;  // возврат необходимой страницы
            }
            return-1; // исключительная ситуация
        }
        private int SearchOldPage() //поиск самой старой страницы в оперативной памяти
        {
            int NumberOldList =0;
            DateTime date= DateTime.MaxValue;
            for(int i = 0; i < pages.Count(); i++)
            {
                if (DateTime.Compare(pages[i].Time , date)  <= 0) 
                {
                    NumberOldList = i;
                    date = pages[i].Time;
                }
            }
            return NumberOldList;
        }
        private void SavePageInMemory(int OldPage) // сохранние страницы в память 
        {
            BinaryWriter file = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            file.Write("VM");
            file.Seek(592 * OldPage, SeekOrigin.Current);
            file.Write(OldPage);
            file.Write(pages[SearchIndexList(OldPage)].Mod);
            file.Write(pages[SearchIndexList(OldPage)].Time.ToBinary());
            for (int g = 0; g < 64; g++)
            {
                file.Write(pages[SearchIndexList(OldPage)].map[g]);     
            }    
             for (int j = 0; j < 512; j++)
            {
                file.Write(pages[SearchIndexList(OldPage)].Data[j]);
            }        
            file.Close();
        }
        private void LoadPageFromMemory(int NewPage) // Загрузка страници из памяти , если ее нет в оперативной 
        {    
            int OldPage =  SearchOldPage();
            BinaryReader file = new BinaryReader(File.OpenRead(path));
            file.ReadString();
            file.ReadBytes(592*NewPage);
            int NumberList = file.ReadInt32();
            int Mod = file.ReadInt32();
            DateTime time = DateTime.FromOADate(file.ReadDouble());
            buffer2 = file.ReadBytes(64);
            buffer = file.ReadBytes(512);
            pages[SearchIndexList(OldPage)] = new Page(NumberList, Mod, time, buffer, buffer2);
            file.Close();

        }
        private int CheckListInMemory(int list) 
        {
            for (int i = 0; i < pages.Count();i++) 
            {
                if (pages[i].NumberList == list)
                {
                    return 1;
                }
            }
            return 0;
        }
        private void CreateOrOpenFile(string name ) // Открытие файла или , если файла нет , то его создание
        {
            string path = name;         
            if (File.Exists(path))
            {
                BinaryReader file = new BinaryReader(File.OpenRead(path));
                file.ReadString();         
                for (int i = 0; i < 5; i++)
                {              
                    int NumberList = file.ReadInt32();
                    int Mod = file.ReadInt32();
                    DateTime time = DateTime.FromOADate(file.ReadDouble());           
                    buffer2 = file.ReadBytes(64); 
                    buffer = file.ReadBytes(512);                  
                    pages.Add(new Page(NumberList,Mod,time,buffer,buffer2));
                }
                file.Close();           
            }
            else
            {
                //VM НомерСтраницы ФлагМодификации БитоваяКарта
                BinaryWriter file = new BinaryWriter(File.Open(path,FileMode.OpenOrCreate));
                file.Write("VM");
                for (int i = 0; i < 40; i++)
                {   
                    file.Write(i);
                    file.Write(0);
                    file.Write(DateTime.MinValue.ToBinary());
                    for (int g = 0; g < 64/4; g++)
                    {
                        file.Write(0);
                    }
                    for (int j = 0; j < 512/4; j++)
                    {
                        file.Write(0);
                    }               
                }
                file.Close();
                CreateOrOpenFile(path);
            }               
        
        }       
        private int SearchList(int index) // поиск страницы
        {
            int list = 0;
            for (int i = 0; i < 40; i++)
            {
                if (index < (i+1) * (512))
                {
                    list = i;
                    return list;
                }
            }
            throw new Exception("Index error");
        }
        public int ByteMapInfo(int index) 
        {
            int point = 0;
            int list = 0;// номер страницы 
            try
            {
                list = SearchList(index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Страница 4 байта Модификация 4 байта Дата 8 байт КартаБитов 64 байт Память 512 байт = 592 байт
            if (CheckListInMemory(list) == 0)
            {
                if (pages[SearchIndexList(SearchOldPage())].Mod == 1)
                SavePageInMemory(SearchOldPage());
                LoadPageFromMemory(list);
            }
            try
            {
                point = CheckValueInByteMap(index, list);
            }
            catch (IndexOutOfRangeException e) { Console.WriteLine(e); return -1; }
            if (point == 0)
            {
                Console.WriteLine($"Позиция занята значением :{Convert.ToByte(buffer[(index-512*list)])}");
            }
            else
            {
                Console.WriteLine("Позиция свободна");
            }
            return list;
        } 
        public int WriteOnIndex(int index )
        {
            int list = ByteMapInfo(index);
            byte value = 0b0000_0000;
            value = Convert.ToByte(Console.ReadLine());
            PullInByteMap(index, list);
            PullInData(index, list, value);
            pages[SearchIndexList(list)].Mod = 1;
            Console.WriteLine("Операция прошла успешно");
            return 0;
        }
        public void SaveAll() // сохранение страницы из оперативной памяти во внешнюю
        {
            for (int i = 0; i < pages.Count(); i++)
            {
                SavePageInMemory(pages[i].NumberList);
            }
        }
    }
}
