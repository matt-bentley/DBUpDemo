using DBUpDemo.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBUpDemo.Tests
{
    [TestFixture]
    public class ScriptTests
    {
        private Assembly _assembly;
        private List<string> _embeddedScripts;
        private string _scriptsFolder;
        private List<string> _checkFiles;
        private List<string> _embeddedFiles;

        [SetUp]
        public void Init()
        {
            _assembly = Assembly.GetAssembly(typeof(DummyClass));
            _scriptsFolder = string.Format("{0}.Scripts", _assembly.GetName().Name);
            _embeddedScripts = _assembly.GetManifestResourceNames().Where(r => r.StartsWith(_scriptsFolder) && r.EndsWith(".sql"))
                .ToList();
            _embeddedScripts.Sort();
            string path = System.Reflection.Assembly.GetAssembly(typeof(DummyClass)).Location;
#if DEBUG
            path = path.Replace(".Tests\\bin\\Debug\\DBUpDemo.exe", "\\Scripts\\");
#endif
            path = path.Replace(".Tests\\bin\\Release\\DBUpDemo.exe", "\\Scripts\\");
            _checkFiles = Directory.GetFiles(path).ToList();
            for (int i = 0; i < _checkFiles.Count; i++)
            {
                _checkFiles[i] = _checkFiles[i].Replace(path, "");
            }
            _embeddedFiles = new List<string>();
            for (int i = 0; i < _embeddedScripts.Count; i++)
            {
                _embeddedFiles.Add(_embeddedScripts[i].Replace(_scriptsFolder + ".", ""));
            }
        }

        [Test]
        public void CheckScriptsEmbedded()
        {
            foreach(string file in _checkFiles)
            {
                bool exists = false;
                for(int i = 0; i < _embeddedFiles.Count; i++)
                {
                    if(file == _embeddedFiles[i])
                    {
                        exists = true;
                        break;
                    }
                }
                Assert.That(exists, Is.True,$"{file} is not an embedded resource");
            }      
        }

        [Test]
        public void CheckUsingStatements()
        {
            foreach(string script in _embeddedScripts)
            {
                int indexOf = -1;
                using (Stream stream = _assembly.GetManifestResourceStream(script))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string sql = reader.ReadToEnd().ToLower();
                    indexOf = sql.IndexOf("\nUSE ".ToLower());
                }
                Assert.That(indexOf, Is.LessThan(0), $"{script} contains a USE statement");
            }
        }

        [Test]
        public void CheckSPNames()
        {
            int i = 1;
            Dictionary<string, int> altered = GetFirstOccurances("ALTER PROCEDURE ".ToLower());
            Dictionary<string, int> created = GetFirstOccurances("CREATE PROCEDURE ".ToLower());
            foreach(var sp in altered)
            {
                if (!created.ContainsKey(sp.Key))
                {
                    Assert.Fail($"{sp.Key} is altered but never created");
                }
                else
                {
                    if(sp.Value < created[sp.Key])
                    {
                        Assert.Fail($"{sp.Key} is altered before it is created");
                    }
                }
            }
        }

        private Dictionary<string, int> GetFirstOccurances(string searchTerm)
        {
            Dictionary<string, int> occurances = new Dictionary<string, int>();
            int scriptNumber = 1;
            foreach (string script in _embeddedScripts)
            {
                using (Stream stream = _assembly.GetManifestResourceStream(script))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string sql = reader.ReadToEnd().ToLower();
                    var indexes = sql.AllIndexesOf(searchTerm);
                    foreach (int index in indexes)
                    {
                        string spName = GetObjectName(sql.Substring(index + searchTerm.Length));
                        if (!occurances.ContainsKey(spName))
                        {
                            occurances.Add(spName, (scriptNumber * 10000000) + index);
                        }                   
                    }
                }
                scriptNumber++;
            }
            return occurances;
        }

        private string GetObjectName(string sql)
        {
            if(sql[0] == '[')
            {
                return sql.Substring(1, sql.IndexOf("]") - 1);
            }
            else
            {
                return sql.Substring(0, GetObjectNameEnd(sql));
            }
        }

        private int GetObjectNameEnd(string sql)
        {
            int spaceTo = sql.IndexOf(" ");
            int bracketTo = sql.IndexOf("(");
            int newLineTo = sql.IndexOf("\r\n");
            if(spaceTo == -1 && bracketTo == -1 && newLineTo == -1)
            {
                throw new IndexOutOfRangeException("Incorrect syntax for stored procedure");
            }
            int endIndex = spaceTo;
            endIndex = endIndex == -1 || (bracketTo != -1 && endIndex > bracketTo) ? bracketTo : endIndex;
            endIndex = endIndex == -1 || (newLineTo != -1 && endIndex > newLineTo) ? newLineTo : endIndex;
            return endIndex;
        }

        //[TearDown]
        //public void TearDown()
        //{

        //}
    }
}
