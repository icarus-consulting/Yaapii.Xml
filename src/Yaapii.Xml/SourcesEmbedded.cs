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
        private readonly IEnumerable<string> folders;
        private readonly IScalar<Assembly> asm;

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
            this.folders = new Joined<string>(new EnumerableOf<string>(""), new EnumerableOf<string>(folders));
            this.asm = asm;
        }

        /// <summary>
        /// Access embedded entity.
        /// </summary>
        /// <param name="absoluteUri">absolute uri to the referenced file</param>
        /// <param name="role">unused in this implementation</param>
        /// <param name="ofObjectToReturn">type of the object</param>
        /// <returns>The requested entity</returns>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var res = Path.GetFileName(absoluteUri.AbsolutePath);
            var stream = Stream.Null;

            foreach (var folder in folders)
            {
                try
                {
                    stream =
                        new ResourceOf(
                            folder != String.Empty ? folder + "/" + res : res,
                            asm
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
