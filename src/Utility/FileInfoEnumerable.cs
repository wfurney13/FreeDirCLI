using System.Collections;

namespace FreeDirCLI
{
    public class FileInfoEnumerable : IEnumerable<long>
    {
        private readonly DirectoryInfo _root;

        public FileInfoEnumerable(DirectoryInfo root)
        {
            _root = root;
        }

        public IEnumerator<long> GetEnumerator()
        {
            if (_root == null || !_root.Exists)
                yield break;

            IEnumerable<FileInfo> matches = new List<FileInfo>();
            try
            {
                matches = matches.Concat(
                    _root.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                );
            }
            catch (UnauthorizedAccessException)
            {
                SizeGatherer.UnauthorizedAccessExceptionFileCount++;
                yield break;
            }
            catch (PathTooLongException)
            {
                yield break;
            }
            catch (System.IO.IOException)
            {
                yield break;
            }

            //return all the file matches
            foreach (var file in matches)
            {
                yield return file.Length;
            }

            //return all the directory matches
            foreach (var dir in _root.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                IEnumerable<long> fileInfosLengths = new FileInfoEnumerable(dir);
                foreach (var fileLength in fileInfosLengths)
                {
                    yield return fileLength;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
