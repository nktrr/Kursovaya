using Microsoft.AspNetCore.Components.Forms;
using NPOI.OpenXml4Net.OPC;
using NPOI.XWPF.UserModel;
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
            int position = 0;
            return Task.FromResult(DecryptText(text, key, ref position));
        }

        public Task<byte[]> DecryptFile(byte[] sourceBytes, string key, string fileExtension)
        {
            if (fileExtension == "txt") return Task.FromResult(DecryptTxt(sourceBytes, key));
            else if (fileExtension == "docx") return Task.FromResult(DecryptDocx(sourceBytes, key));
            throw new CryptoException();
        }

        private byte[] DecryptDocx(byte[] sourceBytes, string key)
        {
            string fileName = random.Next(0, 100000000).ToString();
            string tempFilePath = "tempFiles/" + fileName + ".docx";
            string editedTempFilePath = "tempFiles/" + fileName + "_new.docx";

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(sourceBytes, 0, sourceBytes.Length);
            }
            try
            {
                XWPFDocument doc = new XWPFDocument(OPCPackage.Open(tempFilePath));
                int position = 0;
                foreach (XWPFParagraph paragraph in doc.Paragraphs)
                {
                    var runs = paragraph.Runs;
                    if (runs != null)
                    {
                        foreach (XWPFRun r in runs)
                        {
                            string text = r.GetText(0);
                            if (text != null)
                            {
                                text = DecryptText(text, key, ref position);
                                r.SetText(text, 0);
                            }
                        }
                    }
                }

                foreach (XWPFTable tbl in doc.Tables)
                {
                    foreach (XWPFTableRow row in tbl.Rows)
                    {
                        foreach (XWPFTableCell cell in row.GetTableCells())
                        {
                            foreach (XWPFParagraph paragraph in cell.Paragraphs)
                            {
                                var runs = paragraph.Runs;
                                if (runs != null)
                                {
                                    foreach (XWPFRun r in runs)
                                    {
                                        string text = r.GetText(0);
                                        if (text != null)
                                        {
                                            text = DecryptText(text, key, ref position);
                                            r.SetText(text, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                using (var fs = new FileStream(editedTempFilePath, FileMode.Create, FileAccess.Write))
                {
                    doc.Write(fs);
                }
                doc.Close();
                byte[] newFile = File.ReadAllBytes(editedTempFilePath);
                File.Delete(editedTempFilePath);
                return newFile;
            }
            catch
            {
                throw new CryptoException();
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        private byte[] DecryptTxt(byte[] sourceBytes, string key)
        {
            String fileName = random.Next(0, 100000000).ToString();
            string tempFilePath = "tempFiles/" + fileName + ".txt";
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(sourceBytes, 0, sourceBytes.Length);
            }
            string text = File.ReadAllText(tempFilePath);
            int position = 0;
            text = DecryptText(text, key, ref position);
            File.WriteAllText(tempFilePath, text);
            byte[] newFile = File.ReadAllBytes(tempFilePath);
            File.Delete(tempFilePath);
            return newFile;
        }

        private string DecryptText(string text, string key, ref int position)
        {
            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            if (key == "") key = "скорпион";
            key = key.ToLower();
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
