using demo_project.Controllers.DataPkg;
using demo_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Reflection;

namespace demo_project.Controllers;

public class StudentsController : ODataController
{
    [EnableQuery]
    public ActionResult<IEnumerable<Student>> Get()
    {
        var header = Request.GetTypedHeaders();
        if (header.IfModifiedSince.HasValue && (int)((DateTimeOffset)header.IfModifiedSince - Data.StudentLastModified).TotalSeconds >= 0)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        this.Response.GetTypedHeaders().LastModified = Data.StudentLastModified;
        return Ok(Data.Students);
    }

    [EnableQuery]
    public IActionResult Get([FromRoute] uint key)
    {
        var item = Data.Students.SingleOrDefault(s => s.Id == key);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public ActionResult Post([FromBody] Student student)
    {
        if (student.GroupId != null && !Data.Groups.Any(g => g.Id == student.GroupId))
        {
            return NotFound();
        }

        Data.AddStudent(student);
        student.Group = Data.Groups.SingleOrDefault(g => g.Id == student.GroupId);
        return Created(student);
    }

    [EnableQuery]
    public IActionResult CreateRef([FromRoute] uint key, [FromRoute] uint relatedKey, [FromRoute] string navigationProperty)
    {
        if (navigationProperty != "Group")
        {
            return BadRequest();
        }

        var student = Data.Students.SingleOrDefault(s => s.Id == key);
        var group = Data.Groups.SingleOrDefault(g => g.Id == relatedKey);

        if (student == null || group == null)
        {
            return NotFound();
        }

        student.GroupId = group.Id;
        student.Group = group;

        return NoContent();
    }

    public IActionResult CreateRef([FromRoute] uint key, [FromRoute] string navigationProperty, [FromBody] Uri link)
    {
        if (navigationProperty != "Group")
        {
            return BadRequest();
        }

        var student = Data.Students.SingleOrDefault(s => s.Id == key);

        if (student == null)
        {
            return BadRequest();
        }

        uint relatedKey;
        // The code for TryParseRelatedKey is shown a little further below
        if (!TryParseRelatedKey(link, out relatedKey))
        {
            return BadRequest();
        }

        var group = Data.Groups.SingleOrDefault(g => g.Id == relatedKey);

        if (student == null || group == null)
        {
            return NotFound();
        }

        student.GroupId = group.Id;
        student.Group = group;

        return NoContent();
    }

    [EnableQuery]
    public IActionResult DeleteRef([FromRoute] uint key, [FromRoute] string navigationProperty)
    {
        if (navigationProperty != "Group")
        {
            return BadRequest();
        }

        var student = Data.Students.SingleOrDefault(s => s.Id == key);

        if (student == null)
        {
            return NotFound();
        }

        student.GroupId = null;

        return NoContent();
    }

    [EnableQuery]
    public IActionResult GetGroup([FromRoute] uint key)
    {
        var student = Data.Students.SingleOrDefault(s => s.Id == key);
        if (student == null)
        {
            return NotFound(new ErrorResponse
            {
                DeveloperMessage = "Student not found, make sure the key is correct.",
                UserMessage = "Student not found.",
                ErrorCode = "1050",
                MoreInfo = "http://example.com/errors/1050"
            });
        }
        if (student.Group == null)
        {
            return NotFound(new ErrorResponse
            {
                DeveloperMessage = "This student has not joined a group. Check GroupId property to see if the student has joined a group.",
                UserMessage = "This student has not joined a group.",
                ErrorCode = "1132",
                MoreInfo = "http://example.com/errors/1132"
            });
        }

        return Ok(student.Group);
    }

    private bool TryParseRelatedKey(Uri link, out uint relatedKey)
    {
        relatedKey = 0;

        var model = Request.GetRouteServices().GetService(typeof(IEdmModel)) as IEdmModel;
        var serviceRoot = Request.CreateODataLink();

        var uriParser = new ODataUriParser(model, new Uri(serviceRoot), link);
        // NOTE: ParsePath may throw exceptions for various reasons
        ODataPath odataPath = uriParser.ParsePath();
        KeySegment keySegment = odataPath.OfType<KeySegment>().LastOrDefault();

        if (keySegment == null || !uint.TryParse(keySegment.Keys.First().Value.ToString(), out relatedKey))
        {
            return false;
        }

        return true;
    }

    public ActionResult Put([FromRoute] uint key, [FromBody] Student student)
    {
        var item = Data.Students.SingleOrDefault(d => d.Id == key);

        if (item == null)
        {
            return NotFound();
        }
        else if (!item.GetType().Equals(student.GetType()))
        {
            return BadRequest();
        }

        // Update properties using reflection
        foreach (var propertyInfo in student.GetType().GetProperties(
            BindingFlags.Public | BindingFlags.Instance))
        {
            var itemPropertyInfo = item.GetType().GetProperty(
                propertyInfo.Name,
                BindingFlags.Public | BindingFlags.Instance);

            if (itemPropertyInfo.CanWrite)
            {
                itemPropertyInfo.SetValue(item, propertyInfo.GetValue(student));
            }
        }
        Data.ResetStudentLastModified();

        return Ok();
    }

    public ActionResult Patch([FromRoute] uint key, [FromBody] Delta<Student> delta)
    {
        var student = Data.Students.SingleOrDefault(d => d.Id == key);

        if (student == null)
        {
            return NotFound();
        }
        else if (!student.GetType().Equals(delta.StructuredType))
        {
            return BadRequest();
        }

        delta.Patch(student);
        Data.ResetStudentLastModified();

        return Ok();
    }

    public ActionResult Delete([FromRoute] uint key)
    {
        var student = Data.Students.SingleOrDefault(d => d.Id == key);

        if (student == null)
        {
            return NotFound();
        }

        Data.RemoveStudent(student.Id);

        return NoContent();
    }
}
