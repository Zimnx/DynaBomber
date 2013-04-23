using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
namespace DynaBomber
{
    class FileManager
    {
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, System.Text.StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileIntA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int FlushPrivateProfileString(int lpApplicationName, int lpKeyName, int lpString, string lpFileName);

        string strFilename;

        public void iniFile(string Filename)
        {
            strFilename = Filename;
        }
        public string FileName
        {
            get
            {
                return strFilename;
            }
        }

        public string GetString(string Section, string Key, string @Default) // string klucz = GetString("SEKCJA", "wartosc", "(none)"); 
        {                                                                    // (none) jest domyslna wartoscia w razie bledu
            string returnValue = ""; // Odczyt tekstu z pliku ini
            int intCharCount = 0;
            System.Text.StringBuilder objResult = new System.Text.StringBuilder(256);
            intCharCount = GetPrivateProfileString(Section, Key, @Default, objResult, objResult.Capacity, strFilename);
            if (intCharCount > 0)
            {
                returnValue = objResult.ToString().Substring(0, intCharCount);
            }
            return returnValue;
        }


        public void WriteString(string Section, string Key, string Value) // WriteString("SEKCJA", "wartosc", "zmienna"); 
        {// Zapis tekstu do pliku ini
            WritePrivateProfileString(Section, Key, Value, strFilename);
            Flush();
        }

        public void WriteInteger(string Section, string Key, int Value)
        {// Zapis liczby do pliku ini
            WriteString(Section, Key, Value.ToString());
            Flush();
        }

        public void Flush()
        {
            FlushPrivateProfileString(0, 0, 0, strFilename);
        }
    }
}