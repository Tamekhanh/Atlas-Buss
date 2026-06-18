using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStorageProvider 
{
    Task<string> SaveFileAsync(Stream fileStream, string folderName, string fileName);
    Task<Stream> GetFileAsync(string relativePath);
}