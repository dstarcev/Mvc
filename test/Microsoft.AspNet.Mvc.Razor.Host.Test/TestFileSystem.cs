// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class TestFileSystem : ICachedFileSystem
    {
        private readonly Dictionary<string, IFileInfo> _lookup =
            new Dictionary<string, IFileInfo>(StringComparer.Ordinal);

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            throw new NotImplementedException();
        }

        public void AddFile(string path, string contents)
        {
            var fileInfo = new TestFileInfo
            {
                Content = contents,
                PhysicalPath = path,
                Name = Path.GetFileName(path),
                LastModified = DateTime.UtcNow,
            };

            AddFile(path, fileInfo);
        }

        public void AddFile(string path, TestFileInfo contents)
        {
            _lookup.Add(path, contents);
        }

        public void DeleteFile(string path)
        {
            _lookup.Remove(path);
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            return _lookup.TryGetValue(subpath, out fileInfo);
        }

        public bool TryGetParentPath(string subpath, out string parentPath)
        {
            parentPath = Path.GetDirectoryName(subpath);
            return !string.IsNullOrEmpty(parentPath);
        }
    }
}