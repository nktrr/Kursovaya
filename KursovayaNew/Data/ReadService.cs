using NPOI.OpenXml4Net.OPC;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KursovayaNew.Data
{
    public class ReadService
    {
        Random random = new Random();
        public Task<string> GetTextFromDocx(byte[] bytes)
        {
            return Task.FromResult(ReadDocx(bytes));
        }

        private string ReadDocx(byte[] sourceBytes)
        {
            string fileName = random.Next(0, 100000000).ToString();
            string tempFilePath = "tempFiles/" + fileName + ".docx";
            string editedTempFilePath = "tempFiles/" + fileName + "_new.docx";
            string readedString = "";              

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(sourceBytes, 0, sourceBytes.Length);
            }
            try
            {
                XWPFDocument doc = new XWPFDocument(OPCPackage.Open(tempFilePath));
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
                                readedString += text;
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
            }
            finally
            {
                File.Delete(tempFilePath);
            }
            return readedString;
        }
    }
}
