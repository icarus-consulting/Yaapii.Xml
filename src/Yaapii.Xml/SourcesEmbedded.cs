using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Fail;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// XSL sources embedded in an assembly.
    /// </summary>
    public class SourcesEmbedded : XmlResolver
    {
        private readonly IEnumerable<string> _folders;
        private readonly IScalar<Assembly> _asm;

        /// <summary>
        /// Sources (for example XSL files) embedded in a assembly.
        /// Specify folders 
        /// </summary>
        /// <param name="type">A class which is in the Assembly with the embedded resources</param>
        /// <param name="folders">folders with resources</param>
        public SourcesEmbedded(Type type, params string[] folders) : this(new ScalarOf<Assembly>(() => type.Assembly), folders)
        { }

        /// <summary>
        /// Sources (for example XSL files) embedded in a assembly.
        /// Specify folders 
        /// </summary>
        /// <param name="asm">Assembly with the embedded resources</param>
        /// <param name="folders">folders with resources</param>
        public SourcesEmbedded(Assembly asm, params string[] folders) : this(new ScalarOf<Assembly>(asm), folders)
        { }

        private SourcesEmbedded(IScalar<Assembly> asm, params string[] folders)
        {
            _folders = new Joined<string>(new EnumerableOf<string>(""), new EnumerableOf<string>(folders));
            _asm = asm;
        }


        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var res = Path.GetFileName(absoluteUri.AbsolutePath);
            var stream = Stream.Null;

            foreach (var folder in _folders)
            {
                try
                {
                    stream =
                        new ResourceOf(
                            folder != String.Empty ? folder + "/" + res : res,
                            _asm
                        ).Stream();
                }
                catch (Exception) {  /* go on... */ }
                if (stream != null && stream != Stream.Null) break;
            }

            if (stream == null || stream == Stream.Null)
            {
                throw
                    new UnsupportedOperationException(
                        new FormattedText(
                            "Cannot resolve stylesheet '{0}' XSL-sources currently pointing to assembly resources - use XSLDocument.With(XMLResolver) to use another.",
                            Path.GetFileName(absoluteUri.AbsoluteUri)
                        ).AsString()
                    );
            }

            return stream;
        }
    }
}
