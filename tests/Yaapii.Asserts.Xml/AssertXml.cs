//using System;
//using System.Xml;
//using Yaapii.Atoms.Enumerable;
//using Yaapii.Atoms.Text;
//using Yaapii.Xml;

//namespace Yaapii.Asserts.Xml
//{
//    public static class AssertXml
//    {
//        public static void HasNode(string doc, string xpath)
//        {
//            AssertXml.HasNode(new XMLQuery(doc), xpath);
//        }

//        public static void HasNode(XmlDocument doc, string xpath)
//        {
//            HasNode(new XMLQuery(doc.OuterXml), xpath);
//        }

//        public static void HasNode(IXML doc, string xpath)
//        {
//            if (new LengthOf(doc.Nodes(xpath)).Value() == 0)
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' NOT FOUND in document: {1}",
//                            xpath,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }
//        }

//        public static void HasNoNode(IXML doc, string xpath)
//        {
//            if (new LengthOf(doc.Nodes(xpath)).Value() != 0)
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' FOUND in document: {1}",
//                            xpath,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }
//        }

//        public static void FirstValueEquals(IXML doc, string xpath, string expected)
//        {
//            if (doc.Values(xpath).Count == 0)
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' had no results in document: {1}",
//                            xpath,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }

//            var val = doc.Values(xpath)[0];
//            if (!val.Equals(expected))
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' is '{1}' instead of {2} in document: {3}",
//                            xpath,
//                            val,
//                            expected,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }
//        }

//        public static void True(XmlDocument doc,string xpath)
//        {
//            True(new XMLQuery(doc.OuterXml), xpath);
//        }

//        public static void True(IXML doc, string xpath)
//        {
//            if (new LengthOf(doc.Values(xpath)).Value() == 0)
//            {
//                throw
//                   new ExpectationMissedException(
//                       new FormattedText(
//                           "xpath '{0}' not found in document: {1}",
//                           xpath,
//                           doc.ToString()
//                       ).AsString()
//                   );
//            }

//            if(new ItemAt<string>(doc.Values(xpath)).Value() != "True")
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' result is 'False' in document: {1}",
//                            xpath,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }
//        }

//        public static void False(XmlDocument doc, string xpath)
//        {
//            False(new XMLQuery(doc.OuterXml), xpath);
//        }

//        public static void False(IXML doc, string xpath)
//        {
//            if (new LengthOf(doc.Values(xpath)).Value() == 0)
//            {
//                throw
//                   new ExpectationMissedException(
//                       new FormattedText(
//                           "xpath '{0}' not found in document: {1}",
//                           xpath,
//                           doc.ToString()
//                       ).AsString()
//                   );
//            }

//            if (new ItemAt<string>(doc.Values(xpath)).Value() != "True")
//            {
//                throw
//                    new ExpectationMissedException(
//                        new FormattedText(
//                            "xpath '{0}' result is 'True' in document: {1}",
//                            xpath,
//                            doc.ToString()
//                        ).AsString()
//                    );
//            }
//        }
//    }
//}
