using demo_project.Controllers.DataPkg;
using demo_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace demo_project.Controllers;

public class StudentsController : ODataController
{
    public ActionResult<IEnumerable<Student>> Get()
    {
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
        if (!Data.Groups.Any(g => g.Id == student.GroupId))
        {
            return NotFound();
        }

        Data.AddStudent(student);
        student.Group = Data.Groups.SingleOrDefault(g => g.Id == student.GroupId);
        return Created(student);
    }

    [EnableQuery]
    public IActionResult CreateRef([FromRoute] int key, [FromRoute] int relatedKey, [FromRoute] string navigationProperty)
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

    public IActionResult CreateRef([FromRoute] int key, [FromRoute] string navigationProperty, [FromBody] Uri link)
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

        int relatedKey;
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
    public IActionResult DeleteRef([FromRoute] int key, [FromRoute] string navigationProperty)
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
    public IActionResult GetGroup([FromRoute] int key)
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

    private bool TryParseRelatedKey(Uri link, out int relatedKey)
    {
        relatedKey = 0;

        var model = Request.GetRouteServices().GetService(typeof(IEdmModel)) as IEdmModel;
        var serviceRoot = Request.CreateODataLink();

        var uriParser = new ODataUriParser(model, new Uri(serviceRoot), link);
        // NOTE: ParsePath may throw exceptions for various reasons
        ODataPath odataPath = uriParser.ParsePath();
        KeySegment keySegment = odataPath.OfType<KeySegment>().LastOrDefault();

        if (keySegment == null || !int.TryParse(keySegment.Keys.First().Value.ToString(), out relatedKey))
        {
            return false;
        }

        return true;
    }
}
