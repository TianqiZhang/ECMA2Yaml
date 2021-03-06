﻿using System;
using System.Collections.Generic;

namespace CatLibrary
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Here's main class of this <i>Demo</i>.</para>
    /// <para>You can see mostly type of article within this class and you for more detail, please see the remarks.</para>
    /// <para></para>
    /// <para>this class is a template class. It has two Generic parameter. they are: <typeparamref name="T"/> and <typeparamref name="K"/>.</para>
    /// <para>The extension method of this class can refer to <see cref="ICatExtension"/> class</para>
    /// it is to test the task:383436 scenario1 <see href="https://ceapex.visualstudio.com/Engineering/_workitems/edit/383436" data-linktype="absolute-path"/>
    /// it is to test the task:383436 scenario2 <see href="https://ceapex.visualstudio.com/Engineering/_workitems/edit/383436">Task:383436</see>
    /// it is to test the task:383436 scenario3 <see href="https://ceapex.visualstudio.com/Engineering/_workitems/edit/383436"></see>
    /// <seealso href="http://some.external.net/embeddedcontent">SeeAlsoEmbeddedContent</seealso>
    /// <seealso href="http://some.external.net/embeddedselfclose"/>
    /// </summary>
    /// <seealso cref="CatLibrary.ICat"/>
    /// <seealso href="http://some.external.net/content">SeeAlsoContent</seealso>
    /// <seealso href="http://some.external.net/nocontent"></seealso>
    /// <seealso href="http://some.external.net/selfclose"/>
    /// <example>
    /// <para>Here's example of how to create an instance of this class. As T is limited with <c>class</c> and K is limited with <c>struct</c>.</para>
    /// <code language="c#">
    ///     var a = new Cat(object, int)();
    ///     int catNumber = new int();
    ///     unsafe
    ///     {
    ///         a.GetFeetLength(catNumber);
    ///     }
    /// </code>
    /// <para>As you see, here we bring in <b>pointer</b> so we need to add <languageKeyword>unsafe</languageKeyword> keyword.</para>
    /// </example>
    /// <typeparam name="T">This type should be class and can new instance.</typeparam>
    /// <typeparam name="K">This type is a struct type, class type can't be used for this parameter.</typeparam>
    /// <remarks>
    /// <para>Here's all the content you can see in this class.</para>
    /// <see href="http://some.external.net/see-remarkscontent">SeeRemarksContent</see>
    /// <see href="http://some.external.net/see-remarksselfclose"/>
    /// <seealso href="http://some.external.net/seealso-remarkscontent">SeeAlsoRemarksContent</seealso>
    /// <seealso href="http://some.external.net/seealso-remarksselfclose"/>
    /// <list type="ordered">
    /// <listItem>Constructors. With different input parameters.</listItem>
    /// <listItem>
    /// <b>Methods</b>. Including:
    /// <list>
    /// <listItem>
    /// Template method.
    /// </listItem>
    /// <listItem>
    /// Normal method wit generic parameter.
    /// </listItem>
    /// <listItem>
    /// Override method.
    /// </listItem>
    /// <listItem>
    /// unsafe method with pointer.
    /// </listItem>
    /// </list>
    /// </listItem>
    /// <listItem><b>Operators</b>. You can also see explicit operator here.</listItem>
    /// <listItem><b>Properties</b>. Include normal property and index.</listItem>
    /// <listItem><b>Events</b>.</listItem>
    /// <listItem><b>Fields</b>.</listItem>
    /// <listItem><b>EII</b>. ExplicitImplementInterface. including eii property, eii method, eii event.</listItem>
    /// <listItem><b>Extension Methods</b>. The extension methods not definition in this class, but we can find it!</listItem>
    /// </list>
    /// </remarks>
    [Serializable]
    public class Cat<[Doc("T is a class")] T, [Doc("K is a struct")] K> : ICat
        where T : class, new()
        where K : struct
    {
        //Constructors: normal with parameter
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        ///   This is an example of the format that is used by some libraries, where
        ///   the lines of the remarks are broken up to increase readability for other
        ///   developers, but should be rendered as one paragraph for MS Docs.
        /// </remarks>
        public Cat() { }

        /// <summary>
        /// Constructor with one generic parameter.
        /// </summary>
        /// <param name="ownType">This parameter type defined by class.</param>
        /// <remarks>
        ///   This is an example of the format that is used by some libraries, where
        ///   the lines of the remarks are broken up to increase readability.
        ///
        ///   Because of the blank line that precedes it, this should be considered
        ///   a new paragraph.  Intellisense renders this correctly in VS, the
        ///   MS Docs rendering should follow suit.
        /// </remarks>
        public Cat(T ownType) { }

        /// <summary>
        /// It's a complex constructor. The parameter will have some attributes.
        /// </summary>
        /// <param name="nickName">it's string type.</param>
        /// <param name="age">It's an out and ref parameter.</param>
        /// <param name="realName">It's an out paramter.</param>
        /// <param name="isHealthy">It's an in parameter.</param>
        /// <remarks> Retry policies instruct the Storage Client to retry failed requests.
        /// By default, only some failures are retried. For example, connection failures and
        /// throttling failures can be retried. Resource not found (404) or authentication
        /// failures are not retried, because these are not likely to succeed on retry.
        /// If not set, the Storage Client uses an exponential backoff retry policy, where the wait time gets
        /// exponentially longer between requests, up to a total of around 30 seconds.
        /// The default retry policy is recommended for most scenarios.
        ///
        ///## Examples
        ///  [!code-csharp[Retry_Policy_Sample](~/azure-storage-net/Test/ClassLibraryCommon/Blob/BlobUploadDownloadTest.cs#sample_RequestOptions_RetryPolicy "Retry Policy Sample")]
        ///</remarks>
        public Cat(string nickName, out int age, [Out] string realName, [In] bool isHealthy) { age = 1; }

        //Methods: template + normal with generic type + pointer method
        /// <summary>
        /// It's a method with complex return type.
        /// </summary>
        /// <param name="date">Date time to now.</param>
        /// <returns>It's a relationship map of different kind food.</returns>
        public Dictionary<string, List<int>> CalculateFood(DateTime date) { return null; }

        /// <summary>
        /// This method have attribute above it.
        /// </summary>
        /// <param name="ownType">Type come from class define.</param>
        /// <param name="anotherOwnType">Type come from class define.</param>
        /// <param name="cheat">Hint whether this cat has cheat mode.</param>
        /// <exception cref="ArgumentException">This is an argument exception</exception>
        [Conditional("Debug")]
        public void Jump(T ownType, K anotherOwnType, ref bool cheat)
        {
            EventHandler handler = ownEat;
        }

        /// <summary>
        /// Override the method of <c>Object.Equals(object obj).</c>
        /// </summary>
        /// <param name="obj">Can pass any class type.</param>
        /// <returns>The return value tell you whehter the compare operation is successful.</returns>
        public override bool Equals(object obj) { return false; }

        /// <summary>
        /// It's an <c>unsafe</c> method.
        /// As you see, <paramref name="catName"/> is a <b>pointer</b>, so we need to add <languageKeyword>unsafe</languageKeyword> keyword.
        /// </summary>
        /// <param name="catName">Thie represent for cat name length.</param>
        /// <param name="parameters">Optional parameters.</param>
        /// <returns>Return cat tail's length.</returns>
        public unsafe long GetTailLength(int* catName, params object[] parameters) { return 1; }

        //operator
        /// <summary>
        /// Addition operator of this class.
        /// </summary>
        /// <param name="lsr">...</param>
        /// <param name="rsr">~~~</param>
        /// <returns>Result with <i>int</i> type.</returns>
        public static int operator +(Cat<T, K> lsr, int rsr) { return 1; }

        /// <summary>
        /// Similar with operaotr +, refer to that topic.
        /// </summary>
        public static int operator -(Cat<T, K> lsr, int rsr) { return 1; }

        /// <summary>
        /// Expilicit operator of this class.
        /// <para>It means this cat can evolve to change to Tom. Tom and Jerry.</para>
        /// </summary>
        /// <param name="src">Instance of this class.</param>
        /// <returns>Advanced class type of cat.</returns>
        public static explicit operator Tom(Cat<T, K> src) { return null; }

        //Property: index + normal
        /// <summary>
        /// This is index property of Cat. You can see that the visibility is different between <c>get</c> and <c>set</c> method.
        /// </summary>
        /// <param name="a">Cat's name.</param>
        /// <returns>Cat's number.</returns>
        public int this[string a]
        {
            protected get { return 1; }
            set { }
        }

        /// <summary>
        /// Hint cat's age.
        /// </summary>
        /// <remarks>
        /// <format langauge="markdown">
        /// ```csharp
        /// var cat = new Cat();
        /// var age = cat.Age;  // Will always be 1.
        /// ```
        /// </format>
        /// </remarks>
        protected int Age
        {
            get { return 1; }
            set { }
        }

        //event
        /// <summary>
        /// Eat event of this cat
        /// </summary>
        /// <remarks>
        /// <format>
        ///   This should not be changed.
        ///   Not this line.
        ///
        ///   Nor this one.
        /// </format>
        /// </remarks>
        public event EventHandler ownEat;

        //Field: with attribute
        /// <summary>
        /// Field with attribute.
        /// </summary>
        [ContextStatic]
        [NonSerialized]
        [Obsolete("mark IsHealthy as obsolete for testing")]
        public bool isHealthy;

        //EII Method
        /// <summary>
        /// EII method.
        /// </summary>
        void IAnimal.Eat() { }
        /// <summary>
        /// EII template method.
        /// </summary>
        /// <typeparam name="Tool">Tool for eat.</typeparam>
        /// <param name="a">Tool name.</param>
        void IAnimal.Eat<Tool>(Tool a) { }

        /// <summary>
        /// Implementation of Eat(food)
        /// </summary>
        /// <param name="food">Food to eat</param>
        void IAnimal.Eat(string food) { }

        /// <summary>
        /// This one can't.
        /// </summary>
        public void CatchJerry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If you force it, maybe it can.
        /// </summary>
        [Obsolete("hi,mark CatchJerry(bool force) as obsolete for testing",true)]
        public void CatchJerry(bool force)
        {
            throw new NotImplementedException();
        }

        //EII Property
        /// <summary>
        /// EII property.
        /// </summary>
       [Obsolete("hi,mark Name as obsolete for testing")]
        public string Name { get { return "Pig"; } }

        /// <summary>
        /// EII index.
        /// </summary>
        /// <param name="a">Cat's number.</param>
        /// <returns>Cat's name.</returns>
        string IAnimal.this[int a] { get { return "Animal"; } }

        //EII Event
        /// <summary>
        /// EII event.
        /// </summary>
        event EventHandler ICat.eat
        {
            add { ownEat += value; }
            remove { ownEat -= value; }
        }
    }
}
