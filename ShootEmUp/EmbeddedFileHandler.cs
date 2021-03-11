using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ShootEmUp
{
    public static class EmbeddedFileHandler
    {
        // https://stackoverflow.com/questions/4842038/streamreader-and-reading-an-xml-file
        public static StreamReader GetStreamReader(string filepath)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                // Change filepath to periods
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ShootEmUp." + filepath.Replace("/", ".")));

                return _textStreamReader;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
