﻿#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using RestSharp.Serializers;
using Xunit;

namespace RestSharp.Tests
{
	public class SerializerTests
	{
		[Fact]
		public void Serializes_Properties_In_Specified_Order() {
			var ordered = new OrderedProperties();
			ordered.Name = "Name";
			ordered.Age = 99;
			ordered.StartDate = new DateTime(2010, 1, 1);

			var xml = new XmlSerializer();
			var doc = xml.Serialize(ordered);

			var expected = GetSortedPropsXDoc();

			Assert.Equal(expected.ToString(), doc.ToString());
		}

		[Fact]
		public void Can_serialize_simple_POCO() {
			var poco = new Person {
				Name = "Foo",
				Age = 50,
				Price = 19.95m,
				StartDate = new DateTime(2009, 12, 18, 10, 2, 23),
				Items = new List<Item> {
					new Item { Name = "One", Value = 1 },
					new Item { Name = "Two", Value = 2 },
					new Item { Name = "Three", Value = 3 }
				}
			};

			var xml = new XmlSerializer();
			var doc = xml.Serialize(poco);
			var expected = GetSimplePocoXDoc();

			Assert.Equal(expected.ToString(), doc.ToString());
		}

		[Fact]
		public void Can_serialize_simple_POCO_With_DateFormat_Specified() {
			var poco = new Person {
				Name = "Foo",
				Age = 50,
				Price = 19.95m,
				StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
			};

			var xml = new XmlSerializer();
			xml.DateFormat = DateFormats.Iso8601;
			var doc = xml.Serialize(poco);
			var expected = GetSimplePocoXDocWithIsoDate();

			Assert.Equal(expected.ToString(), doc.ToString());
		}

		[Fact]
		public void Can_serialize_simple_POCO_With_Different_Root_Element() {
			var poco = new Person {
				Name = "Foo",
				Age = 50,
				Price = 19.95m,
				StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
			};

			var xml = new XmlSerializer();
			xml.RootElement = "Result";
			var doc = xml.Serialize(poco);
			var expected = GetSimplePocoXDocWithRoot();

			Assert.Equal(expected.ToString(), doc.ToString());
		}

		[Fact]
		public void Can_serialize_simple_POCO_With_Attribute_Options_Defined() {
			var poco = new WackyPerson {
				Name = "Foo",
				Age = 50,
				Price = 19.95m,
				StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
			};

			var xml = new XmlSerializer();
			var doc = xml.Serialize(poco);
			var expected = GetSimplePocoXDocWackyNames();

			Assert.Equal(expected.ToString(), doc.ToString());
		}

		private class Person
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public decimal Price { get; set; }
			public DateTime StartDate { get; set; }
			public List<Item> Items { get; set; }
		}

		private class Item
		{
			public string Name { get; set; }
			public int Value { get; set; }
		}

		private class WackyPerson
		{
			[SerializeAs(Name = "WackyName", Attribute = true)]
			public string Name { get; set; }

			public int Age { get; set; }

			[SerializeAs(Attribute = true)]
			public decimal Price { get; set; }

			[SerializeAs(Name = "start_date", Attribute = true)]
			public DateTime StartDate { get; set; }
		}

		private XDocument GetSimplePocoXDoc() {
			var doc = new XDocument();
			var root = new XElement("Person");
			root.Add(new XElement("Name", "Foo"),
					new XElement("Age", 50),
					new XElement("Price", 19.95m),
					new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

			var items = new XElement("Items");
			items.Add(new XElement("Item", new XElement("Name", "One"), new XElement("Value", 1)));
			items.Add(new XElement("Item", new XElement("Name", "Two"), new XElement("Value", 2)));
			items.Add(new XElement("Item", new XElement("Name", "Three"), new XElement("Value", 3)));
			root.Add(items);

			doc.Add(root);

			return doc;
		}

		private XDocument GetSimplePocoXDocWithIsoDate() {
			var doc = new XDocument();
			var root = new XElement("Person");
			root.Add(new XElement("Name", "Foo"),
					new XElement("Age", 50),
					new XElement("Price", 19.95m),
					new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString("s")));

			doc.Add(root);

			return doc;
		}

		private XDocument GetSimplePocoXDocWithRoot() {
			var doc = new XDocument();
			var root = new XElement("Result");

			var start = new XElement("Person");
			start.Add(new XElement("Name", "Foo"),
					new XElement("Age", 50),
					new XElement("Price", 19.95m),
					new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

			root.Add(start);
			doc.Add(root);

			return doc;
		}

		private XDocument GetSimplePocoXDocWackyNames() {
			var doc = new XDocument();
			var root = new XElement("WackyPerson");
			root.Add(new XAttribute("WackyName", "Foo"),
					new XElement("Age", 50),
					new XAttribute("Price", 19.95m),
					new XAttribute("start_date", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

			doc.Add(root);

			return doc;
		}

		private XDocument GetSortedPropsXDoc() {
			var doc = new XDocument();
			var root = new XElement("OrderedProperties");

			root.Add(new XElement("StartDate", new DateTime(2010, 1, 1).ToString()));
			root.Add(new XElement("Name", "Name"));
			root.Add(new XElement("Age", 99));

			doc.Add(root);

			return doc;
		}
	}
}