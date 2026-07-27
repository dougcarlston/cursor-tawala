package com.tawala.external;

import java.util.HashMap;
import java.util.Map;

import com.tawala.TestCase;
import com.thoughtworks.xstream.XStream;
import com.thoughtworks.xstream.annotations.Annotations;
import com.thoughtworks.xstream.annotations.XStreamAlias;
import com.thoughtworks.xstream.annotations.XStreamConverter;
import com.thoughtworks.xstream.converters.Converter;
import com.thoughtworks.xstream.converters.MarshallingContext;
import com.thoughtworks.xstream.converters.UnmarshallingContext;
import com.thoughtworks.xstream.io.HierarchicalStreamReader;
import com.thoughtworks.xstream.io.HierarchicalStreamWriter;

public class XStreamTest extends TestCase {

    public void testBasicOutput() {
        Person person = new Person("Joe Smith");
        XStream xstream = new XStream();
        Annotations.configureAliases(xstream, Person.class);
        assertEquals("<person>\n  <name>Joe Smith</name>\n</person>", xstream.toXML(person));
    }

    public void testBasicInput() {
        XStream xstream = new XStream();
        Annotations.configureAliases(xstream, Person.class);
        Person person = (Person) xstream.fromXML("<person>\n  <name>Joe Smith</name>\n</person>");
        assertEquals("Joe Smith", person.getName());
    }

    public void testSchemaChange() {
        XStream xstream = new XStream();
        Annotations.configureAliases(xstream, Person2.class);
        Person2 person = (Person2) xstream.fromXML("<person>\n  <name>Joe Smith</name>\n</person>");
        assertEquals("Joe", person.getFirstName());
        assertEquals("Smith", person.getLastName());
        assertEquals("<person>\n  <firstName>Joe</firstName>\n  <lastName>Smith</lastName>\n</person>", xstream.toXML(person));
    }

    @XStreamAlias("person")
    private static class Person {
        private final String name;

        public Person(String name) {
            this.name = name;
        }

        public String getName() {
            return name;
        }

    }

    @XStreamAlias("person")
    @XStreamConverter(XStreamTest.PersonConverter.class)
    private static class Person2 {
        private String firstName;
        private String lastName;


        public Person2(String firstName, String lastName) {
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public String getFirstName() {
            return firstName;
        }

        public String getLastName() {
            return lastName;
        }

        public void setName(String name) {
            firstName = name;
        }

    }

    public static class PersonConverter implements Converter {

        public boolean canConvert(Class type) {
            return type.equals(Person.class) || type.equals(Person2.class);
        }

        public void marshal(Object source, HierarchicalStreamWriter writer, MarshallingContext context) {
            writer.startNode("firstName");
            writer.setValue(((Person2) source).getFirstName());
            writer.endNode();
            writer.startNode("lastName");
            writer.setValue(((Person2) source).getLastName());
            writer.endNode();
        }

        public Object unmarshal(HierarchicalStreamReader reader, UnmarshallingContext context) {
            Map<String, String> data = new HashMap<String, String>();
            while (reader.hasMoreChildren()) {
                reader.moveDown();
                data.put(reader.getNodeName(), reader.getValue());
                reader.moveUp();
            }
            if (data.containsKey("firstName")) {
                return new Person2(data.get("firstName"), data.get("lastName"));
            } else {
                String[] nameParts = data.get("name").split(" ", 2);
                return new Person2(nameParts[0], nameParts[1]);
            }
        }
    }
}
