// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
    /// <summary>
    /// A tidy file list which will be created in a temp folder located in the current folder.
    /// It invokes an action. 
    /// Finaly the root directory and all files will be deleted.
    /// </summary>
    public sealed class TidyFileList : IAction
    {
        private readonly IScalar<string> _root;
        private readonly IDictionary<string, string> _files;
        private readonly Action _action;

        /// <summary>
        /// A tidy file list which will be created in a temp folder located in the current folder.
        /// It invokes an action. 
        /// Finaly the root directory and all files will be deleted.
        /// </summary>
        /// <param name="files">Files with key as relative path to the root directory and value as file content</param>
        public TidyFileList(IDictionary<string, string> files, Action action) : this(
            new ScalarOf<string>(() =>
                Path.Combine(Directory.GetCurrentDirectory(), "TmpFileList")),
            files,
            action)
        { }

        /// <summary>
        /// A tidy file list which will be created in a temp folder located in the current folder.
        /// It invokes an action. 
        /// Finaly the root directory and all files will be deleted.
        /// </summary>
        /// <param name="root">The root directory in where the files (and subfolders) will be created</param>
        /// <param name="files">Files with key as relative path to the root directory and value as file content</param>
        public TidyFileList(string root, IDictionary<string, string> files, Action action) : this(
            new ScalarOf<string>(root),
            files,
            action)
        { }

        /// <summary>
        /// A tidy file list which will be created in the filesystem and invokes an action. 
        /// Finaly the root directory and all files will be deleted.
        /// </summary>
        /// <param name="root">The root directory in where the files (and subfolders) will be created</param>
        /// <param name="files">Files with key as relative path to the root directory and value as file content</param>
        public TidyFileList(IScalar<string> root, IDictionary<string, string> files, Action action)
        {
            this._root = root;
            this._files = files;
            this._action = action;
        }

        public void Invoke()
        {
            this.Destroy();
            this.Create();
            try
            {
                this._action.Invoke();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.Destroy();
            }
        }
        /// <summary>
        /// Deletes the file tree in the file system
        /// </summary>
        private void Destroy()
        {
            if (Directory.Exists(this._root.Value()))
            {
                Directory.Delete(this._root.Value(), true);
            }
        }

        /// <summary>
        /// Creates the file tree in the file system
        /// </summary>
        /// <returns></returns>
        private void Create()
        {
            Directory.CreateDirectory(this._root.Value());

            foreach (var file in this._files)
            {
                // create directory
                var idx = file.Key.LastIndexOf('\\');
                if (idx >= 0)
                {
                    Directory.CreateDirectory(
                        Path.Combine(
                            this._root.Value(),
                            file.Key.Substring(0, idx)));
                }

                // create file
                new LengthOf(
                    new TeeInput(
                        new InputOf(file.Value),
                        new OutputTo(
                            Path.Combine(
                                this._root.Value(),
                                file.Key)))
                ).Value();
            }
        }
    }
}
