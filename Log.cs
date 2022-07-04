using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
namespace server
{
    public  class Log
    {
         public  delegate Task<string> LogHnadeler(string filename, string message);
         public   LogHnadeler logEnent;
        public Log()
        {
            logEnent = new LogHnadeler(LogMsg);
            path=AppDomain.CurrentDomain.BaseDirectory+@"\Log\"+DateTime.Now.ToString("yyyyMMdd");
        }
        public string path = "";
        private async Task<string> LogMsg(string filename,string MSG)
        {

            try
            {
                 return await Task.Run<string>(     () => {
                    string path1 = path + @"\" + filename.Replace(":","")  + "txt";
                    if (logEnent != null)
                    {
                        if (!Directory.Exists(path))
                        {
                         
                          var resuit=    Directory.CreateDirectory(path);
                          

                        }
                        if (!File.Exists(path1))
                        {
                            File.Create(path1).Close();
                           

                        }

                        using (StreamWriter sw = new StreamWriter(path1, true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss:FF") + MSG);
                            sw.Close();
                        }
                       
                    }

                     return "OK";

                });//.ConfigureAwait(false)
           
            }
            catch (Exception e4x)
            {
                MessageBox.Show(e4x.Message);
                return e4x.Message;
            }
        }

        public string _FilePath = "";
        public bool CreateFlie(string name)
        {

            try
            {
                _FilePath = path + @"\" + name + ".csv";
                if (!Directory.Exists(path))
                {

                    var resuit = Directory.CreateDirectory(path);


                }
                if (!File.Exists(_FilePath))
                {
                    File.Create(_FilePath).Close();


                }

                return true;    
            }
            catch (Exception ex)
            {

                return false;
            }
        
        }

        public async void LogCsv(string msg)
        {
            await Task.Run(() => {

                try
                {
                 using (StreamWriter sw = new StreamWriter(_FilePath, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss:FF") +"," +msg);
                    sw.Close();
                }
                }
                catch (Exception)
                {

                    
                }
              
            });
        }
    }

}
