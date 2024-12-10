using demo_project.Controllers.DataPkg;
using demo_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace demo_project.Controllers;

public class GroupsController : ODataController
{

    [EnableQuery]
    public ActionResult<IEnumerable<Group>> Get()
    {
        return Ok(Data.Groups);
    }

    [EnableQuery]
    public ActionResult<Group> Get(uint key)
    {
        var item = Data.Groups.SingleOrDefault(g => g.Id == key);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [EnableQuery]
    public IActionResult GetStudents([FromRoute] int key)
    {
        var students = Data.Students.Where(s => s.GroupId == key);
        return Ok(students);
    }

}
