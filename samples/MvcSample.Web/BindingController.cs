using System.Text;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using MvcSample.Web.Models;

namespace MvcSample.Web.RandomNameSpace
{
    public class BindingController
    {
        // This is model bound from the query string. 
        public int Count { get; set; }

        [Activate]
        public IModelMetadataProvider ModelMetadataProvider { get; set; }

        public string Bind([FromHeader("Accept")] string acceptHeader, Person person, PersonFromHawai hawainPerson)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("AcceptHeader: " + acceptHeader);
            stringBuilder.AppendLine("ModelBoundProperty:Count: " + Count);
            stringBuilder.AppendLine("InjectedProperty:ModelMetadataProvider: " + ModelMetadataProvider.ToString());
            stringBuilder.AppendLine("ModelBoundComplexObject:Person.Name: " + person.Name);
            stringBuilder.AppendLine("ModelBoundComplexObject:Person.Age: " + person.Age);
            stringBuilder.AppendLine("ModelBoundComplexObject:Person.Parent: " + person.Parent); 

            stringBuilder.AppendLine("ModelBoundComplexObject:HawaianPerson.Age: " + hawainPerson?.Age);
            return stringBuilder.ToString();
        }
    }

    public class Person
    {
        // should be set from the query string.
        public int Age { get; set; }

        [FromHeader("Host")]
        public string Name { get; set; }

        // Should be set to null
        public Person Parent { get; set; }
    }

    [FromBody]
    public class PersonFromHawai
    {
        // should be set from the query string.
        public int Age { get; set; }

        public string Name { get; set; }

        // Should be set to null
        public Person Parent { get; set; }
    }
}