using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DirectoryScanner.Core;
using Xunit;

namespace DirectoryScanner.Tests
{
    public class ScannerTests : IDisposable
    {
        private readonly string _testDirPath;
        private readonly Scanner _scanner;

        public ScannerTests()
        {
            _testDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirPath);
            _scanner = new Scanner();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirPath))
            {
                Directory.Delete(_testDirPath, true);
            }
        }

        [Fact]
        public async Task ScanDirectory_EmptyFolder_ReturnsRootNodeWithSizeZero()
        {
            var result = await _scanner.ScanDirectory(_testDirPath, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(0, result.Size);
            Assert.Equal(NodeType.Directory, result.Type);
            Assert.Empty(result.Children);
        }

        [Fact]
        public async Task ScanDirectory_FileExists_CalculatesCorrectSize()
        {
            string filePath = Path.Combine(_testDirPath, "test.txt");
            string content = "Hello, World!";
            File.WriteAllText(filePath, content);

            var result = await _scanner.ScanDirectory(_testDirPath, CancellationToken.None);

            var fileNode = Assert.Single(result.Children);
            Assert.Equal("test.txt", fileNode.Name);
            Assert.Equal(content.Length, fileNode.Size);
        }

        [Fact]
        public async Task ScanDirectory_NestedFolders_BuildsCorrectHierarchy()
        {
            string pathA = Path.Combine(_testDirPath, "A");
            string pathB = Path.Combine(pathA, "B");
            Directory.CreateDirectory(pathB);

            string filePath = Path.Combine(pathB, "test.txt");
            string content = "test content";
            File.WriteAllText(filePath, content);

            var result = await _scanner.ScanDirectory(_testDirPath, CancellationToken.None);

            var nodeA = result.Children.FirstOrDefault(n => n.Name == "A");
            Assert.NotNull(nodeA);

            var nodeB = nodeA.Children.FirstOrDefault(n => n.Name == "B");
            Assert.NotNull(nodeB);

            var fileNode = nodeB.Children.FirstOrDefault(n => n.Name == "test.txt");
            Assert.NotNull(fileNode);
            Assert.Equal(content.Length, fileNode.Size);
        }

        [Fact]
        public async Task ScanDirectory_Cancelled_ThrowsOperationCanceledException()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel(); 

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await _scanner.ScanDirectory(_testDirPath, cts.Token));
        }

        [Fact]
        public async Task ScanDirectory_SymbolicLink_SizeIsZeroOrIgnored()
        {
            string targetDir = Path.Combine(_testDirPath, "TargetDir");
            Directory.CreateDirectory(targetDir);
            string linkPath = Path.Combine(_testDirPath, "MyLink");

            try
            {
                File.CreateSymbolicLink(linkPath, targetDir);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                return;
            }

            var result = await _scanner.ScanDirectory(_testDirPath, CancellationToken.None);

            var linkNode = result.Children.FirstOrDefault(n => n.Name == "MyLink");
            Assert.Null(linkNode); 
        }
    }
}