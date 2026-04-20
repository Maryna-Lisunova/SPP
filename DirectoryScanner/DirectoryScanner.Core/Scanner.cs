using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryScanner.Core
{
    public class Scanner
    {
        private const int MaxDegreeOfParallelism = 4;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);
        private CountdownEvent _countdown;

        public async Task<FileSystemNode> ScanDirectory(string rootPath, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Путь не может быть пустым.", nameof(rootPath));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Папка не найдена: {rootPath}");

            var rootNode = new FileSystemNode { Path = rootPath, Name = new DirectoryInfo(rootPath).Name, Type = NodeType.Directory };
            _countdown = new CountdownEvent(1);
            ThreadPool.QueueUserWorkItem(_ => ProcessDirectory(rootNode, rootPath, token));
            await Task.Run(() => _countdown.Wait(token), token);
            CalculateRecursiveSizes(rootNode);
            return rootNode;
        }

        private void ProcessDirectory(FileSystemNode parentNode, string path, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested) return;

                _semaphore.Wait(token);

                try
                {
                    var dirInfo = new DirectoryInfo(path);
                    foreach (var file in dirInfo.EnumerateFiles())
                    {
                        if (file.LinkTarget != null) continue;

                        var fileNode = new FileSystemNode
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Size = file.Length,
                            Type = NodeType.File
                        };
                        parentNode.Children.Add(fileNode);
                    }

                    foreach (var subDir in dirInfo.EnumerateDirectories())
                    {
                        if (subDir.LinkTarget != null) continue;

                        var dirNode = new FileSystemNode
                        {
                            Name = subDir.Name,
                            Path = subDir.FullName,
                            Type = NodeType.Directory
                        };

                        parentNode.Children.Add(dirNode);
                        _countdown.AddCount();
                        ThreadPool.QueueUserWorkItem(_ => ProcessDirectory(dirNode, subDir.FullName, token));
                    }
                }
                finally
                {
                    _semaphore.Release();
                    _countdown.Signal();
                }
            }
            catch (UnauthorizedAccessException) { /* ignore */ }
            catch (Exception) { /* ignore */ }
        }

        private long CalculateRecursiveSizes(FileSystemNode node)
        {
            if (node.Type == NodeType.File) 
            { 
                return node.Size; 
            }
            long totalSize = 0;
            foreach (var child in node.Children)
            {
                totalSize += CalculateRecursiveSizes(child);
            }
            node.Size = totalSize;
            return totalSize;
        }
    }
}
