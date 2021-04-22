using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KursovayaNew.Data
{
    public class DecryptService
    {
        private Random random = new Random();
        public Task<string> GetDecryptedString(string text, string key)
        {
            return Task.FromResult(DecryptText(text, key));
        }

        public Task<byte[]> DecryptFile(byte[] sourceBytes, string key, string fileExtension)
        {
            if (fileExtension == "txt") return Task.FromResult(DecryptTxt(sourceBytes, key));
            throw new DecryptException();
        }

        private byte[] DecryptTxt(byte[] sourceBytes, string key)
        {
            foreach(byte b in sourceBytes)
            {
                Console.WriteLine();
            }
            String fileName = random.Next(0, 100000000).ToString();
            using (FileStream fs = new FileStream("tempFiles/" + fileName + ".txt", FileMode.Create, FileAccess.Write))
            {
                fs.Write(sourceBytes, 0, sourceBytes.Length);
            }
            string text = File.ReadAllText("tempFiles/" + fileName + ".txt");
            text = DecryptText(text, key);
            File.WriteAllText("tempFiles/" + fileName + ".txt", text);
            return File.ReadAllBytes("tempFiles/" + fileName + ".txt");
        }

        private string DecryptText(string text, string key)
        {
            Console.WriteLine("Thread: " + Thread.CurrentThread.Name);
            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            if (key == "") key = "скорпион";
            key = key.ToLower();
            int position = 0;
            string decrypted = "";
            foreach (char ch in text)
            {
                if (alphabet.Contains(ch))
                {
                    bool isUpper = false;
                    if (Char.IsUpper(ch)) isUpper = true;
                    char c = Char.ToLower(ch);
                    int charPosition = alphabet.IndexOf(c);
                    int keyPosition = alphabet.IndexOf(key[position]);
                    int dectyptedCharPosition = charPosition - keyPosition;
                    if (dectyptedCharPosition < 0) dectyptedCharPosition += 33;
                    if (isUpper) decrypted += Char.ToUpper(alphabet[dectyptedCharPosition]);
                    else decrypted += alphabet[dectyptedCharPosition];
                    position++;
                    if (position >= key.Length) position = 0;
                }
                else decrypted += ch;
            }
            return decrypted;
        }
    }
}
