using System;
using System.Collections.Generic;
using System.IO;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
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
        /// <param name="files"></param>
        /// <param name="action"></param>
        public TidyFileList(IDictionary<string, string> files, Action action) : this(
            new StickyScalar<string>(() =>
                Path.Combine(Directory.GetCurrentDirectory(), "TmpFileList")),
            files,
            action)
        { }

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
        /// <param name="action"></param>
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
