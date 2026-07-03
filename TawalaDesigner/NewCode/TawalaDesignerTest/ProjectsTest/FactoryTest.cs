using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Factory class
	/// </summary>
	[TestFixture]
	public class FactoryTest
	{
		[Test]
		public void MakeSubclasses()
		{
			string xmlString =
				"<complexThing>" +
				"    <thingOne memberOne=\"1\"/>" +
				"    <thingTwo memberOne=\"2\"/>" +
				"</complexThing>";

			IXmlElement element = new XmlElement(xmlString);

			Factory<Thing> factory = new Factory<Thing>();
			factory.Register("thingOne", "memberOne", typeof(ThingOne));
			factory.Register("thingTwo", "memberOne", typeof(ThingTwo));

			Thing thingOne = factory.MakeObject(element.GetChild("thingOne"));
			Thing thingTwo = factory.MakeObject(element.GetChild("thingTwo"));

			Assert.IsTrue(thingOne is ThingOne);
			Assert.IsTrue(thingTwo is ThingTwo);

			Assert.AreEqual(1, thingOne.memberOne);
			Assert.AreEqual(2, thingTwo.memberOne);
		}

		[Test]
		public void UniqueRegistration()
		{
			Factory<Thing> factory = new Factory<Thing>();

			// start with 2 unique "name" entries
			factory.Register("thingOne", typeof(ThingOne));
			factory.Register("thingTwo", typeof(ThingTwo));
			Assert.AreEqual(2, factory.RegisteredEntries);

			// make sure entry is not duplicated
			factory.Register("thingOne", typeof(ThingOne));
			Assert.AreEqual(2, factory.RegisteredEntries);

			// add a "name/attribute" entry
			factory.Register("thingTwo", "attribute", typeof(ThingTwo));
			Assert.AreEqual(3, factory.RegisteredEntries);

			// make sure "name/attribute" entry replaces previous entry
			factory.Register("thingTwo", "attribute", typeof(ThingTwo));
			Assert.AreEqual(3, factory.RegisteredEntries);

			// add a "name/variable attribute" entry
			factory.Register("thingTwo", typeof(ThingTwo), "attributeOne", "attributeTwo");
			Assert.AreEqual(4, factory.RegisteredEntries);
		}

		[Test]
		public void BestMatchSingleRegistration()
		{
			Factory<Thing> factory = new Factory<Thing>();

			factory.Register("thing", typeof(ThingOne));

			Thing thing;

			IXmlElement element1 = new XmlElement("<thing/>");
			thing = factory.MakeObject(element1);
			Assert.IsTrue(thing is ThingOne);

			IXmlElement element2 = new XmlElement("<thing attributeOne=\"One\"/>");
			thing = factory.MakeObject(element2);
			Assert.IsTrue(thing is ThingOne);

			IXmlElement element3 = new XmlElement("<thing attributeOne=\"One\" attributeTwo=\"Two\"/>");
			thing = factory.MakeObject(element3);
			Assert.IsTrue(thing is ThingOne);
		}

		[Test]
		public void BestMatchMultipleRegistration()
		{
			Factory<Thing> factory = new Factory<Thing>();

			factory.Register("thing", typeof(ThingOne));
			factory.Register("thing", typeof(ThingTwo), "attributeOne");
			factory.Register("thing", typeof(ThingThree), "attributeOne", "attributeTwo");

			Thing thing;

			IXmlElement element1 = new XmlElement("<thing/>");
			thing = factory.MakeObject(element1);
			Assert.IsTrue(thing is ThingOne);

			IXmlElement element2 = new XmlElement("<thing attributeOne=\"One\"/>");
			thing = factory.MakeObject(element2);
			Assert.IsTrue(thing is ThingTwo);

			IXmlElement element3 = new XmlElement("<thing attributeOne=\"One\" attributeTwo=\"Two\"/>");
			thing = factory.MakeObject(element3);
			Assert.IsTrue(thing is ThingThree);
		}

		[Test]
		public void CanMakeObjectFromFactoryMethod()
		{
			Factory<Thing> factory = new Factory<Thing>();

			factory.RegisterFactoryMethod(typeof(AnyClass), "FactoryMethod", "thing");

			Thing thing = factory.MakeObject(new XmlElement("<thing/>"));

			Assert.IsTrue(thing is Thing);
		}

		[Test]
		public void CanMakeObjectFromFactoryMethodUsingElementAndAttribute()
		{
			Factory<Thing> factory = new Factory<Thing>();

			factory.RegisterFactoryMethod(typeof(ThingOne), "FactoryMethod", "thing", "id");

			Thing thing = factory.MakeObject(new XmlElement("<thing id=\"1234\"/>"));

			Assert.IsTrue(thing is Thing);
		}
	}

	class Thing
	{
		public int memberOne;

		public Thing()
		{
		}

		public Thing(IXmlElement element)
		{
			this.memberOne = Convert.ToInt32(element.GetAttribute("memberOne"));
		}
	}

	class ThingOne : Thing
	{
		public ThingOne(IXmlElement element)
			: base(element)
		{
		}

		public static Thing FactoryMethod(IXmlElement element)
		{
			return new ThingOne(element);
		}
	}

	class ThingTwo : Thing
	{
		public ThingTwo(IXmlElement element)
			: base(element)
		{
		}
	}

	class ThingThree : Thing
	{
		public ThingThree(IXmlElement element)
			: base(element)
		{
		}
	}

	class AnyClass
	{
		public static Thing FactoryMethod(IXmlElement element)
		{
			return new ThingOne(element);
		}
	}

}
