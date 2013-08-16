using System;
using System.Runtime.CompilerServices;

namespace Avm
{
    /// <summary>
    /// The XML class contains methods and properties for working with XML objects. The XML class
    /// (along with the XMLList, Namespace, and QName classes) implements the
    /// powerful XML-handling standards defined in ECMAScript for XML
    /// (E4X) specification (ECMA-357 edition 2).
    /// </summary>
    [PageFX.AbcInstance(173)]
    [PageFX.ABC]
    [PageFX.QName("XML")]
    [PageFX.FP9]
    public class XML : Avm.Object
    {
        /// <summary>
        /// Determines whether XML comments are ignored
        /// when XML objects parse the source XML data. By default, the comments are ignored
        /// (true). To include XML comments, set this property to false.
        /// The ignoreComments property is used only during the XML parsing, not during
        /// the call to any method such as myXMLObject.child(*).toXMLString().
        /// If the source XML includes comment nodes, they are kept or discarded during the XML parsing.
        /// </summary>
        public extern static bool ignoreComments
        {
            [PageFX.AbcClassTrait(1)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcClassTrait(2)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Determines whether XML
        /// processing instructions are ignored when XML objects parse the source XML data.
        /// By default, the processing instructions are ignored (true). To include XML
        /// processing instructions, set this property to false. The
        /// ignoreProcessingInstructions property is used only during the XML parsing,
        /// not during the call to any method such as myXMLObject.child(*).toXMLString().
        /// If the source XML includes processing instructions nodes, they are kept or discarded during
        /// the XML parsing.
        /// </summary>
        public extern static bool ignoreProcessingInstructions
        {
            [PageFX.AbcClassTrait(3)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcClassTrait(4)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Determines whether white space characters
        /// at the beginning and end of text nodes are ignored during parsing. By default,
        /// white space is ignored (true). If a text node is 100% white space and the
        /// ignoreWhitespace property is set to true, then the node is not created.
        /// To show white space in a text node, set the ignoreWhitespace property to
        /// false.
        /// </summary>
        public extern static bool ignoreWhitespace
        {
            [PageFX.AbcClassTrait(5)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcClassTrait(6)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Determines whether the toString()
        /// and toXMLString() methods normalize white space characters between some tags.
        /// The default value is true.
        /// </summary>
        public extern static bool prettyPrinting
        {
            [PageFX.AbcClassTrait(7)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcClassTrait(8)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Determines the amount of indentation applied by
        /// the toString() and toXMLString() methods when
        /// the XML.prettyPrinting property is set to true.
        /// Indentation is applied with the space character, not the tab character.
        /// The default value is 2.
        /// </summary>
        public extern static int prettyIndent
        {
            [PageFX.AbcClassTrait(9)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcClassTrait(10)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern XML(object value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern XML();

        /// <summary>
        /// Returns a string representation of the XML object. The rules for this conversion depend on whether
        /// the XML object has simple content or complex content:
        /// If the XML object has simple content, toString() returns the String contents of the
        /// XML object with  the following stripped out: the start tag, attributes, namespace declarations, and
        /// end tag. If the XML object has complex content, toString() returns an XML encoded String
        /// representing the entire XML object, including the start tag, attributes, namespace declarations,
        /// and end tag.To return the entire XML object every time, use toXMLString().
        /// </summary>
        /// <returns>The string representation of the XML object.</returns>
        [PageFX.AbcInstanceTrait(0)]
        [PageFX.ABC]
        [PageFX.QName("toString", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.String toString();

        /// <summary>Checks to see whether the object has the property specified by the p parameter.</summary>
        /// <param name="p">The property to match.</param>
        /// <returns>If the property exists, true; otherwise false.</returns>
        [PageFX.AbcInstanceTrait(1)]
        [PageFX.ABC]
        [PageFX.QName("hasOwnProperty", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override bool hasOwnProperty(object p);

        /// <summary>
        /// Checks whether the property p is in the set of properties that can be iterated in a
        /// for..in statement applied to the XML object. Returns true only
        /// if toString(p) == &quot;0&quot;.
        /// </summary>
        /// <param name="p">The property that you want to check.</param>
        /// <returns>
        /// If the property can be iterated in a for..in statement, true;
        /// otherwise, false.
        /// </returns>
        [PageFX.AbcInstanceTrait(2)]
        [PageFX.ABC]
        [PageFX.QName("propertyIsEnumerable", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override bool propertyIsEnumerable(object p);

        /// <summary>
        /// Adds a namespace to the set of in-scope namespaces for the XML object. If the namespace already
        /// exists in the in-scope namespaces for the XML object (with a prefix matching that of the given
        /// parameter), then the prefix of the existing namespace is set to undefined. If the input parameter
        /// is a Namespace object, it&apos;s used directly. If it&apos;s a QName object, the input parameter&apos;s
        /// URI is used to create a new namespace; otherwise, it&apos;s converted to a String and a namespace is created from
        /// the String.
        /// </summary>
        /// <param name="ns">The namespace to add to the XML object.</param>
        /// <returns>The new XML object, with the namespace added.</returns>
        [PageFX.AbcInstanceTrait(3)]
        [PageFX.ABC]
        [PageFX.QName("addNamespace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML addNamespace(object ns);

        /// <summary>
        /// Appends the given child to the end of the XML object&apos;s properties.
        /// The appendChild() method takes an XML object, an XMLList object, or
        /// any other data type that is then converted to a String.
        /// Use the delete (XML) operator to remove XML nodes.
        /// </summary>
        /// <param name="child">The XML object to append.</param>
        /// <returns>The resulting XML object.</returns>
        [PageFX.AbcInstanceTrait(4)]
        [PageFX.ABC]
        [PageFX.QName("appendChild", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML appendChild(object child);

        /// <summary>
        /// Returns the XML value of the attribute that has the name matching the attributeName
        /// parameter. Attributes are found within XML elements.
        /// In the following example, the element has an attribute named &quot;gender&quot;
        /// with the value &quot;boy&quot;: &lt;first gender=&quot;boy&quot;&gt;John&lt;/first&gt;.
        /// The attributeName parameter can be any data type; however,
        /// String is the most common data type to use. When passing any object other than a QName object,
        /// the attributeName parameter uses the toString() method
        /// to convert the parameter to a string. If you need a qualified name reference, you can pass in a QName object. A QName object
        /// defines a namespace and the local name, which you can use to define the qualified name of an
        /// attribute. Therefore calling attribute(qname) is not the same as calling
        /// attribute(qname.toString()).
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>
        /// An XMLList object or an empty XMLList object. Returns an empty XMLList object
        /// when an attribute value has not been defined.
        /// </returns>
        [PageFX.AbcInstanceTrait(5)]
        [PageFX.ABC]
        [PageFX.QName("attribute", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList attribute(object attributeName);

        /// <summary>
        /// Returns a list of attribute values for the given XML object. Use the name()
        /// method with the attributes() method to return the name of an attribute.
        /// Use @* to return the names of all attributes.
        /// </summary>
        /// <returns>The list of attribute values.</returns>
        [PageFX.AbcInstanceTrait(6)]
        [PageFX.ABC]
        [PageFX.QName("attributes", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList attributes();

        /// <summary>
        /// Lists the children of an XML object. An XML child is an XML element, text node, comment,
        /// or processing instruction.
        /// Use the propertyName parameter to list the
        /// contents of a specific XML child. For example, to return the contents of a child named
        /// &lt;first&gt;, use child.name(&quot;first&quot;). You can generate the same result
        /// by using the child&apos;s index number. The index number identifies the child&apos;s position in the
        /// list of other XML children. For example, name.child(0) returns the first child
        /// in a list. Use an asterisk (*) to output all the children in an XML document.
        /// For example, doc.child(&quot;*&quot;).Use the length() method with the asterisk (*) parameter of the
        /// child() method to output the total number of children. For example,
        /// numChildren = doc.child(&quot;*&quot;).length().
        /// </summary>
        /// <param name="propertyName">The element name or integer of the XML child.</param>
        /// <returns>An XMLList object of child nodes that match the input parameter.</returns>
        [PageFX.AbcInstanceTrait(7)]
        [PageFX.ABC]
        [PageFX.QName("child", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList child(object propertyName);

        /// <summary>Identifies the zero-indexed position of this XML object within the context of its parent.</summary>
        /// <returns>The position of the object. Returns -1 as well as positive integers.</returns>
        [PageFX.AbcInstanceTrait(8)]
        [PageFX.ABC]
        [PageFX.QName("childIndex", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual int childIndex();

        /// <summary>
        /// Lists the children of the XML object in the sequence in which they appear. An XML child
        /// is an XML element, text node, comment, or processing instruction.
        /// </summary>
        /// <returns>An XMLList object of the XML object&apos;s children.</returns>
        [PageFX.AbcInstanceTrait(9)]
        [PageFX.ABC]
        [PageFX.QName("children", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList children();

        /// <summary>Lists the properties of the XML object that contain XML comments.</summary>
        /// <returns>An XMLList object of the properties that contain comments.</returns>
        [PageFX.AbcInstanceTrait(10)]
        [PageFX.ABC]
        [PageFX.QName("comments", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList comments();

        /// <summary>Compares the XML object against the given value parameter.</summary>
        /// <param name="value">A value to compare against the current XML object.</param>
        /// <returns>If the XML object matches the value parameter, then true; otherwise false.</returns>
        [PageFX.AbcInstanceTrait(11)]
        [PageFX.ABC]
        [PageFX.QName("contains", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual bool contains(object value);

        /// <summary>
        /// Returns a copy of the given XML object. The copy is a duplicate of the entire tree of nodes.
        /// The copied XML object has no parent and returns null if you attempt to call the
        /// parent() method.
        /// </summary>
        /// <returns>The copy of the object.</returns>
        [PageFX.AbcInstanceTrait(12)]
        [PageFX.ABC]
        [PageFX.QName("copy", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML copy();

        /// <summary>
        /// Returns all descendants (children, grandchildren, great-grandchildren, and so on) of the
        /// XML object that have the given name parameter. The name parameter
        /// is optional. The name parameter can be a QName object, a String data type
        /// or any other data type that is then converted to a String data type.
        /// To return all descendants, use the &quot;*&quot; parameter. If no parameter is passed,
        /// the string &quot;*&quot; is passed and returns all descendants of the XML object.
        /// </summary>
        /// <param name="name">(default = *)  The name of the element to match.</param>
        /// <returns>
        /// An XMLList object of matching descendants. If there are no descendants, returns an
        /// empty XMLList object.
        /// </returns>
        [PageFX.AbcInstanceTrait(13)]
        [PageFX.ABC]
        [PageFX.QName("descendants", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList descendants(object name);

        [PageFX.AbcInstanceTrait(13)]
        [PageFX.ABC]
        [PageFX.QName("descendants", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Avm.XMLList descendants();

        /// <summary>
        /// Lists the elements of an XML object. An element consists of a start and an end tag;
        /// for example &lt;first&gt;&lt;/first&gt;. The name parameter
        /// is optional. The name parameter can be a QName object, a String data type,
        /// or any other data type that is then converted to a String data type. Use the name parameter to list a specific element. For example,
        /// the element &quot;first&quot; returns &quot;John&quot; in this example:
        /// &lt;first&gt;John&lt;/first&gt;.
        /// To list all elements, use the asterisk (*) as the
        /// parameter. The asterisk is also the default parameter. Use the length() method with the asterisk parameter to output the total
        /// number of elements. For example, numElement = addressbook.elements(&quot;*&quot;).length().
        /// </summary>
        /// <param name="name">
        /// (default = *)  The name of the element. An element&apos;s name is surrounded by angle brackets.
        /// For example, &quot;first&quot; is the name in this example:
        /// &lt;first&gt;&lt;/first&gt;.
        /// </param>
        /// <returns>
        /// An XMLList object of the element&apos;s content. The element&apos;s content falls between the start and
        /// end tags. If you use the asterisk (*) to call all elements, both the
        /// element&apos;s tags and content are returned.
        /// </returns>
        [PageFX.AbcInstanceTrait(14)]
        [PageFX.ABC]
        [PageFX.QName("elements", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList elements(object name);

        [PageFX.AbcInstanceTrait(14)]
        [PageFX.ABC]
        [PageFX.QName("elements", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Avm.XMLList elements();

        /// <summary>
        /// Checks to see whether the XML object contains complex content. An XML object contains complex content if
        /// it has child elements. XML objects that representing attributes, comments, processing instructions,
        /// and text nodes do not have complex content. However, an object that contains these can
        /// still be considered to contain complex content (if the object has child elements).
        /// </summary>
        /// <returns>If the XML object contains complex content, true; otherwise false.</returns>
        [PageFX.AbcInstanceTrait(15)]
        [PageFX.ABC]
        [PageFX.QName("hasComplexContent", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual bool hasComplexContent();

        /// <summary>
        /// Checks to see whether the XML object contains simple content. An XML object contains simple content
        /// if it represents a text node, an attribute node, or an XML element that has no child elements.
        /// XML objects that represent comments and processing instructions do not contain simple
        /// content.
        /// </summary>
        /// <returns>If the XML object contains simple content, true; otherwise false.</returns>
        [PageFX.AbcInstanceTrait(16)]
        [PageFX.ABC]
        [PageFX.QName("hasSimpleContent", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual bool hasSimpleContent();

        /// <summary>Lists the namespaces for the XML object, based on the object&apos;s parent.</summary>
        /// <returns>An array of Namespace objects.</returns>
        [PageFX.AbcInstanceTrait(17)]
        [PageFX.ABC]
        [PageFX.QName("inScopeNamespaces", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.Array inScopeNamespaces();

        /// <summary>
        /// Inserts the given child2 parameter after the child1 parameter in this XML object and returns the
        /// resulting object. If the child1 parameter is null, the method
        /// inserts the contents of child2before all children of the XML object
        /// (in other words, after none). If child1 is provided, but it does not
        /// exist in the XML object, the XML object is not modified and undefined is
        /// returned.
        /// If you call this method on an XML child that is not an element (text, attributes, comments, pi, and so on)
        /// undefined is returned.Use the delete (XML) operator to remove XML nodes.
        /// </summary>
        /// <param name="child1">The object in the source object that you insert before child2.</param>
        /// <param name="child2">The object to insert.</param>
        /// <returns>The resulting XML object or undefined.</returns>
        [PageFX.AbcInstanceTrait(18)]
        [PageFX.ABC]
        [PageFX.QName("insertChildAfter", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object insertChildAfter(object child1, object child2);

        /// <summary>
        /// Inserts the given child2 parameter before the child1 parameter
        /// in this XML object and returns the resulting object. If the child1 parameter
        /// is null, the method inserts the contents of
        /// child2after all children of the XML object (in other words, before
        /// none). If child1 is provided, but it does not exist in the XML object,
        /// the XML object is not modified and undefined is returned.
        /// If you call this method on an XML child that is not an element (text, attributes,
        /// comments, pi, and so on) undefined is returned.Use the delete (XML) operator to remove XML nodes.
        /// </summary>
        /// <param name="child1">The object in the source object that you insert after child2.</param>
        /// <param name="child2">The object to insert.</param>
        /// <returns>The resulting XML object or undefined.</returns>
        [PageFX.AbcInstanceTrait(19)]
        [PageFX.ABC]
        [PageFX.QName("insertChildBefore", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object insertChildBefore(object child1, object child2);

        /// <summary>
        /// For XML objects, this method always returns the integer 1.
        /// The length() method of the XMLList class returns a value of 1 for
        /// an XMLList object that contains only one value.
        /// </summary>
        /// <returns>Always returns 1 for any XML object.</returns>
        [PageFX.AbcInstanceTrait(20)]
        [PageFX.ABC]
        [PageFX.QName("length", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual int length();

        /// <summary>Gives the local name portion of the qualified name of the XML object.</summary>
        /// <returns>The local name as either a String or null.</returns>
        [PageFX.AbcInstanceTrait(21)]
        [PageFX.ABC]
        [PageFX.QName("localName", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object localName();

        /// <summary>Gives the qualified name for the XML object.</summary>
        /// <returns>The qualified name is either a QName or null.</returns>
        [PageFX.AbcInstanceTrait(22)]
        [PageFX.ABC]
        [PageFX.QName("name", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object name();

        /// <summary>
        /// If no parameter is provided, gives the namespace associated with the qualified name of
        /// this XML object. If a prefix parameter is specified, the method returns the namespace
        /// that matches the prefix parameter and that is in scope for the XML object. If there is no
        /// such namespace, the method returns undefined.
        /// </summary>
        /// <param name="prefix">(default = null)  The prefix you want to match.</param>
        /// <returns>Returns null, undefined, or a namespace.</returns>
        [PageFX.AbcInstanceTrait(24)]
        [PageFX.ABC]
        [PageFX.QName("namespace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object @namespace(object prefix);

        [PageFX.AbcInstanceTrait(24)]
        [PageFX.ABC]
        [PageFX.QName("namespace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern object @namespace();

        /// <summary>Lists namespace declarations associated with the XML object in the context of its parent.</summary>
        /// <returns>An array of Namespace objects.</returns>
        [PageFX.AbcInstanceTrait(25)]
        [PageFX.ABC]
        [PageFX.QName("namespaceDeclarations", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.Array namespaceDeclarations();

        /// <summary>
        /// Specifies the type of node: text, comment, processing-instruction,
        /// attribute, or element.
        /// </summary>
        /// <returns>The node type used.</returns>
        [PageFX.AbcInstanceTrait(26)]
        [PageFX.ABC]
        [PageFX.QName("nodeKind", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.String nodeKind();

        /// <summary>
        /// For the XML object and all descendant XML objects, merges adjacent text nodes and
        /// eliminates empty text nodes.
        /// </summary>
        /// <returns>The resulting normalized XML object.</returns>
        [PageFX.AbcInstanceTrait(27)]
        [PageFX.ABC]
        [PageFX.QName("normalize", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML normalize();

        /// <summary>
        /// Returns the parent of the XML object. If the XML object has no parent, the method returns
        /// undefined.
        /// </summary>
        /// <returns>The parent XML object. Returns either a String or null.</returns>
        [PageFX.AbcInstanceTrait(28)]
        [PageFX.ABC]
        [PageFX.QName("parent", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object parent();

        /// <summary>
        /// If a name parameter is provided, lists all the children of the XML object
        /// that contain processing instructions with that name. With no parameters, the method
        /// lists all the children of the XML object that contain any processing instructions.
        /// </summary>
        /// <param name="name">(default = &quot;*&quot;)  The name of the processing instructions to match.</param>
        /// <returns>A list of matching child objects.</returns>
        [PageFX.AbcInstanceTrait(29)]
        [PageFX.ABC]
        [PageFX.QName("processingInstructions", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList processingInstructions(object name);

        [PageFX.AbcInstanceTrait(29)]
        [PageFX.ABC]
        [PageFX.QName("processingInstructions", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Avm.XMLList processingInstructions();

        /// <summary>
        /// Inserts a copy of the provided child object into the XML element before any existing XML
        /// properties for that element.
        /// Use the delete (XML) operator to remove XML nodes.
        /// </summary>
        /// <param name="value">The object to insert.</param>
        /// <returns>The resulting XML object.</returns>
        [PageFX.AbcInstanceTrait(30)]
        [PageFX.ABC]
        [PageFX.QName("prependChild", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML prependChild(object value);

        /// <summary>
        /// Removes the given namespace for this object and all descendants. The removeNamespaces()
        /// method does not remove a namespace if it is referenced by the object&apos;s qualified name or the
        /// qualified name of the object&apos;s attributes.
        /// </summary>
        /// <param name="ns">The namespace to remove.</param>
        /// <returns>A copy of the resulting XML object.</returns>
        [PageFX.AbcInstanceTrait(31)]
        [PageFX.ABC]
        [PageFX.QName("removeNamespace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML removeNamespace(object ns);

        /// <summary>
        /// Replaces the properties specified by the propertyName parameter
        /// with the given value parameter.
        /// If no properties match propertyName, the XML object is left unmodified.
        /// </summary>
        /// <param name="propertyName">
        /// Can be a
        /// numeric value, an unqualified name for a set of XML elements, a qualified name for a set of
        /// XML elements, or the asterisk wildcard (&quot;*&quot;).
        /// Use an unqualified name to identify XML elements in the default namespace.
        /// </param>
        /// <param name="value">
        /// The replacement value. This can be an XML object, an XMLList object, or any value
        /// that can be converted with toString().
        /// </param>
        /// <returns>The resulting XML object, with the matching properties replaced.</returns>
        [PageFX.AbcInstanceTrait(32)]
        [PageFX.ABC]
        [PageFX.QName("replace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML replace(object propertyName, object value);

        /// <summary>
        /// Replaces the child properties of the XML object with the specified set of XML properties,
        /// provided in the value parameter.
        /// </summary>
        /// <param name="value">The replacement XML properties. Can be a single XML object or an XMLList object.</param>
        /// <returns>The resulting XML object.</returns>
        [PageFX.AbcInstanceTrait(33)]
        [PageFX.ABC]
        [PageFX.QName("setChildren", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML setChildren(object value);

        /// <summary>Changes the local name of the XML object to the given name parameter.</summary>
        /// <param name="name">The replacement name for the local name.</param>
        [PageFX.AbcInstanceTrait(34)]
        [PageFX.ABC]
        [PageFX.QName("setLocalName", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual void setLocalName(object name);

        /// <summary>Sets the name of the XML object to the given qualified name or attribute name.</summary>
        /// <param name="name">The new name for the object.</param>
        [PageFX.AbcInstanceTrait(35)]
        [PageFX.ABC]
        [PageFX.QName("setName", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual void setName(object name);

        /// <summary>Sets the namespace associated with the XML object.</summary>
        /// <param name="ns">The new namespace.</param>
        [PageFX.AbcInstanceTrait(36)]
        [PageFX.ABC]
        [PageFX.QName("setNamespace", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual void setNamespace(object ns);

        /// <summary>Returns an XMLList object of all XML properties of the XML object that represent XML text nodes.</summary>
        /// <returns>The list of properties.</returns>
        [PageFX.AbcInstanceTrait(37)]
        [PageFX.ABC]
        [PageFX.QName("text", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XMLList text();

        /// <summary>
        /// Returns a string representation of the XML object. Unlike the toString() method,
        /// the toXMLString() method always returns the start tag, attributes,
        /// and end tag of the XML object, regardless of whether the XML object has simple content or complex
        /// content. (The toString() method strips out these items for XML objects that contain
        /// simple content.)
        /// </summary>
        /// <returns>The string representation of the XML object.</returns>
        [PageFX.AbcInstanceTrait(38)]
        [PageFX.ABC]
        [PageFX.QName("toXMLString", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.String toXMLString();

        [PageFX.AbcInstanceTrait(39)]
        [PageFX.ABC]
        [PageFX.QName("notification", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.Function notification();

        [PageFX.AbcInstanceTrait(40)]
        [PageFX.ABC]
        [PageFX.QName("setNotification", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual object setNotification(Avm.Function f);

        /// <summary>Returns the XML object.</summary>
        /// <returns>The primitive value of an XML instance.</returns>
        [PageFX.AbcInstanceTrait(41)]
        [PageFX.ABC]
        [PageFX.QName("valueOf", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual Avm.XML valueOf();

        /// <summary>
        /// Retrieves the following properties: ignoreComments,
        /// ignoreProcessingInstructions, ignoreWhitespace,
        /// prettyIndent, and prettyPrinting.
        /// </summary>
        /// <returns>
        /// An object with the following XML properties:
        /// ignoreCommentsignoreProcessingInstructionsignoreWhitespaceprettyIndentprettyPrinting
        /// </returns>
        [PageFX.AbcClassTrait(11)]
        [PageFX.ABC]
        [PageFX.QName("settings", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static object settings();

        /// <summary>
        /// Sets values for the following XML properties: ignoreComments,
        /// ignoreProcessingInstructions, ignoreWhitespace,
        /// prettyIndent, and prettyPrinting.
        /// The following are the default settings, which are applied if no setObj parameter
        /// is provided:
        /// XML.ignoreComments = trueXML.ignoreProcessingInstructions = trueXML.ignoreWhitespace = trueXML.prettyIndent = 2XML.prettyPrinting = trueNote: You do not apply this method to an instance of the XML class; you apply it to
        /// XML, as in the following code: XML.setSettings().
        /// </summary>
        [PageFX.AbcClassTrait(12)]
        [PageFX.ABC]
        [PageFX.QName("setSettings", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void setSettings(object rest);

        [PageFX.AbcClassTrait(12)]
        [PageFX.ABC]
        [PageFX.QName("setSettings", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void setSettings();

        /// <summary>
        /// Returns an object with the following properties set to the default values: ignoreComments,
        /// ignoreProcessingInstructions, ignoreWhitespace, prettyIndent, and
        /// prettyPrinting. The default values are as follows:
        /// ignoreComments = trueignoreProcessingInstructions = trueignoreWhitespace = trueprettyIndent = 2prettyPrinting = trueNote: You do not apply this method to an instance of the XML class; you apply it to
        /// XML, as in the following code: var df:Object = XML.defaultSettings().
        /// </summary>
        /// <returns>An object with properties set to the default settings.</returns>
        [PageFX.AbcClassTrait(13)]
        [PageFX.ABC]
        [PageFX.QName("defaultSettings", "http://adobe.com/AS3/2006/builtin", "public")]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static object defaultSettings();

        #region Custom Members
        public extern XML this[int index]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }
        
        public extern XMLList this[Avm.String name]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }
        
        public extern XMLList this[Avm.Namespace ns, Avm.String name]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }
        
        public extern XMLList this[Avm.String ns, Avm.String name]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }
        #endregion



    }
}