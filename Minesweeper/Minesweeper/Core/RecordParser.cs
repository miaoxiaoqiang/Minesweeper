using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Minesweeper.Utils;

namespace Minesweeper.Core
{
    internal sealed class RecordParser
    {
        private static readonly ConfusionGen _confusionGen;
        private static readonly RNGCryptoServiceProvider rngCSP;
        private static readonly RijndaelManaged rijndael;
        private static readonly XmlReaderSettings settings;

        private static string ErrString = string.Empty;

        static RecordParser()
        {
            _confusionGen = new ConfusionGen();
            rngCSP = new RNGCryptoServiceProvider();

            rijndael = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = new byte[]
                {
                    0x56, 0x53, 0x7A, 0x71, 0x45, 0x3D, 0x54, 0x7C, 0x3F, 0x47, 0x61, 0x74, 0x37, 0x31, 0x2A, 0x55,
                    0x5B, 0x3B, 0x40, 0x4F, 0x51, 0x5F, 0x70, 0x47, 0x35, 0x5B, 0x5F, 0x22, 0x70, 0x5D, 0x75, 0x64
                },
                IV = new byte[]
                {
                    0x44, 0x72, 0x37, 0x68, 0x54, 0x6B, 0x5F, 0x40, 0x27, 0x5B, 0x33, 0x65, 0x36, 0x76, 0x49, 0x66
                }
            };

            string xsd = string.Empty;
            using (Stream stream = System.Windows.Application.GetResourceStream(new Uri("Structure\\Structure.xsd", UriKind.Relative)).Stream)
            {
                using (StreamReader reader = new(stream))
                {
                    settings = new XmlReaderSettings();
                    XmlSchemaSet schemaSet = new();
                    schemaSet.Add(null, XmlReader.Create(new StringReader(reader.ReadToEnd())));
                    settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventCallBack);
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas = schemaSet;
                }
            }
        }

        public static void SaveRecord(Model.PlayerArchive record, string filepath)
        {
            try
            {
                //if (File.Exists(filepath))
                //{
                //    FileInfo fi = new(filepath);
                //    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                //    {
                //        fi.Attributes = FileAttributes.Normal;
                //    }
                //}

                using (MemoryStream sr = new())
                {
                    XmlWriterSettings settings = new()
                    {
                        Indent = false,
                        NewLineOnAttributes = false,
                        Encoding = new UTF8Encoding(false)
                    };

                    using (XmlWriter xmlWriter = XmlWriter.Create(sr, settings))
                    {
                        XmlSerializer serializer = new(record.GetType());
                        XmlSerializerNamespaces namespaces = new();
                        namespaces.Add(string.Empty, string.Empty);
                        serializer.Serialize(xmlWriter, record, namespaces);

                        using (ICryptoTransform cTransform = rijndael.CreateEncryptor())
                        {
                            byte[] data = sr.ToArray();
                            byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
                            File.WriteAllBytes(filepath, resultArray);
                        }
                        //File.SetAttributes(filepath, FileAttributes.ReadOnly);
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                throw ex;
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Model.PlayerArchive LoadRecord(string recordfile)
        {
            ErrString = string.Empty;

            byte[] _data = File.ReadAllBytes(recordfile);
            if (_data == null || _data.Length == 0)
            {
                throw new FileFormatException("字节流应不为空");
            }

            string rawtext = string.Empty;
            using (ICryptoTransform cTransform = rijndael.CreateDecryptor())
            {
                byte[] rawdata = cTransform.TransformFinalBlock(_data, 0, _data.Length).BytesTrimEnd();
                rawtext = Encoding.UTF8.GetString(rawdata);
            }

            CheckXmlValidate(rawtext);

            if (ErrString.Length > 0)
            {
                throw new FileFormatException(ErrString);
            }

            try
            {
                XmlSerializer xs = new(typeof(Model.PlayerArchive));
                Model.PlayerArchive ret = xs.Deserialize(new StringReader(rawtext)) as Model.PlayerArchive;

                return ret;
            }
            catch(XmlSchemaException ex)
            {
                throw ex;
            }
            catch (XmlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckXmlValidate(string xmltext)
        {
            try
            {
                using (StringReader sRead = new(xmltext))
                {
                    using (XmlReader xmlRead = XmlReader.Create(sRead, settings))
                    {
                        while (xmlRead.Read())
                        {

                        }
                    }
                }
            }
            catch (XmlException exec)
            {
                ErrString = exec.Message;
            }
        }

        private static void ValidationEventCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)//区分是警告还是错误 
            {
                ErrString += "警告：" + e.Message + "\r\n";
            }
            else
            {
                ErrString += "错误：" + e.Message + "\r\n";
            }
        }

        /// <summary>
        /// 对字节数组进行混淆化
        /// </summary>
        /// <param name="rawData">原始数据</param>
        /// <param name="useHash">指示是否对 <paramref name="rawData"/> 先进行哈希处理</param>
        /// <returns>
        /// 生成经过Base62编码的字符串
        /// </returns>
        public static string ConfuseBytes(byte[] rawData, bool useHash = true)
        {
            return _confusionGen.ConfuseBytes(rawData, useHash);
        }

        /// <summary>
        /// 在指定的连续范围生成随机数
        /// </summary>
        /// <param name="minValue">最小值（包含）</param>
        /// <param name="maxValue">最大值（不包含）</param>
        /// <returns>
        /// <paramref name="minValue"/> &lt;= 随机数值 &lt; <paramref name="maxValue"/>
        /// </returns>
        public static int GenerateRandom(int minValue, int maxValue)
        {
            int m = maxValue - minValue;
            decimal _base = long.MaxValue;
            byte[] rndSeries = new byte[8];
            rngCSP.GetBytes(rndSeries);
            long l = BitConverter.ToInt64(rndSeries, 0);
            int rnd = (int)(Math.Abs(l) / _base * m);
            return minValue + rnd;
        }
    }
}
