/*https://stackoverflow.com/a/15683147/2948523*/

namespace CompareDirectoryHash
{
    internal class FileModel
    {
        public string Path { get; set; }
        public string Name { get; internal set; }
        public string Extension { get; set; }
    }
}