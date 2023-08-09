// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;



// namespace CsvHelper
// {
//     /// <summary>
//     /// Class to read csv content from various sources
//     /// </summary>
//     public sealed class CsvReader : IDisposable
//     {

//         #region Members

//         private FileStream _fileStream;
//         private Stream _stream;
//         private StreamReader _streamReader;
//         private StreamWriter _streamWriter;
//         private Stream _memoryStream;
//         private Encoding _encoding;
//         private readonly static StringBuilder _columnBuilder = new StringBuilder(1024);
//         //private readonly Type _type = Type.File;

//         #endregion Members

//         #region Properties

//         /// <summary>
//         /// Gets or sets whether column values should be trimmed
//         /// </summary>
//         public bool TrimColumns { get; set; }

//         /// <summary>
//         /// Gets or sets whether the csv file has a header row
//         /// </summary>
//         public bool HasHeaderRow { get; set; }

//         /// <summary>
//         /// Returns a collection of fields or null if no record has been read
//         /// </summary>
//         private List<string> _Fields;
//         public List<string> Fields 
//         {
//             get
//             {
//                 return _Fields;
//             }
//             private set
//             {
//                 _Fields = value;
//             }
//         }

//         /// <summary>
//         /// Gets the field count or returns null if no fields have been read
//         /// </summary>
//         public int FieldCount
//         {
//             get
//             {
//                 return (Fields != null ? Fields.Count : 0);
//             }
//         }

//         #endregion Properties

//         #region Enums

//         /// <summary>
//         /// Type enum
//         /// </summary>
//         private enum Type
//         {
//             File,
//             Stream,
//             String
//         }

//         #endregion Enums

//         #region Constructors

//         /// <summary>
//         /// Initialises the reader to work from a file
//         /// </summary>
//         /// <param name="filePath">File path</param>
//         public CsvReader(string filePath)
//         {
//             //_type = Type.File;
//             Initialise(filePath, Encoding.Default);
//         }

//         /// <summary>
//         /// Initialises the reader to work from a file
//         /// </summary>
//         /// <param name="filePath">File path</param>
//         /// <param name="encoding">Encoding</param>
//         public CsvReader(string filePath, Encoding encoding)
//         {
//             //_type = Type.File;
//             Initialise(filePath, encoding);
//         }

//         /// <summary>
//         /// Initialises the reader to work from an existing stream
//         /// </summary>
//         /// <param name="stream">Stream</param>
//         public CsvReader(Stream stream)
//         {
//             //_type = Type.Stream;
//             Initialise(stream, Encoding.Default);
//         }

//         /// <summary>
//         /// Initialises the reader to work from an existing stream
//         /// </summary>
//         /// <param name="stream">Stream</param>
//         /// <param name="encoding">Encoding</param>
//         public CsvReader(Stream stream, Encoding encoding)
//         {
//             //_type = Type.Stream;
//             Initialise(stream, encoding);
//         }

//         /// <summary>
//         /// Initialises the reader to work from a csv string
//         /// </summary>
//         /// <param name="encoding"></param>
//         /// <param name="csvContent"></param>
//         public CsvReader(Encoding encoding, string csvContent)
//         {
//             //_type = Type.String;
//             Initialise(encoding, csvContent);  
//         }

//         #endregion Constructors

//         #region Methods

//         /// <summary>
//         /// Initialises the class to use a file
//         /// </summary>
//         /// <param name="filePath"></param>
//         /// <param name="encoding"></param>
//         private void Initialise(string filePath, Encoding encoding)
//         {
//             if (!File.Exists(filePath))
//                 throw new FileNotFoundException(string.Format("The file '{0}' does not exist.", filePath));

//             _fileStream = File.OpenRead(filePath);
//             Initialise(_fileStream, encoding);
//         }

//         /// <summary>
//         /// Initialises the class to use a stream
//         /// </summary>
//         /// <param name="stream"></param>
//         /// <param name="encoding"></param>
//         private void Initialise(Stream stream, Encoding encoding)
//         {
//             if (stream == null)
//                 throw new ArgumentNullException("The supplied stream is null.");

//             _stream = stream;
//             //_stream.Position = 0; //这边不能直接操作stream的position,length,之类的函数,因为我们用到了比如CryptoStream,直接不支持一些操作
//             _encoding = (encoding ?? Encoding.Default);
//             _streamReader = new StreamReader(_stream, _encoding);
//         }

//         /// <summary>
//         /// Initialies the class to use a string
//         /// </summary>
//         /// <param name="encoding"></param>
//         /// <param name="csvContent"></param>
//         private void Initialise(Encoding encoding, string csvContent)
//         {
//             if (csvContent == null)
//                 throw new ArgumentNullException("The supplied csvContent is null.");

//             _encoding = (encoding ?? Encoding.Default);

//             _memoryStream = new MemoryStream(csvContent.Length);
//             _streamWriter = new StreamWriter(_memoryStream);
//             _streamWriter.Write(csvContent);
//             _streamWriter.Flush();
//             _memoryStream.Seek(0, SeekOrigin.Begin);
//             Initialise(_memoryStream, encoding);   
//         }

//         /// <summary>
//         /// Reads the next record
//         /// </summary>
//         /// <returns>True if a record was successfuly read, otherwise false</returns>
//         public bool ReadNextRecord()
//         {
//             //Fields = null;
//             _Fields.Clear();
//             string line = _streamReader.ReadLine();

//             if (line == null)
//                 return false;

//             ParseLine(line);
//             return true;
//         }
               

//         /// <summary>
//         /// Parses a csv line
//         /// </summary>
//         /// <param name="line">Line</param>
//         private void ParseLine(string line)
//         {
//             lock(_columnBuilder)
//             {
//                 //Fields = new List<string>();
//                 Fields = new List<string>();
//                 bool inColumn = false;
//                 bool inQuotes = false;
//                 _columnBuilder.Remove(0, _columnBuilder.Length);

//                 // Iterate through every character in the line
//                 for (int i = 0; i < line.Length; i++)
//                 {
//                     char character = line[i];

//                     // If we are not currently inside a column
//                     if (!inColumn)
//                     {
//                         // If the current character is a double quote then the column value is contained within
//                         // double quotes, otherwise append the next character
//                         if (character == '"')
//                             inQuotes = true;
//                         else
//                         {
//                             if (character != ',')
//                             {
//                                 _columnBuilder.Append(character);
//                             }
//                             else
//                             {
//                                 inColumn = false;
//                                 Fields.Add(TrimColumns ? _columnBuilder.ToString().Trim() : _columnBuilder.ToString());
//                                 _columnBuilder.Remove(0, _columnBuilder.Length);
//                                 continue;
//                             }
//                         }
//                         inColumn = true;
//                         continue;
//                     }

//                     // If we are in between double quotes
//                     if (inQuotes)
//                     {
//                         // If the current character is a double quote and the next character is a comma or we are at the end of the line
//                         // we are now no longer within the column.
//                         // Otherwise increment the loop counter as we are looking at an escaped double quote e.g. "" within a column
//                         if (character == '"' && ((line.Length > (i + 1) && line[i + 1] == ',') || ((i + 1) == line.Length)))
//                         {
//                             inQuotes = false;
//                             inColumn = false;
//                             i++;
//                         }
//                         else if (character == '"' && line.Length > (i + 1) && line[i + 1] == '"')
//                             i++;
//                     }
//                     else if (character == ',')
//                     {
//                         inColumn = false;
//                     }

//                     // If we are no longer in the column clear the builder and add the columns to the list
//                     if (!inColumn)
//                     {
//                         Fields.Add(TrimColumns ? _columnBuilder.ToString().Trim() : _columnBuilder.ToString());
//                         _columnBuilder.Remove(0, _columnBuilder.Length);
//                     }
//                     else
//                     {
//                         // append the current column
//                         _columnBuilder.Append(character);
//                     }
//                 }

//                 //add by hj, fix last column is empty error
//                 if (line.Length > 0 && line[line.Length - 1] == ',')
//                 {
//                     Fields.Add("");
//                 }

//                 // If we are still inside a column add a new one
//                 if (inColumn)
//                 {
//                     Fields.Add(TrimColumns ? _columnBuilder.ToString().Trim() : _columnBuilder.ToString());
//                 }
//             }            
//         }

//         /// <summary>
//         /// Disposes of all unmanaged resources
//         /// </summary>
//         public void Dispose()
//         {
//             _Fields.Clear();

//             if (_streamReader != null)
//             {
//                 _streamReader.Close();
//                 _streamReader.Dispose();
//             }

//             if (_streamWriter != null)
//             {
//                 _streamWriter.Close();
//                 _streamWriter.Dispose();
//             }

//             if (_memoryStream != null)
//             {
//                 _memoryStream.Close();
//                 _memoryStream.Dispose();
//             }

//             if (_fileStream != null)
//             {
//                 _fileStream.Close();
//                 _fileStream.Dispose();
//             }

//             //if ((_type == Type.String || _type == Type.File) && _stream != null)
//             if (_stream != null)
//             {
//                 _stream.Close();
//                 _stream.Dispose();
//             }

//             _streamReader = null;
//             _streamWriter = null;
//             _memoryStream = null;
//             _fileStream = null;
//             _stream = null;
//         }

//         #endregion Methods

//     }
// }
